import { JsonPipe } from '@angular/common';
import { Injectable } from '@angular/core';

import { UserTokenDto } from '../models/userTokenDto.model';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  
  //setup the keys for different items
  public _keyUser: string = "MySocialConnectUser";

  constructor() { }

  
  getStorage():any[] {
    let s = [];
    for (let i = 0; i < localStorage.length; i++){
      const key: string = localStorage.key(i)!;
      if (key) {
        const value: string = localStorage.getItem(key)!;
        s.push({ key: key, value: value });
      }
    }
    return s;
  }

  getItem(key: string) {
    return JSON.parse(localStorage.getItem(key)!);
  }
  
  setItem(key: string, data: any) {
    localStorage.setItem(key, JSON.stringify(data));
  }

  removeItem(key: string) {
    localStorage.removeItem(key);
  }

  //public properties to get some common pieces
  getLoggedInUser: UserTokenDto = this.getItem(this._keyUser);
  getLoggedinToken: string = this.getLoggedInUser?.token;
  getloggedinUserName: string = this.getLoggedInUser?.userName;
  getLoggedinUserGuid: string = this.getLoggedInUser?.guId;
  
}
