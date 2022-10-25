import { Component, ElementRef, Input, OnDestroy, OnInit, Renderer2, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { AccountService } from '../../core/services/account.service';
import { ErrorMessageService } from '../../core/services/error-message.service';

import { LoginDto } from '../../core/models/loginDto.model';
import { UserTokenDto } from '../../core/models/userTokenDto.model';

import { environment } from '../../../environments/environment';
import { zRoles } from 'src/app/core/enums/zRoles';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit, OnDestroy {

  //getting passed in from app.component.html
  @Input() title = '';

  //note use of ! or will see a compiler error
  @ViewChild('navbarCollapse') navbarCollapseElement!: ElementRef;

  loginInfo: LoginDto = <LoginDto>{};
  userInfo: UserTokenDto = <UserTokenDto>{};

  error: string = "";
  isLoggedIn: boolean = false;
  private isExecutingLogin: boolean = false;

  //note use of ! or will see a compiler error
  loginSubscription!: Subscription;
  loginAlternateSubscription!: Subscription;
  currentUserSubscription!: Subscription;

  //to be used for the appHasRole directive with Admin link
  zRoles = zRoles;

  constructor(private accountService: AccountService, private errorMsgService: ErrorMessageService,
    private el: ElementRef, private renderer: Renderer2,
    private router: Router, private toastrService: ToastrService ) { }

  ngOnInit(): void {
    //subscribe to the observable being fired from the account service
    this.getCurrentUser();
  }

  ngOnDestroy(): void {
    //unsubscribe
    if (this.loginSubscription) this.loginSubscription.unsubscribe();
    if (this.loginAlternateSubscription) this.loginAlternateSubscription.unsubscribe();
    if (this.currentUserSubscription) this.currentUserSubscription.unsubscribe();
  }

  onLogin() {
    this.isExecutingLogin = true;
    if(environment.displayConsoleLog) console.log(this.loginInfo);

    this.loginSubscription = this.accountService.login(this.loginInfo).subscribe({
      next: r => {
        this.setUser(r, "onLogin");
        //clear login
        this.loginInfo = <LoginDto>{};
        //hide navbar in mobile mode
        this.onNavBarItemClickCloseNavBar();
        //redirect to the /members/list
        this.router.navigateByUrl('/members/list');
      }, error: e => {
        //after the error interceptor the error is being displayed by toastr by the intercepter
        //this.displayError(e, "onLogin");
      }, complete: () => {
        //do something on complete
        this.isExecutingLogin = false;
      }
    });


    //alternate way
    /*
    this.loginAlternateSubscription = this.accountService.login(this.loginInfo).subscribe(response => {
      if (environment.displayConsoleLog) console.log(response);
      this.isLoggedIn = true;
    }, error => {
      if (environment.displayConsoleLog) console.log(error);
    });
    */
  }

  onLogout() {
    //logout, due to persistence remove the user from the local storage as well
    this.accountService.logout();

    //hide navbar in mobile mode
    this.onNavBarItemClickCloseNavBar();

    //flag
    this.isLoggedIn = false;
    //redirect the home page
    this.router.navigateByUrl('/');
  }

  //hide the navbar in mobile mode after an action has been performed
  onNavBarItemClickCloseNavBar() {
    //this.el.nativeElement.querySelector('.navbar-ex1-collapse')  get the DOM
    //this.renderer.setElementClass('DOM-Element', 'css-class-you-want-to-add', false) if 3rd value is true
    //it will add the css class. 'in' class is responsible for showing the menu.
    //this.renderer.setElementClass(this.el.nativeElement.querySelector('.navbar-ex1-collapse'), 'in', false);

    //renderer2 method
    /*
    var navBarCollapseTargetItem = this.el.nativeElement.querySelector('#navbarCollapse');
    if (environment.displayConsoleLog) console.log(navBarCollapseTargetItem);
    this.renderer.removeClass(navBarCollapseTargetItem, 'show');
    */

    //view child method
    const classToRemove = "show";
    if (this.navbarCollapseElement && this.navbarCollapseElement.nativeElement.classList.contains(classToRemove))
      this.navbarCollapseElement.nativeElement.classList.remove(classToRemove);
  }

  getCurrentUser() {
    //subscribe to the observable being fired from the account service
    this.currentUserSubscription = this.accountService.currentUser$.subscribe({
      next: user => {
        //getting fired on login as well but that is handled above in login so do not set the user again
        if (this.isExecutingLogin) return;
        this.setUser(user, "getCurrentUser");
      }, error: e => {
        this.displayError(e, "getCurrentUser");
      }, complete: () => {
        //do something on complete
      }
    });
  }

  setUser(user: UserTokenDto, from: string) {
    this.userInfo = user;
    if (environment.displayConsoleLog && this.userInfo) {
      console.log(`setUser-${from} UserName: ${this.userInfo?.userName}`);
      console.log(`setUser-${from} Token: ${this.userInfo?.token}`);
    }
    this.isLoggedIn = !!user;
  }

  displayError(error: any, from: string) {
    this.error = this.errorMsgService.getHttpErrorMessage(error);
    if(environment.displayConsoleLog) console.log(`displayError-${from} Error: ${this.error}`);
    //alert(`displayError-${from} Error: ${this.error}`);
    this.toastrService.error(this.error);
  }

}
