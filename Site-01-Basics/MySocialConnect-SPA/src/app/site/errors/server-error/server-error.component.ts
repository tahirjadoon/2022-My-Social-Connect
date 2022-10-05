import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {

  //gets error as navigation extras from the error interceptor
  error: any;

  constructor(private router: Router) { 
    const navigation = this.router.getCurrentNavigation();
    //doesnt work in ng13, either use as below or suppress the error
    //see: https://stackoverflow.com/questions/70106472/property-fname-comes-from-an-index-signature-so-it-must-be-accessed-with-fn
    //this.error = navigation?.extras?.state?.error; 
    this.error = navigation?.extras?.state?.['error'];
  }

  ngOnInit(): void {
  }

}
