import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subscription, take } from 'rxjs';

import { userDto } from '../../../core/models/userDto.model';
import { UserTokenDto } from '../../../core/models/userTokenDto.model';

import { AccountService } from '../../../core/services/account.service';
import { MembersService } from '../../../core/services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm!: NgForm;
  member: userDto = <userDto>{};
  user: UserTokenDto = <UserTokenDto>{};

  memberUpdateSubscription!:Subscription;

  //inside the angular user wil get propted when the user will move to another route without saving the changes. 
  //when the browser is closed or gone to another website then do the following
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm.dirty)
      $event.returnValue = true;
  }

  memberSubscription!: Subscription;

  constructor(private accountService: AccountService, private memberService: MembersService, private toastrService: ToastrService) {
    //subscribe to the logged in user. due to take(1) dont need to unsubscribe
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberSubscription = this.memberService.getMemberByGuId(this.user.guId).subscribe({
      next: (member: userDto) => {
        this.member = member;
      },
      error: e => {},
      complete: () => { }
    });
  }

  onUpdateMember() {
    this.memberUpdateSubscription = this.memberService.updateMember(this.member).subscribe({
      next: () => {
        //it is not reurning any thing
        this.toastrService.success('Profile updated successfully');
        this.editForm.reset(this.member);
      },
      error: e => { },
      complete: () => {}
    });
  }

}
