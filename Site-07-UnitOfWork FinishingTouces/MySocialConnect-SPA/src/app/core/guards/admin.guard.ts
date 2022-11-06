import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { map, Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { AccountService } from '../services/account.service';
import { zRoles } from '../enums/zRoles';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  constructor(private accountService: AccountService, private toastrService: ToastrService) { }

  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user.roles && (user.roles.includes(zRoles.Admin) || user.roles.includes(zRoles.Moderator)))
          return true;

        this.toastrService.error('You cannot enter this area', 'Restricted Area');
        return false;
      })
    );
  }
}
