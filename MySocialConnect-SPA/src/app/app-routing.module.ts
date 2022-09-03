import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SiteHomeComponent } from './site/site-home/site-home.component';
import { MemberListComponent } from './site/members/member-list/member-list.component';
import { MemberDetailComponent } from './site/members/member-detail/member-detail.component';
import { ListsComponent } from './site/lists/lists.component';
import { MessagesComponent } from './site/messages/messages.component';

import { AuthGuard } from './core/guards/auth.guard';

//add the components here
//first empty one is the default route
//** is the default route, it can be any component
const routes: Routes = [
  { path: '', component: SiteHomeComponent },
  //dummy route to group secure resources together
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members/list', component: MemberListComponent },
      { path: 'members/detail/:id', component: MemberDetailComponent },
      { path: 'lists', component: ListsComponent },
      { path: 'messages', component: MessagesComponent },
    ]
  },
  { path: '**', component:  SiteHomeComponent, pathMatch: 'full'} //path match full is important here
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
