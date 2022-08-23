import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { ErrorMessageService } from '../../core/services/error-message.service';
import { AccountService } from '../../core/services/account.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-site-home',
  templateUrl: './site-home.component.html',
  styleUrls: ['./site-home.component.css']
})
export class SiteHomeComponent implements OnInit, OnDestroy {
  //getting passed in from app.component.html
  @Input() title = '';
  registerMode: boolean = false;

  isLoggedIn: boolean = false;
  currentUserSubscription!: Subscription;
  
  constructor(private accountService: AccountService, private errorMsgService: ErrorMessageService) { }

  ngOnInit(): void {
    this.getCurrentUser();
  }

  ngOnDestroy(): void {
    if (this.currentUserSubscription) this.currentUserSubscription.unsubscribe();
  }

  onRegisterToggle() {
    this.registerMode = !this.registerMode;
  }

  onCancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }

  //when the user is logged in then not displaying the register form 
  getCurrentUser() {
    //subscribe to the observable being fired from the account service
    this.currentUserSubscription = this.accountService.currentUser$.subscribe({
      next: user => {
        this.isLoggedIn = !!user;
      }, error: e => {
        this.displayError(e, "getCurrentUser SiteHome");
      }, complete: () => {
        //do something on complete
      }
    });
  }

  displayError(error: any, from: string) {
    const errorMsg = this.errorMsgService.getHttpErrorMessage(error);
    if(environment.displayConsoleLog) console.log(`displayError-${from} Error: ${errorMsg}`);
    alert(`displayError-${from} Error: ${errorMsg}`);
  }

}
