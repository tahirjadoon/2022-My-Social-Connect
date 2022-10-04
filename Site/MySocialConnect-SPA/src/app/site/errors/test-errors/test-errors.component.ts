import { Component, OnInit } from '@angular/core';

import { userDto } from '../../../core/models/userDto.model';
import { SiteRegisterDto } from '../../../core/models/siteRegisterDto.model';
import { UserTokenDto } from '../../../core/models/userTokenDto.model';

import { environment } from '../../../../environments/environment';

import { ApiUrlService } from '../../../core/services/api-url.service';
import { HttpClientService } from '../../../core/services/http-client.service';


@Component({
  selector: 'app-test-errors',
  templateUrl: './test-errors.component.html',
  styleUrls: ['./test-errors.component.css']
})
export class TestErrorsComponent implements OnInit {

  validationErrors: string[] = [];

  constructor(private apiUrlService: ApiUrlService, private httpClientService: HttpClientService) { }

  ngOnInit(): void {
  }

  getNotFoundError() {
    var url = this.apiUrlService.buggyNotFound;
    if (environment.displayConsoleLog) console.log(`NotFoundURL: ${url}`);

    this.httpClientService.get<userDto>(url).subscribe({
      next: (user: userDto) => {
        if (environment.displayConsoleLog) console.log(user);
      }, error: e => {
        if (environment.displayConsoleLog) console.log(e);
      }, complete: () => {
        if (environment.displayConsoleLog) console.log("NotFoundComplete");
      }
    });

  }

  getBadRequestError() {
    var url = this.apiUrlService.buggyBadRequest;
    if (environment.displayConsoleLog) console.log(`BadRequestURL: ${url}`);

    this.httpClientService.get<string>(url).subscribe({
      next: response => {
        if (environment.displayConsoleLog) console.log(response);
      }, error: e => {
        if (environment.displayConsoleLog) console.log(e);
      }, complete: () => {
        if (environment.displayConsoleLog) console.log("BadRequestComplete");
      }
    });
  }

  getServerError() {
    var url = this.apiUrlService.buggyServerError;
    if (environment.displayConsoleLog) console.log(`ServerErrorURL: ${url}`);

    this.httpClientService.get<string>(url).subscribe({
      next: response => {
        if (environment.displayConsoleLog) console.log(response);
      }, error: e => {
        if (environment.displayConsoleLog) console.log(e);
      }, complete: () => {
        if (environment.displayConsoleLog) console.log("ServerErrorComplete");
      }
    });
  }

  getAuthError(){
    var url = this.apiUrlService.buggyAuth;
    if (environment.displayConsoleLog) console.log(`AuthURL: ${url}`);

    this.httpClientService.get<string>(url).subscribe({
      next: response => {
        if (environment.displayConsoleLog) console.log(response);
      }, error: e => {
        if (environment.displayConsoleLog) console.log(e);
      }, complete: () => {
        if (environment.displayConsoleLog) console.log("AuthComplete");
      }
    });
  }

  getValidationError() {
    const register: SiteRegisterDto = new SiteRegisterDto("", "pas", "", "", new Date(), "", "");
    var url = this.apiUrlService.accountRegisterUser;
    if (environment.displayConsoleLog) {
      console.log(`ValidationURL: ${url}`);
      console.log(`RegisterModel: ${register}`);
    }
    this.httpClientService.post<UserTokenDto>(url, register).subscribe({
      next: (user: UserTokenDto) => {
        if (environment.displayConsoleLog) console.log(user);
      }, error: e => {
        if (environment.displayConsoleLog) console.log(e);
        //put this line only after implementing the error interceptor
        this.validationErrors = e;
      }, complete: () => {
        if (environment.displayConsoleLog) console.log("ValidationComplete");
      }
    });
  }

}
