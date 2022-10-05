import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { AbstractControlOptions, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { SiteRegisterDto } from '../../core/models/siteRegisterDto.model';
import { environment } from '../../../environments/environment';

import { AccountService } from '../../core/services/account.service';
import { ErrorMessageService } from '../../core/services/error-message.service';

import { MustMatchValidator } from '../../core/validators/must-match.validator';
import { PasswordStrengthValidator } from '../../core/validators/password-strength.validator';
import { OnlyCharValidator } from '../../core/validators/only-char.validator';

@Component({
  selector: 'app-site-register',
  templateUrl: './site-register.component.html',
  styleUrls: ['./site-register.component.css']
})
export class SiteRegisterComponent implements OnInit, OnDestroy {

  //to tell the site home to hide the register form since cancel has been clicked
  @Output() cancelRegister = new EventEmitter();

  //reactive form
  registerForm!: FormGroup;
  isReactiveForm: boolean = true;

  siteRegister: SiteRegisterDto = <SiteRegisterDto>{};

  //note use of ! or will see a compiler error
  registerSubscription!: Subscription;
  registerReactiveSubscription!: Subscription;

  //after implementation on error interceptor
  validationErrors: string[] = [];

  //must be minimum 18 years old
  maxDate!: Date;

  //gender list
  genderList = [
    { id: 'female', value: 'female', label: 'Female' },
    { id: 'male', value: 'male', label: 'Male' },
  ];

  constructor(private accountService: AccountService,
    private errorMsgService: ErrorMessageService,
    private toastrService: ToastrService,
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    //reactive form
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  ngOnDestroy(): void {
    if (this.registerSubscription) this.registerSubscription.unsubscribe();
    if (this.registerReactiveSubscription) this.registerReactiveSubscription.unsubscribe();
  }

  //reactive form
  initializeForm() {
    //we have a formGroup and formFroup has formControl
    /*
    this.registerForm = new FormGroup({
      userName: new FormControl('', [Validators.required, Validators.minLength(5), OnlyCharValidator.onlyChar()]),
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8), PasswordStrengthValidator.passwordStrength()]),
      confirmPassword: new FormControl('', Validators.required)
      },
      MustMatchValidator.mustMatch("password", "confirmPassword")
    );
    */
    this.registerForm = this.fb.group({
      gender: ['male', Validators.required],
      displayName: ['', [Validators.required, Validators.minLength(5)]],
      dateOfBirth: ['', Validators.required],
      city: ['', [Validators.required, OnlyCharValidator.onlyChar()]],
      country: ['', [Validators.required, OnlyCharValidator.onlyChar()]],
      userName: ['', [Validators.required, Validators.minLength(5), OnlyCharValidator.onlyChar()]],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8), PasswordStrengthValidator.passwordStrength()]],
      confirmPassword: ['', Validators.required]
      },
      {validators: [MustMatchValidator.mustMatch("password", "confirmPassword")]} as AbstractControlOptions
    );
    console.log(this.registerForm.controls);
  }

  getErrorList(errorObject: any) {
    if (!errorObject)
      return [];
    return Object.keys(errorObject);
  }

  //convenience getter for easy access to form fields
  get rf() {
    return this.registerForm.controls;
  }

  rf2(key: string) {
    return this.registerForm.get(key) as FormControl;
  }

  get registerFormUsernameControl(): FormControl {
    return this.registerForm.get('userName') as FormControl;
    //return this.rf['userName'] as FormControl;
  }
 
  get registerFormPasswordControl(): FormControl {
    return this.registerForm.get('password') as FormControl;
    //return this.rf['password'] as FormControl;
  }
 
  get registerFormConfirmPasswordControl(): FormControl {
    return this.registerForm.get('confirmPassword') as FormControl;
    //return this.rf['confirmPassword'] as FormControl;
  }

  //per template driven form
  onRegisterTemplateDriven() {
    this.validationErrors = [];//reset
    this.doRegisteration(this.siteRegister);
  }
  
  //show erros on submit
  private showErrorsOnSubmit() {
    Object.keys(this.registerForm.controls).forEach(field => {
      const control = this.registerForm.get(field);
      if (control?.errors)
        control.markAsTouched({onlySelf: true});
    });
  }

  //check reactive form 
  private isReactiveFormGood(): boolean {
    if (environment.displayConsoleLog) console.log(this.registerForm.value);

    if (this.registerForm.invalid) {
      this.showErrorsOnSubmit();
      this.toastrService.error("Please fix errors and try again", "Validation Error(s)")
      return false;
    }

    return true;
  }

  //per reactive form
  onRegisterReactiveForm() {
    this.validationErrors = [];//reset
    //when invalid do not proceed further
    if (!this.isReactiveFormGood()) return;
    //convert to siteRegister
    const registerUser = new SiteRegisterDto(this.registerForm.value['userName'],
                                            this.registerForm.value['password'],
                                            this.registerForm.value['gender'],
                                            this.registerForm.value['displayName'],
                                            this.registerForm.value['dateOfBirth'],
                                            this.registerForm.value['city'],
                                            this.registerForm.value['country']);
    this.doRegisteration(registerUser)
    
  }

  private doRegisteration(registerUser: SiteRegisterDto) {
    if (environment.displayConsoleLog) console.log(registerUser);
    //register
    this.registerSubscription = this.accountService.register(registerUser).subscribe({
      next: r => {
        if (environment.displayConsoleLog) {
          console.log("RegisterUserBack: ");
          console.log(r);
        }
        //go to the members page
        this.router.navigateByUrl('/members/list');
      }, error: e => {
        //due to error intercepter we are getting a flat array of validation items so for modal validation need to check that
        //check array and length > 0
        //other cases the error interceptor is displaying the error
        if (e?.length) {
          if (environment.displayConsoleLog) console.log("***inside model validation errors***");
          this.validationErrors = e;
        }
        if (environment.displayConsoleLog) console.log(e);
        //this.displayError(e, "Registeration");
      }, complete: () => {

      }
    });
  }

  private displayError(error: any, from: string) {
    const errormsg = this.errorMsgService.getHttpErrorMessage(error);
    if(environment.displayConsoleLog) console.log(`displayError-${from} Error: ${errormsg}`);
    //alert(`displayError-${from} Error: ${errormsg}`);
    this.toastrService.error(errormsg);
  }

  onCancel() {
    this.validationErrors = [];//reset
    if (environment.displayConsoleLog) console.log('cancelled');
    this.cancelRegister.emit(false);
    //reset form
    this.siteRegister = <SiteRegisterDto>{};
  }

}
