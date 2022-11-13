import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TabsModule } from 'ngx-bootstrap/tabs';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ToastrModule } from 'ngx-toastr';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FileUploadModule } from 'ng2-file-upload';
import { TimeagoModule } from 'ngx-timeago';

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
import { MemberMessagesComponent } from '../../site/members/member-messages/member-messages.component';

import { AdminPanelComponent } from '../../site/admin/admin-panel/admin-panel.component';
import { UserManagementComponent } from '../../site/admin/user-management/user-management.component';
import { PhotoManagementComponent } from '../../site/admin/photo-management/photo-management.component';

import { RolesModalComponent } from '../../site/modals/roles-modal/roles-modal.component';
import { ConfirmModalComponent } from '../../site/modals/confirm-modal/confirm-modal.component';

import { TextInputComponent } from '../../site/form-controls/text-input/text-input.component';
import { DateInputComponent } from '../../site/form-controls/date-input/date-input.component';

import { ValidatorsTransformPipe } from '../pipes/validators-transform.pipe';

import { HasRoleDirective } from '../directives/has-role.directive';

import { AppRoutingModule } from '../../app-routing.module';

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
    MemberMessagesComponent,
    AdminPanelComponent,
    UserManagementComponent,
    PhotoManagementComponent,
    DisplayFormgroupErrorsComponent,
    TextInputComponent,
    DateInputComponent,
    RolesModalComponent,
    ValidatorsTransformPipe,
    HasRoleDirective,
    ConfirmModalComponent,
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
    PaginationModule.forRoot(),
    ButtonsModule.forRoot(),
    ModalModule.forRoot(),
    FileUploadModule,
    TimeagoModule.forRoot(),
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
    PaginationModule,
    ButtonsModule,
    ModalModule,
    FileUploadModule,
    TimeagoModule,
    HasRoleDirective,
  ]
})
export class SharedModule { }
