import { Injectable } from '@angular/core';
import { map, ReplaySubject } from 'rxjs';

import { environment } from '../../../environments/environment';

import { HttpClientService } from './http-client.service';
import { ApiUrlService } from './api-url.service';
import { LocalStorageService } from './local-storage.service';
import { PresenceHubService } from './signalr/presence-hub.service';

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

  constructor(private apiUrlService: ApiUrlService,
    private httpClientService: HttpClientService,
    private localStorageService: LocalStorageService,
    private presenceService: PresenceHubService) { }

  //fire current user
  private fireCurrentUser(user: UserTokenDto) {
    this.currentUserSource.next(user);
  }

  //login the user. receives LoginDto and returns UserTokenDto
  login(loginDto: LoginDto) {
    var url = this.apiUrlService.accountLogin;

    if(environment.displayConsoleLog) console.log(`AccountService LoginUrl: ${url}`);

    //persist the user in the local storage
    return this.httpClientService
      .post<UserTokenDto>(url, loginDto)
      .pipe(
        map((respone: UserTokenDto) => {
          const user = respone;
          if(environment.displayConsoleLog) console.log(user);
          if (user) {
            this.setAndFireCurrentUser(user);
            //signalr presence - create hub connection to be notified
            this.presenceService.createHubConnection(user);
          }
          return user;
        })
      );
  }

  setAndFireCurrentUser(user: UserTokenDto) {
    //decode token and add roles to the user. Keep in mind some users may only have single role so that is not a string[]
    user.roles = [];
    const roles = this.getDecodedToken(user.token)?.role;
    if (roles) {
      Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    }
    //store the user in local storage
    this.localStorageService.setItem(this.localStorageService._keyUser, user);
    this.fireCurrentUser(user);
  }

  //this is being called from inside app.component.ts
  getAndFireCurrentUser() {
    const user: UserTokenDto = this.localStorageService.getItem(this.localStorageService._keyUser);
    if (!user) return;
    this.fireCurrentUser(user);
    //signalr presence - create hub connection to be notified
    this.presenceService.createHubConnection(user);
  }

  logout() {
    //remove the user from local storage
    this.localStorageService.removeItem(this.localStorageService._keyUser);
    this.fireCurrentUser(null!);
    //signalr presence - stop hub connection
    this.presenceService.stopHubConnection();
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
            this.setAndFireCurrentUser(user);
            //signalr presence - create hub connection to be notified
            this.presenceService.createHubConnection(user);
          }
          return user;
        })
      );
  }

  getDecodedToken(token: any) :any {
    //token is not encypted, the signature is.
    //get the user roles from the token
    //The atob() method decodes a string that has been encoded by the btoa() method
    //token comes in three parts seperated by the . It is Header, Payload and signature
    //interested in the middle part
    var parsedToken = token.split(".")[1];
    var decoded = JSON.parse(atob(parsedToken));
    return decoded;
  }
}
