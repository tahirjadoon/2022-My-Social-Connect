import { Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';

import { ApiUrlService } from './api-url.service';
import { HttpClientService } from './http-client.service';
import { LocalStorageService } from './local-storage.service';

import { AppConstants } from '../constants/app-constants';

import { environment } from '../../../environments/environment';

import { userDto } from '../models/userDto.model';
import { HttpHeaders } from '@angular/common/http';



@Injectable({
  providedIn: 'root'
})
export class MembersService {
  //to pass the auth token to the api, later will use interceptor
  private httpOptions;

  //state mangement. We dont want to pull the user info every time
  members: userDto[] = [];

  constructor(private apiUrlService: ApiUrlService, private httpClientService: HttpClientService, private localStorageService: LocalStorageService) {
    //to pass the auth token to the api, later will user interceptor
    //not using this any more, using the interceptor
    this.httpOptions = {
      headers: new HttpHeaders({
        Authorization: AppConstants.Bearer + localStorageService.getLoggedinToken
      })
    };
  }

  /**
   * A GET method
   * @returns returns Observable userDto[] 
   */
  getMembers(): Observable<userDto[]> {
    //state mangement. when we habe members then return that
    if (this.members.length > 0) {
      //return observable
      return of(this.members);
    }

    const url = this.apiUrlService.usersAll;
    if (environment.displayConsoleLog) console.log(`Users allUrl: ${url}`);
    //users end point is protected by authentication so need to send the token as well 
    //check jwt interceptor for details
    /*
    return this.httpClientService
      .get<userDto[]>(url, this.httpOptions);
    */
    return this.httpClientService.get<userDto[]>(url).pipe(
        map(members => {
          //set the members for state management and return as well  
          this.members = members;
          return members;
        })
      ); 
    }

  /**
   * A GET method
   * @param id id to fetch
   * @returns returns Observable userDto 
   */
  getMemberByGuId(guid: string): Observable<userDto>{ 
    //state management. When the member is in members array then return from there other wise get from api
    if (this.members.length > 0) {
      const member = this.members.find(x => x.guId === guid);
      if (member !== undefined) {
        return of(member);
      }
    }

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
    //state management. When the member is in members array then return from there other wise get from api
    if (this.members.length > 0) {
      const member = this.members.find(x => x.id === id);
      if (member !== undefined) {
        return of(member);
      }
    }

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
    //state management. When the member is in members array then return from there other wise get from api
    if (this.members.length > 0) {
      const member = this.members.find(x => x.userName === userName);
      if (member !== undefined) {
        return of(member);
      }
    }

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
    
  /**
   * A PUT method
   * @param member member to update. 
   * Only updating introduction, lookingFor, interests, city, country so still can send the full member model
   * @returns returns Observable noContent 204 
   */
  updateMember(member: userDto) {
    let url = this.apiUrlService.userUpdate;
    if (environment.displayConsoleLog) console.log(`User update url: ${url}`);
    return this.httpClientService.put(url, member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  /**
 * A PUT method to make the photo main
 * @param photoId photo id to make active 
 * 
 * @returns returns nocontent 
 */
  setMainPhoto(photoId: number) {
    let url = this.apiUrlService.userSetPhotoMain;
    url = url.replace(this.apiUrlService.userPhotoIdReplace, photoId.toString());
    if (environment.displayConsoleLog) console.log(`setPhotoActive url: ${url}`);
    //pass an empty object since it is a put request
    return this.httpClientService.put(url, {});
  }

  /**
 * A DELETE method to delete the photo
 * @param photoId photo id to delete 
 * 
 * @returns returns Ok
 */
  deletePhoto(photoId: number) {
    let url = this.apiUrlService.userDeletePhoto;
    url = url.replace(this.apiUrlService.userPhotoIdReplace, photoId.toString());
    if (environment.displayConsoleLog) console.log(`deletePhoto url: ${url}`);
    return this.httpClientService.delete(url);
  }

}
