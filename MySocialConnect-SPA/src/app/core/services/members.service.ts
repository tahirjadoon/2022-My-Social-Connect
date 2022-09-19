import { Injectable } from '@angular/core';

import { ApiUrlService } from './api-url.service';
import { HttpClientService } from './http-client.service';
import { LocalStorageService } from './local-storage.service';

import { environment } from 'src/environments/environment';

import { userDto } from '../models/userDto.model';
import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  //to pass the auth token to the api, later will use interceptor
  private httpOptions;

  constructor(private apiUrlService: ApiUrlService, private httpClientService: HttpClientService, private localStorageService: LocalStorageService) {
    //to pass the auth token to the api, later will user interceptor
    //not using this any more, using the interceptor
    this.httpOptions = {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + localStorageService.getLoggedinToken
      })
    };
  }

  /**
   * A GET method
   * @returns returns Observable userDto[] 
   */
  getMembers() : Observable<userDto[]> {
    const url = this.apiUrlService.usersAll;
    if (environment.displayConsoleLog) console.log(`Users allUrl: ${url}`);
    //users end point is protected by authentication so need to send the token as well 
    //check jwt interceptor for details
    /*
    return this.httpClientService
      .get<userDto[]>(url, this.httpOptions);
    */
      return this.httpClientService.get<userDto[]>(url); 
    }

  /**
   * A GET method
   * @param id id to fetch
   * @returns returns Observable userDto 
   */
   getMemberByGuId(guid: string): Observable<userDto>{
    let url = this.apiUrlService.userByGuId;
    //replace [guid] with the guid in the url 
    url = url.replace(this.apiUrlService.userByGuIdReplace, guid.toString());
    if (environment.displayConsoleLog) console.log(`Users by id url: ${url}`)
    //users end point is protected by authentication so need to send the token as well
    //check jwt interceptor for details
    /*
    return this.httpClientService
      .get<userDto>(url, this.httpOptions);
      */
      return this.httpClientService.get<userDto>(url);
  }
  
  /**
   * A GET method
   * @param id id to fetch
   * @returns returns Observable userDto 
   */
  getMemberById(id: number): Observable<userDto>{
    let url = this.apiUrlService.userById;
    //replace [id] with the id in the url 
    url = url.replace(this.apiUrlService.userByIdReplace, id.toString());
    if (environment.displayConsoleLog) console.log(`Users by id url: ${url}`);
    //users end point is protected by authentication so need to send the token as well
    //check jwt interceptor for details
    /*
    return this.httpClientService
      .get<userDto>(url, this.httpOptions);
      */
      return this.httpClientService.get<userDto>(url);
  }

  /**
   * A GET method
   * @param userName userName to fetch
   * @returns returns Observable userDto 
   */
  getMemberByUserName(userName: string): Observable<userDto>{
    let url = this.apiUrlService.userByName;
    //replace [name] with the userName in the url 
    url = url.replace(this.apiUrlService.userByNameReplace, userName);
    if (environment.displayConsoleLog) console.log(`Users by userName url: ${url}`);
    //users end point is protected by authentication so need to send the token as well
    //check jwt interceptor for details
    /*return this.httpClientService
      .get<userDto>(url, this.httpOptions);
    */
      return this.httpClientService.get<userDto>(url);
  }
}
