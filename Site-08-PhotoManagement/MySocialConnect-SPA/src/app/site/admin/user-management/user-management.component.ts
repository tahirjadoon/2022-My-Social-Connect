import { Component, OnDestroy, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';

import { Subscription, take } from 'rxjs';
import { zRoles } from '../../../core/enums/zRoles';

import { userDto } from '../../../core/models/userDto.model';

import { AdminService } from '../../../core/services/admin.service';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';

//also check /core/services/confirm.service for abother use of bsModalRef

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit, OnDestroy {
  users?: Partial<userDto[]>;
  usersSubscription?: Subscription;

  //modal reference
  bsModalRef?: BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService, private toastrService: ToastrService) { }

  ngOnDestroy(): void {
    if (this.usersSubscription) this.usersSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.loadUsersWithRoles();
  }

  loadUsersWithRoles() {
    this.usersSubscription = this.adminService.getUsersWithRoles().subscribe({
      next: (users: Partial<userDto[]>) => {
        this.users = users
      }
    });
  }

  openRolesModal(user?: userDto) {
    //default is overwritten check followig for the defaults
    //https://valor-software.com/ngx-bootstrap/#/components/modals?tab=overview
    const config = {
      class: 'modal-disalog-center',
      initialState: {
        user: user,
        roles: this.getRolesArray(user)
      }
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe((values: any[]) => {
      //get the roles that are checked on modal
      const rolesToUpdate = {
        roles: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      //make sure that user has some roles selcted
      if (rolesToUpdate && rolesToUpdate.roles) {
        //update the user roles in DB
        this.adminService.updateUserRoles(user!.guId, rolesToUpdate.roles).pipe(take(1)).subscribe({
          next: () => {
            //update the user roles
            user!.roles = [...rolesToUpdate.roles];
            this.toastrService.success('Roles updates successfully!', 'Success');
          }
        });
      }
      else {
        this.toastrService.show('No roles selected!', 'confirmation');
      }
    });
  }

  private getRolesArray(user?: userDto) : any[] {
    const roles: any[] = [];
    const userRoles = user?.roles;
    const availableRoles: any[] = [
      { name: zRoles.Admin, value: zRoles.Admin },
      { name: zRoles.Moderator, value: zRoles.Moderator },
      { name: zRoles.Member, value: zRoles.Member },
    ];

    availableRoles.forEach(role => {
      let isMatch = false;
      for (const userRole of userRoles!) {
        if (role.name === userRole) {
          //role found for user
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break;
        }
      }
      if (!isMatch) {
        //role not found for user
        role.checked = false;
        roles.push(role);
      }
    });
    return roles;
  }

}
