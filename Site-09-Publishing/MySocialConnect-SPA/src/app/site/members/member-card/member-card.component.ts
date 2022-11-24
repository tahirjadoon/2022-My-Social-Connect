import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { userDto } from '../../../core/models/userDto.model';

import { ToastrService } from 'ngx-toastr';
import { MembersService } from '../../../core/services/members.service';
import { PresenceHubService } from '../../../core/services/signalr/presence-hub.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit, OnDestroy {

  @Input() member: userDto = <userDto>{};

  addSubscription!: Subscription;

  //signalr - presence service - online users
  isUserOnline: boolean = false;
  onlineUsers: string[] = [];
  onlineUsersSubscription!: Subscription;

  constructor(private memberService: MembersService,
              private toastrService: ToastrService,
              private presenceService: PresenceHubService) { }

  ngOnInit(): void {
    this.getOnlineUsers();
  }

  ngOnDestroy(): void {
    if (this.addSubscription) this.addSubscription.unsubscribe();
    if (this.onlineUsersSubscription) this.onlineUsersSubscription.unsubscribe();
  }

  onAddLike(member: userDto) {
    this.addSubscription = this.memberService.addLike(member.id).subscribe({
      next: () => {
        this.toastrService.success(`You have liked ${member.displayName}`);
      },
      error: e => {
        //no need to do something here since interceptor will display
      },
      complete: () => {
        //no need to do something here since interceptor will display
      }
    });
  }

  //signalR - presence service
  getOnlineUsers() {
    this.isUserOnline = false;
    this.onlineUsersSubscription = this.presenceService.onlineUsers$.subscribe({
      next: (userNames: string[]) => {
        this.onlineUsers = userNames;
        if (userNames && userNames.length > 0) {
          this.isUserOnline = userNames.includes(this.member.userName);
        }
      }
    });
  }
}
