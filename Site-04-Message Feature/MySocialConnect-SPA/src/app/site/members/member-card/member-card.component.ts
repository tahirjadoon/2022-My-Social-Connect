import { Component, Input, OnDestroy, OnInit } from '@angular/core';

import { userDto } from 'src/app/core/models/userDto.model';

import { ToastrService } from 'ngx-toastr';
import { MembersService } from 'src/app/core/services/members.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit, OnDestroy {

  @Input() member: userDto = <userDto>{};

  addSubscription!: Subscription;

  constructor(private memberService: MembersService, private toastrService: ToastrService) { }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    if (this.addSubscription) this.addSubscription.unsubscribe();
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
}
