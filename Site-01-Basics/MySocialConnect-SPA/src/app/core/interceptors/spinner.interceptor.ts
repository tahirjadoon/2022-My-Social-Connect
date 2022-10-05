import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { delay, finalize, Observable } from 'rxjs';

/*
calls the spinner service
root component has the spinner html
add the styles to the angular.json
*/

import { SpinnerService } from '../services/spinner.service';

@Injectable()
export class SpinnerInterceptor implements HttpInterceptor {

  constructor(private spinnerService: SpinnerService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    //call the spinner when the request is made
    this.spinnerService.show();

    //when the request is complete then hide it so tap into next
    return next.handle(request).pipe(
      //fake delay for showing the spinner. real work application no delay is needed
      delay(1000), //one second 
      finalize(() => {
        this.spinnerService.hide();
      })
    );
  }
}
