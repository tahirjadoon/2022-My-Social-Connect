import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TabsModule } from 'ngx-bootstrap/tabs';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ToastrModule } from 'ngx-toastr';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';

import { SiteHomeComponent } from '../../site/site-home/site-home.component';
import { SiteRegisterComponent } from '../../site/site-register/site-register.component';
import { MemberListComponent } from '../../site/members/member-list/member-list.component';
import { MemberDetailComponent } from '../../site/members/member-detail/member-detail.component';
import { ListsComponent } from '../../site/lists/lists.component';
import { MessagesComponent } from '../../site/messages/messages.component';
import { MemberCardComponent } from '../../site/members/member-card/member-card.component';
import { MemberEditComponent } from '../../site/members/member-edit/member-edit.component';
import { PhotoEditorComponent } from '../../site/members/photo-editor/photo-editor.component';
import { DisplayFormgroupErrorsComponent } from '../../site/errors/display-formgroup-errors/display-formgroup-errors.component';

import { TextInputComponent } from '../../site/form-controls/text-input/text-input.component';

import { AppRoutingModule } from '../../app-routing.module';

import { ValidatorsTransformPipe } from '../pipes/validators-transform.pipe';
import { DateInputComponent } from '../../site/form-controls/date-input/date-input.component';


//what ever is imported here, the same should be exported as well
@NgModule({
  declarations: [
    SiteHomeComponent,
    SiteRegisterComponent,
    MemberListComponent,
    MemberDetailComponent,
    ListsComponent,
    MessagesComponent,
    MemberCardComponent,
    MemberEditComponent,
    PhotoEditorComponent,
    DisplayFormgroupErrorsComponent,
    TextInputComponent,
    DateInputComponent,
    ValidatorsTransformPipe,
  ],
  imports: [
    CommonModule, 
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    ToastrModule.forRoot({ positionClass: 'toast-bottom-right' }),
    TabsModule.forRoot(),
    NgxGalleryModule, 
    NgxSpinnerModule,
    FileUploadModule,
  ],
  exports: [
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule, 
    FormsModule,
    ReactiveFormsModule,
    BsDropdownModule,
    BsDatepickerModule,
    TabsModule,
    ToastrModule,
    NgxGalleryModule,
    NgxSpinnerModule,
    FileUploadModule,
  ]
})
export class SharedModule { }
