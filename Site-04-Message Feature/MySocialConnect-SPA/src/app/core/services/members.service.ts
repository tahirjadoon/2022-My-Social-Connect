import { Injectable } from '@angular/core';
import { map, Observable, of, take } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { ApiUrlService } from './api-url.service';
import { HttpClientService } from './http-client.service';
import { LocalStorageService } from './local-storage.service';
import { AccountService } from './account.service';

import { AppConstants } from '../constants/app-constants';

import { environment } from '../../../environments/environment';

import { UserTokenDto } from '../models/userTokenDto.model';
import { userDto } from '../models/userDto.model';
import { LikeDto } from '../models/likeDto';
import { PaginatedResult } from '../models/helpers/paginated-result.model';
import { UserParams } from '../models/helpers/user-params.model';
import { LikeParams } from '../models/helpers/like-params.model';

import { zMemberGetBy } from '../enums/zMemberGetBy';
import { zUserLikeType } from '../enums/zUserLikeType';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  //to pass the auth token to the api, later will use interceptor
  private httpOptions;

  //state mangement. We dont want to pull the user info every time
  //little tricky since page size, filtering and page number are at play 
  private members: userDto[] = []; //not used any more
  private memberCache = new Map();

  private userParams!: UserParams;
  private user!: UserTokenDto;

  constructor(private apiUrlService: ApiUrlService,
      private httpClientService: HttpClientService,
      private localStorageService: LocalStorageService,
      private http: HttpClient,
      private accountService: AccountService) {
    //to pass the auth token to the api, later will user interceptor
    //not using this any more, using the interceptor
    this.httpOptions = {
      headers: new HttpHeaders({
        Authorization: AppConstants.Bearer + localStorageService.getLoggedinToken
      })
    };

    this.retrieveUserInfo();
  }

  private retrieveUserInfo() {
    //get the logged in user
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user: UserTokenDto) => {
        this.user = user;
        this.resetUserParams();
      },
      error: e => { },
      complete: () => { }
    });
  }

  resetUserParams() : UserParams {
    //filtering information is un userParams, check the class for more details. 
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getUserParams() : UserParams {
    return this.userParams;
  }

  setUserParams(user_params: UserParams) {
    this.userParams = user_params;
  }

  getUser() : UserTokenDto {
    return this.user;  
  }
  
  /**
   * A GET method
   * @returns returns Observable userDto[] 
   */
  /*
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
    
    //return this.httpClientService
    //  .get<userDto[]>(url, this.httpOptions);
    
    return this.httpClientService.get<userDto[]>(url).pipe(
        map(members => {
          //set the members for state management and return as well  
          this.members = members;
          return members;
        })
      ); 
  }
  */
  getMembers(user_params: UserParams) {
    //we'll use the key using the full param object properties we have in place for searching
    const cacheKey = Object.values(user_params).join('-');
    if (environment.displayConsoleLog) console.log(`MemberKey: ${cacheKey}`);

    let cacheReuslt = this.memberCache.get(cacheKey);
    if (cacheReuslt)
      return of(cacheReuslt);
    
    //add the pagination and filtering parameters
    const params = user_params.getMemberSearchParams();
    //url
    const url = this.apiUrlService.usersAll;
    if (environment.displayConsoleLog) console.log(`Users allUrl: ${url} params: ${params}`);

    //users end point is protected by authentication so need to send the token as well 
    //check jwt interceptor for details
    
    return this.getPaginatedResult<userDto[]>(url, params).pipe(
      map(response => {
        //set the cache
        this.memberCache.set(cacheKey, response);
        return response;
      })
    );  
  }

  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    
    return this.httpClientService.getWithFullResponse<T>(url, params).pipe(
      map(response => {
        paginatedResult.result = response.body!;
        const paginationHeader = response.headers.get(AppConstants.PaginationHeader);
        if (paginationHeader !== null) {
          paginatedResult.pagination = JSON.parse(paginationHeader);
        }
        return paginatedResult;
      })
    );
  }

  /**
   * A GET method
   * @param guid guid to fetch
   * @returns returns Observable userDto 
   */
  getMemberByGuId(guid: string): Observable<userDto>{ 
    //state management. When the member is in members array then return from there other wise get from api
    const member = this.getMemberCache(guid, zMemberGetBy.guid);//this.getMember(guid, zMemberGetBy.guid);
    if (member !== undefined) return of(member);
    
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
    const member = this.getMemberCache(id, zMemberGetBy.id);//this.getMember(id, zMemberGetBy.id);
    if (member !== undefined) return of(member);

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
    const member = this.getMemberCache(userName, zMemberGetBy.userName);//this.getMember(userName, zMemberGetBy.userName);
    if (member !== undefined) return of(member);

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
  
  private getMember(key: any, by: zMemberGetBy): userDto | undefined {
    let member: userDto | undefined;

    if (!by || !key || this.members.length <= 0) return member;

    switch (by as zMemberGetBy) {
      case zMemberGetBy.id: 
        member =  this.members.find(x => x.id === +key);
        break;
      case zMemberGetBy.userName:
        member =  this.members.find(x => x.userName === key);
        break;
      case zMemberGetBy.guid: 
      member =  this.members.find(x => x.guId === key);
        break; 
    }
    return member;
  }

  private getMemberCache(key: any, by: zMemberGetBy): userDto | undefined{
    let member: userDto | undefined;
    if (!key || !by) return member;
    
    //using map and reduce to create a single array out of key value array pairs
    const members = [...this.memberCache.values()].reduce((arr, elem) => arr.concat(elem.result), []);
    if (environment.displayConsoleLog) {
      console.log("******************")
      console.log(members);
      console.log("******************")  
    }
    if (members.length <= 0) return member;

    switch (by as zMemberGetBy) {
      case zMemberGetBy.id:
        member = members.find((x: userDto) => x.id === +key);
        break;
      case zMemberGetBy.userName:
        member = members.find((x: userDto) => x.userName === key);
        break;
      case zMemberGetBy.guid:
        member = members.find((x: userDto) => x.guId === key);
        break;
    }

    return member;
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

  /**
   * A add method to add likes
   * @param id id of the user getting liked 
   * 
   * @returns returns Ok
   */
  addLike(id: number) : Observable<any> {
    var url = this.apiUrlService.likeAdd
      .replace(this.apiUrlService.likeUserIdReplace, id.toString())
      .replace(this.apiUrlService.likeUserNameReplace, this.user.userName);
    if (environment.displayConsoleLog) console.log(`addLike: ${url}`);
    return this.httpClientService.post(url, {});
  }

  //not using likeDto instead using partial userDto since the properties are the same
  getLikes(like_parms: LikeParams) {
    //add the pagination and filtering parameters
    const params = like_parms.getLikesSearchParams();
    var url = this.apiUrlService.likesForUser;
    if (environment.displayConsoleLog) console.log(`getLikes url: ${url} params: ${params}`);

    return this.getPaginatedResult<Partial<userDto[]>>(url, params);
  }

}