import { Component, OnInit } from '@angular/core';

import { userDto } from '../../../core/models/userDto.model';

import { MembersService } from '../../../core/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members: userDto[] = [];

  constructor(private membersService: MembersService) { }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.membersService.getMembers().subscribe({
      next: (members : userDto[]) => {
        this.members = members;
        console.log(members);
      }, error: e => {
        //error will get displayed by the error interceptor
      }, complete: () => {}
    });
  }

}
