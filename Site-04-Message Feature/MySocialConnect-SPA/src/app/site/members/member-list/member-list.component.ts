import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { AppConstants } from '../../../core/constants/app-constants';

import { userDto } from '../../../core/models/userDto.model';
import { Pagination } from '../../../core/models/helpers/pagination.model';
import { PaginatedResult } from '../../../core/models/helpers/paginated-result.model';
import { UserParams } from '../../../core/models/helpers/user-params.model';
import { UserTokenDto } from '../../../core/models/userTokenDto.model';

import { MembersService } from '../../../core/services/members.service';
import { AccountService } from '../../../core/services/account.service';


@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: userDto[] = [];
  pagination!: Pagination;
  userParams!: UserParams;

  //filtering
  genderList = [
    { value: AppConstants.Male, display: 'Males' },
    { value: AppConstants.Female, display: 'Females' }
  ];

  constructor(private membersService: MembersService) { 
    this.userParams = this.membersService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  }
  
  loadMembers() {
    this.membersService.setUserParams(this.userParams);

    this.membersService.getMembers(this.userParams).subscribe({
      next: (response : PaginatedResult<userDto[]>) => {
        if(environment.displayConsoleLog) console.log(response);
        this.members = response.result;
        this.pagination = response.pagination;
      }, error: e => {
        //error will get displayed by the error interceptor
      }, complete: () => {}
    });
  }

  onResetFilters() {
    this.userParams = this.membersService.resetUserParams();
    this.loadMembers();
  }

  onGenderSelect() {
    //reset the page number to 1 since the page the user is on my not exist when the gender is changed 
    this.userParams.pageNumber = 1;
  }

  onPageChanged(event: any) {
    if (this.userParams)
      this.userParams.pageNumber = event.page;
    //get next batch of members
    this.loadMembers();
  }
}
