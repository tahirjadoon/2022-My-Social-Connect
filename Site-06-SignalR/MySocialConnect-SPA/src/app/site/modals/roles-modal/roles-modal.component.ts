import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { zRoles } from 'src/app/core/enums/zRoles';
import { userDto } from 'src/app/core/models/userDto.model';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  /*
  following are the default, will over write for our use case. For the default use see: https://valor-software.com/ngx-bootstrap/#/components/modals?tab=overview
  title: string = "";
  list: any[] = [];
  closeBtnName: string = "";
  */
  @Input() updateSelectedRoles = new EventEmitter();
  user: userDto = <userDto>{};
  roles: any[] = [];

  zRoles = zRoles;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  onUpdateSelectedRoles() {
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}
