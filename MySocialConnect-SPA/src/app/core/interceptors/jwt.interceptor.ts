import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, take } from 'rxjs';

import { AccountService } from '../services/account.service';
import { UserTokenDto } from '../models/userTokenDto.model';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  //when we login the current user is setup via account service. Check the login method
  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    
    let currentUser: UserTokenDto;
    
    //subscribe. only taking the first item 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => currentUser = user);

    if (currentUser!) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`
        }
      });
    }

    return next.handle(request);
  }
}
