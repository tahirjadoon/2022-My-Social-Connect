import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { environment } from '../../../environments/environment';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastrService: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      //error is the http response that we see when writing the console.log
      catchError(error => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                //400 validation errors, it has error.error.errors array, start
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key])
                    modalStateErrors.push(error.error.errors[key]);
                }
                //modalStateErrors so that the errors could be displayed near the form 
                throw modalStateErrors.flat();
                //400 validation errors, end
              }
              else {
                //400 all other errors, start
                //will use the toastr service to display the error
                this.toastrService.error(`${error.statusText}: ${error.error}`, error.status);
                //400 all other errors, end
              }
              break;
            case 401:
              //will use the toastr service to display the error
              this.toastrService.error(`${error.statusText}: ${error.error}`, error.status);
              break;
            case 404:
              //redirect to the not found page
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              //the error object. It has the properties message and details that you would need to tap into
              const navigationExtras: NavigationExtras = { state: { error: error.error } };
              //redirect to the server error page
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastrService.error('Something unexpected went wrong');
              if(environment.displayConsoleLog) console.log(error);
              break;
          }
        }
        //return the error
        return throwError(error);
      })
    );
  }
}
