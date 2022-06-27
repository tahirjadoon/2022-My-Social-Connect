import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'My Social Connect The Dating App';
  webApiUrl: string = "";

  //for subscripton first method
  users1: any;
  error1: string = "";
  complete1: string = "";

  //for subscripton second method
  users2: any;
  error2: string = "";
  complete2: string = "";

  constructor(private http: HttpClient){}

  ngOnInit() {
    this.webApiUrl = environment.usebaseUrlHttps ? environment.webApiBaseUrlHttps : environment.webApiBaseUrlHttp;

    //preferred way of doing the subscription
    this.getUsers();

    //second and old way of doing subscripton
    this.getUsers2();
  }

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
}
