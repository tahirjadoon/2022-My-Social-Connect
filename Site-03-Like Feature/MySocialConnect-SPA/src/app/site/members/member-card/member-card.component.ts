import { Component, Input, OnInit } from '@angular/core';
import { userDto } from 'src/app/core/models/userDto.model';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() member: userDto = <userDto>{};

  constructor() { }

  ngOnInit(): void {
  }

}
