import { Injectable } from '@angular/core';
import { map, ReplaySubject } from 'rxjs';

import { environment } from '../../../environments/environment';

import { HttpClientService } from './http-client.service';
import { ApiUrlService } from './api-url.service';
import { LocalStorageService } from './local-storage.service';

import { LoginDto } from '../models/loginDto.model';
import { UserTokenDto } from '../models/userTokenDto.model';
import { SiteRegisterDto } from '../models/siteRegisterDto.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  //create an observable to store the user in 
  //replaySubject is a special type of observable.
  //replaySubject is kind of buffer object, it will store the values and any time a subscriber subscribes to it, it will emit the last value inside it
  private currentUserSource = new ReplaySubject<UserTokenDto>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private apiUrlService: ApiUrlService, private httpClientService: HttpClientService, private localStorageService: LocalStorageService) { }

  //login the user. receives LoginDto and returns UserTokenDto
  login(loginDto: LoginDto) {
    var url = this.apiUrlService.accountLogin;

    if(environment.displayConsoleLog)
      console.log(`AccountService LoginUrl: ${url}`);

    //this is directly calling the http post method
    //return this.http.post(url, loginDto);

    //using the httpClientService to make the http calls
    //we'll also persist the user in the local storage
    return this.httpClientService
      .post<UserTokenDto>(url, loginDto)
      .pipe(
        map((respone: UserTokenDto) => {
          const user = respone;
          if (user) {
            //store the user in local storage
            this.localStorageService.setItem(this.localStorageService._keyUser, user);
            this.currentUserSource.next(user);
          }
          return user;
        })
      );
  }

  getAndFireCurrentUser() {
    const user: UserTokenDto = this.localStorageService.getItem(this.localStorageService._keyUser);
    this.fireCurrentUser(user);
  }

  fireCurrentUser(user: UserTokenDto) {
    this.currentUserSource.next(user);
  }

  logout() {
    //remove the user from local storage
    this.localStorageService.removeItem(this.localStorageService._keyUser);
    this.currentUserSource.next(null!);
  }

  register(registerDto: SiteRegisterDto) {
    var url = this.apiUrlService.accountRegisterUser;

    if(environment.displayConsoleLog)
      console.log(`AccountService RegisterUrl: ${url}`);
    
    return this.httpClientService
      .post<UserTokenDto>(url, registerDto)
      .pipe(
        map((respone: UserTokenDto) => {
          const user = respone;
          if (user) {
            //store the user in local storage
            this.localStorageService.setItem(this.localStorageService._keyUser, user);
            this.currentUserSource.next(user);
          }
          return user;
        })
      );
  }
}
