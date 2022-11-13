import { Component, OnInit } from '@angular/core';

import { zRoles } from '../../../core/enums/zRoles';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {

  zRoles = zRoles;

  constructor() { }

  ngOnInit(): void {
  }

}
