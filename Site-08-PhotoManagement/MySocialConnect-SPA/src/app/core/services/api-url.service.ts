import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiUrlService {

  //api base url ==> comes with /api/
  private apiBaseUrl: string = environment.usebaseUrlHttps ? environment.webApiBaseUrlHttps : environment.webApiBaseUrlHttp;
  //signalr hub ==> comes with /hub/
  private hubBaseUrl: string = environment.usebaseUrlHttps ? environment.webApiBaseHubsUrlHttps : environment.webApiBaseHubsUrlHttp;

  constructor() {

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

  //message controller urls
  private messagesBaseUrl = `${this.apiBaseUrl}messages/`;
  messageRecipIdReplace = "[recpid]";
  messageDelIdReplace = "[msgid]";
  messageSend = `${this.messagesBaseUrl}send/message`;
  messagesGet = `${this.messagesBaseUrl}user/get/messages`;
  messageThread = `${this.messagesBaseUrl}message/thread/${this.messageRecipIdReplace}`; //replace [recpid] with receipent id
  messageDelete = `${this.messagesBaseUrl}delete/message/${this.messageDelIdReplace}`; //replace [msgid] with the msg id to delete

  //admin controller urls
  private adminBaseUrl = `${this.apiBaseUrl}admin/`;
  adminRolesGuidReplace = "[guid]";
  adminPhotoIdReplace = "[photoId]";
  adminUsersWithroles = `${this.adminBaseUrl}users-with-roles`;
  adminEditRoles = `${this.adminBaseUrl}edit-roles/${this.adminRolesGuidReplace}`; //replace [guid] with user guids and pass in the new roles as params
  adminPhotosToModerate = `${this.adminBaseUrl}photos-to-moderate`;
  adminApprovePhoto = `${this.adminBaseUrl}approve-photo/${this.adminPhotoIdReplace}`; //replace [photoId] with photoId
  adminRejectPhoto = `${this.adminBaseUrl}reject-photo/${this.adminPhotoIdReplace}`; //replace [photoId] with photoId

  //signalr urls
  signalr_presenceUrl = `${this.hubBaseUrl}presence`;
  signalr_messageUrl = `${this.hubBaseUrl}message`;
}
