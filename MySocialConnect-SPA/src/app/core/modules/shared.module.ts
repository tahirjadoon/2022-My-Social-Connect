import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';

import { SiteHomeComponent } from '../../site/site-home/site-home.component';
import { SiteRegisterComponent } from '../../site/site-register/site-register.component';
import { MemberListComponent } from '../../site/members/member-list/member-list.component';
import { MemberDetailComponent } from '../../site/members/member-detail/member-detail.component';
import { ListsComponent } from '../../site/lists/lists.component';
import { MessagesComponent } from '../../site/messages/messages.component';

//what ever is imported here, the same should be exported as well
@NgModule({
  declarations: [
    SiteHomeComponent,
    SiteRegisterComponent,
    MemberListComponent,
    MemberDetailComponent,
    ListsComponent,
    MessagesComponent
  ],
  imports: [
    CommonModule, 
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({positionClass: 'toast-bottom-right'})
  ],
  exports: [
    BrowserAnimationsModule,
    HttpClientModule, 
    FormsModule,
    BsDropdownModule,
    ToastrModule
  ]
})
export class SharedModule { }
