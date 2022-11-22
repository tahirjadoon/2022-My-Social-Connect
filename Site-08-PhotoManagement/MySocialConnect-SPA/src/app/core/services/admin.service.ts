import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from 'src/environments/environment';
import { PhotoForApprovalDto } from '../models/photoForApprovalDto';

import { userDto } from '../models/userDto.model';

import { ApiUrlService } from './api-url.service';
import { HttpClientService } from './http-client.service';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private apiUrlService: ApiUrlService, private httpClientService: HttpClientService) { }

  /*
  this pass back an array object of followng type
  {"id":12,"userName":"bobmed","displayName":"Bobmed","guId":"cfad1be7-d0ad-4137-b95b-5d4a5b92991a","roles":["Member"]}
  */
  getUsersWithRoles() {
    var url = this.apiUrlService.adminUsersWithroles;
    if (environment.displayConsoleLog) console.log(url);
    return this.httpClientService.get<Partial<userDto[]>>(url);
  }

  updateUserRoles(userGuid: string, roles: string[]) {
    var url = this.apiUrlService.adminEditRoles.replace(this.apiUrlService.adminRolesGuidReplace, userGuid);
    if (roles && roles.length > 0) url += "?roles=" + roles;
    if (environment.displayConsoleLog) console.log(url);
    return this.httpClientService.post<string[]>(url, {});
  }

  getPhotosForApproval() {
    var url = this.apiUrlService.adminPhotosToModerate;
    if (environment.displayConsoleLog) console.log(url);
    return this.httpClientService.get<PhotoForApprovalDto[]>(url);
  }

  approvePhoto(photoId: number) {
    var url = this.apiUrlService.adminApprovePhoto.replace(this.apiUrlService.adminPhotoIdReplace, photoId.toString());
    if (environment.displayConsoleLog) console.log(url);
    return this.httpClientService.post<any>(url, {});
  }

  rejectPhoto(photoId: number) {
    var url = this.apiUrlService.adminRejectPhoto.replace(this.apiUrlService.adminPhotoIdReplace, photoId.toString());
    if (environment.displayConsoleLog) console.log(url);
    return this.httpClientService.post<any>(url, {});
  }

}
