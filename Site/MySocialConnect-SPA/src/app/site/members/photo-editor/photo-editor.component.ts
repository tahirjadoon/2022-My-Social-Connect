import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription, take } from 'rxjs';
import { FileUploader } from 'ng2-file-upload';
import { ToastrService } from 'ngx-toastr';

import { AppConstants } from '../../../core/constants/app-constants';

import { ApiUrlService } from '../../../core/services/api-url.service';
import { AccountService } from '../../../core/services/account.service';
import { MembersService } from '../../../core/services/members.service';

import { environment } from '../../../../environments/environment';

import { userDto } from '../../../core/models/userDto.model';
import { UserTokenDto } from '../../../core/models/userTokenDto.model';
import { PhotoDto } from '../../../core/models/photoDto.model';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit, OnDestroy {
  @Input() member!: userDto ;

  //bg2-file-uplaod 
  uploader!: FileUploader;
  hasBaseDropZoneOver = false;

  currentUser!: UserTokenDto;

  setPhotoActiveSubscription!: Subscription;
  deletePhotoSubscriptoion!: Subscription;

  constructor(private apiUrlService: ApiUrlService, private accountService: AccountService, private toastrService: ToastrService, private memberService: MembersService) { 
    //no need to unsubscribe since we are taking 1 only
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.currentUser = user);
  }

  ngOnInit(): void {
    this.initilizeUploader();
  }

  ngOnDestroy(): void {
    //unsubscribe
    if (this.setPhotoActiveSubscription) this.setPhotoActiveSubscription.unsubscribe();
    if (this.deletePhotoSubscriptoion) this.deletePhotoSubscriptoion.unsubscribe();
  }

  formatBytes(bytes:number, decimals?: number) {
    if (bytes == 0) return '0 Bytes';
    const k = 1024,
      dm = decimals || 2,
      sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'],
      i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }

  //to set the drop zone, name must be exact
  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  initilizeUploader() {
    if (environment.displayConsoleLog) {
      console.log("*******PhotoEditor Start***********")
      console.log(`AddPhots Url: ${this.apiUrlService.userAddPhoto}`);
      console.log(`authtoken: ${AppConstants.Bearer}${this.currentUser.token}`);
      console.log("*******PhotoEditor End***********")
    }

    const maxFileSize = 10 * 1024 * 1024; //10 mb image only

    //set up. provide url, authToken
    this.uploader = new FileUploader({
      url: this.apiUrlService.userAddPhoto,
      authToken: `${AppConstants.Bearer}${this.currentUser.token}`,
      isHTML5: true,
      allowedFileType: ['image'], //jpeg, png, gif etc are allowed
      removeAfterUpload: true, //remove the file from drop zone
      autoUpload: false, //user will need to click the button 
      maxFileSize: maxFileSize, 
    });

    //when the file is not added display error message
    this.uploader.onWhenAddingFileFailed = (item, filter) => {
      switch (filter.name) {
        case 'fileSize': {
          const fileSizeError = `${item.name} is ${this.formatBytes(item.size)}. Max allowed is ${this.formatBytes(maxFileSize)}`;
          this.toastrService.error(fileSizeError, `Error: ${filter.name}`);
          break;
        }
        default: {
          this.toastrService.error(`Error trying to upload file ${item.name}`, `Error: ${filter.name}`);
          break;
        }
      }
    }

    //we are using bearer token. Otherwise will need to adjust the cors configuration and allow credentials to go with file
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }

    //push the photo to the members.photos array after successful upload
    //it is making the users/add/photo call which is returning the added photo. Photo is in the response
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = <PhotoDto>JSON.parse(response);
        this.member.photos.push(photo);
      }
    }
  }

  onSetMainPhoto(photo: PhotoDto) {
    this.setPhotoActiveSubscription = this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        //make the new photo current to show for the nav button
        this.currentUser.mainPhotoUrl = photo.url;
        //update the local storage and fire it so that the left nav can update the image
        this.accountService.setAndFireCurrentUser(this.currentUser);
        //update member
        this.member.photoUrl = photo.url;
        //go through member photos and mark the current one as not main and mark the new as main
        this.member.photos.forEach(p => {
          //make the current photo as true, the rest as false
            p.isMain = (p.id === photo.id); 
        });
      },
      error: e => {},
      complete: () => {}
    });
  }

  onDeletePhoto(photo: PhotoDto) {
    this.deletePhotoSubscriptoion = this.memberService.deletePhoto(photo.id).subscribe({
      next: () => {
        //remove the photo from the members 
        this.member.photos = this.member.photos.filter(x => x.id !== photo.id);
      },
      error: e => { },
      complete: () => {}
    });
  }

}
