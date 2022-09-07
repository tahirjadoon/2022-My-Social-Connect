import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { NavComponent } from './site/nav/nav.component';
import { TestErrorsComponent } from './site/errors/test-errors/test-errors.component';
import { NotFoundComponent } from './site/errors/not-found/not-found.component';

import { SharedModule } from './core/modules/shared.module';

import { ErrorInterceptor } from './core/interceptors/error.interceptor';
import { ServerErrorComponent } from './site/errors/server-error/server-error.component';

//common items moved to shared module
@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    TestErrorsComponent,
    NotFoundComponent,
    ServerErrorComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
