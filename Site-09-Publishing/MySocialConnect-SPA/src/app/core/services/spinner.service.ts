import { Injectable } from '@angular/core';

import { NgxSpinnerService } from 'ngx-spinner';

/*
spinner interceptor is calling this service
root component has the spinner html
add the styles to the angular.json
*/

@Injectable({
  providedIn: 'root'
})
export class SpinnerService {
  busyRequestCount: number = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  //there are multiple type of spinner avaialble, check https://www.npmjs.com/package/ngx-spinner 
  show() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'line-scale-party',
      bdColor: 'rgba(255,255,255,0)',
      color: '#333333'
    });
  }

  hide() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.spinnerService.hide();
    }
  }
}
