import { Component, OnInit } from '@angular/core';

import { MembersService } from '../../core/services/members.service';

import { zUserLikeType } from '../../core/enums/zUserLikeType';

import { userDto } from '../../core/models/userDto.model';
import { LikeDto } from '../../core/models/likeDto';
import { LikeParams } from '../../core/models/helpers/like-params.model';
import { Pagination } from '../../core/models/helpers/pagination.model';
import { PaginatedResult } from 'src/app/core/models/helpers/paginated-result.model';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  //members!: Partial<userDto[]>;
  members: Partial<userDto[]> = [];
  //userLikeType: zUserLikeType = zUserLikeType.liked;
  likeParams!: LikeParams;
  pagination!: Pagination;
  userTypeString!: string;

  //to be used in the component
  zUserLikeType = zUserLikeType;

  constructor(private memberService: MembersService) {
    this.likeParams = new LikeParams();
    this.userTypeString = zUserLikeType[this.likeParams.userLikeType];
  }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    const userTypeString = zUserLikeType[this.likeParams.userLikeType];
    //if the user is asked for a different type then reset the number
    if (this.userTypeString != userTypeString) {
      this.userTypeString = userTypeString;
      this.likeParams.pageNumber = 1;
    }

    this.memberService.getLikes(this.likeParams).subscribe({
      next: (response: PaginatedResult<Partial<userDto[]>>) => {
        this.members = response.result;
        this.pagination = response.pagination;
      },
      error: e => { },
      complete: () => {}
    });
  }

  onPageChanged(event: any) {
    if (this.likeParams)
      this.likeParams.pageNumber = event.page;
    //get next batch of members
    this.loadLikes();
  }

}
