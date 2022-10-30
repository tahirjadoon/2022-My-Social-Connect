import { Component, OnInit } from '@angular/core';

import { environment } from '../environments/environment';
import { UserTokenDto } from './core/models/userTokenDto.model';

import { AccountService } from './core/services/account.service';
import { LocalStorageService } from './core/services/local-storage.service';
import { PresenceHubService } from './core/services/signalr/presence-hub.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = '';
  webApiUrl: string = "";
  /*
  //users commented code
  //for subscripton first method
  users1: any;
  error1: string = "";
  complete1: string = "";

  //for subscripton second method
  users2: any;
  error2: string = "";
  complete2: string = "";

  constructor(private http: HttpClient){}
  */
  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.title = environment.title;
    this.webApiUrl = environment.usebaseUrlHttps ? environment.webApiBaseUrlHttps : environment.webApiBaseUrlHttp;
    //get the user from local storage if available and set it. This is persistence
    this.setCurrentUser();

    /*
    //users commencted code
    //preferred way of doing the subscription
    this.getUsers();

    //second and old way of doing subscripton
    this.getUsers2();
    */
  }

  /*
  //users commented code
  getUsers(){
    this.http.get(`${this.webApiUrl}api/users`).subscribe({
      next: r => {
        this.users1 = r;
      }, error: e => {
        this.error1 = e;
        console.log("Error1:"+e);
      }, complete: () => {
        this.complete1 = "Request 1 Complete";
      }
    });
  }

  getUsers2(){
    this.http.get(`${this.webApiUrl}api/users`).subscribe(response => {
      this.users2 = response;
    }, error => {
      this.error2 = error;
      console.log("Error2:"+error);
    }, () => {
      this.complete2 = "Request 2 Completed"
    });
  }
  */

  //on website load get the user and fire it. The subscribers then will pick it, nav bar component in this case
  setCurrentUser() {
    this.accountService.getAndFireCurrentUser();
  }
}
