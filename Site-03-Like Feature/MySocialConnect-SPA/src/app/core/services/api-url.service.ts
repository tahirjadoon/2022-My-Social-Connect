import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiUrlService {
  private rId = "{id}";


  //get the api base url from the environment. This url ends with a / 
  private baseUrl: string = environment.usebaseUrlHttps ? environment.webApiBaseUrlHttps : environment.webApiBaseUrlHttp;

  //add api key word to the base url and also add ending / to it
  private apiBaseUrl: string = `${this.baseUrl}api/`;

  constructor() {

    if (environment.displayConsoleLog) {
      console.log(`ApiUrlService baseUrl: ${this.baseUrl}`);
      console.log(`ApiUrlService apiBaseUrl: ${this.apiBaseUrl}`);  
    }
    
  }

  //accounts controller urls
  private accountBaseUrl = `${this.apiBaseUrl}account/`;
  accountRegisterUser: string = `${this.accountBaseUrl}register`;
  accountLogin: string = `${this.accountBaseUrl}login`;

  //buggy controller urls
  private buggyBaseUrl = `${this.apiBaseUrl}buggy/`;
  buggyAuth: string = `${this.buggyBaseUrl}auth`;
  buggyNotFound: string = `${this.buggyBaseUrl}not-found`;
  buggyServerError: string = `${this.buggyBaseUrl}server-error`;
  buggyBadRequest: string = `${this.buggyBaseUrl}bad-request`;

  //users controller urls
  private usersBaseUrl = `${this.apiBaseUrl}users/`;
  userByIdReplace = "[id]";
  userByNameReplace = "[name]";
  userByGuIdReplace = "[guid]";
  userPhotoIdReplace = "[photoId]";

  userUpdate = `${this.usersBaseUrl}`;
  usersAll = `${this.usersBaseUrl}`;
  userByGuId = `${this.usersBaseUrl}${this.userByGuIdReplace}/guid`; //replace [guid] with the id of the user
  userById = `${this.usersBaseUrl}${this.userByIdReplace}/id`; //replace [id] with the id of the user
  userByName = `${this.usersBaseUrl}${this.userByNameReplace}/name`; //replace [name] with the userName of the user
  userAddPhoto = `${this.usersBaseUrl}add/photo`;
  userSetPhotoMain = `${this.usersBaseUrl}set/photo/${this.userPhotoIdReplace}/main`;//replace [photoId] with the photoId
  userDeletePhoto = `${this.usersBaseUrl}delete/${this.userPhotoIdReplace}/photo`;//replace [photoId] with the photoId
  
  //like constroller urls
  private likesBaseUrl = `${this.apiBaseUrl}likes/`;
  likeUserIdReplace = "[id]";
  likeUserNameReplace = "[name]";
  likesForUser = `${this.likesBaseUrl}user/likes`;
  likeAdd = `${this.likesBaseUrl}${this.likeUserIdReplace}/like/${this.likeUserNameReplace}`; //replace [id] with userid and [name] with logged in user name

}
