import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { map, Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { AccountService } from '../services/account.service';
import { UserTokenDto } from '../models/userTokenDto.model';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private accountService: AccountService, private toastrService: ToastrService) { }

  canActivate(): Observable<boolean>  {
    //account service already has the observable that we will use. If it is null then the user is not logged in
    return this.accountService.currentUser$.pipe(
      map((user: UserTokenDto) => {
        if (user)
          return true;
        //in real time we do not do this. 
        this.toastrService.error("You are not authorized to view the resource. Login please!");
        return false;
      })
    );
  }
  
}
