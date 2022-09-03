import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { SiteRegisterDto } from '../../core/models/siteRegisterDto.model';
import { environment } from '../../../environments/environment';

import { AccountService } from '../../core/services/account.service';
import { ErrorMessageService } from '../../core/services/error-message.service';

@Component({
  selector: 'app-site-register',
  templateUrl: './site-register.component.html',
  styleUrls: ['./site-register.component.css']
})
export class SiteRegisterComponent implements OnInit, OnDestroy {

  //to tell the site home to hide the register form since cancel has been clicked
  @Output() cancelRegister = new EventEmitter();

  siteRegister: SiteRegisterDto = <SiteRegisterDto>{};

  //note use of ! or will see a compiler error
  registerSubscription!: Subscription;

  constructor(private accountService: AccountService, private errorMsgService: ErrorMessageService, private toastrService: ToastrService) { }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    if (this.registerSubscription) this.registerSubscription.unsubscribe();
  }

  onRegister() {
    if (environment.displayConsoleLog) console.log(this.siteRegister);
    this.registerSubscription = this.accountService.register(this.siteRegister).subscribe({
      next: r => {
        if (environment.displayConsoleLog) {
          console.log("RegisterUserBack: ");
          console.log(r);
          //cancel the form
          this.onCancel();
        }
      }, error: e => {
        this.displayError(e, "Registeration");
      }, complete: () => {

      }
    });
  }

  displayError(error: any, from: string) {
    const errormsg = this.errorMsgService.getHttpErrorMessage(error);
    if(environment.displayConsoleLog) console.log(`displayError-${from} Error: ${errormsg}`);
    //alert(`displayError-${from} Error: ${errormsg}`);
    this.toastrService.error(errormsg);
  }

  onCancel() {
    if (environment.displayConsoleLog) console.log('cancelled');
    this.cancelRegister.emit(false);
    //reset form
    this.siteRegister = <SiteRegisterDto>{};
  }

}
