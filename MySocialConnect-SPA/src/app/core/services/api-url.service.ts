import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiUrlService {

  //get the api base url from the environment. This url ends with a / 
  private baseUrl: string = environment.usebaseUrlHttps ? environment.webApiBaseUrlHttps : environment.webApiBaseUrlHttp;

  //add api key word to the base url and also add ending / to it
  private apiBaseUrl: string = `${this.baseUrl}api/`;

  //add controllers to the base url
  private accountBaseUrl = `${this.apiBaseUrl}account/`;

  constructor() {

    if (environment.displayConsoleLog) {
      console.log(`ApiUrlService baseUrl: ${this.baseUrl}`);
      console.log(`ApiUrlService apiBaseUrl: ${this.apiBaseUrl}`);  
    }
    
  }

  //accounts controller urls
  accountRegisterUser: string = `${this.accountBaseUrl}register`;
  accountLogin: string = `${this.accountBaseUrl}login`;
}
