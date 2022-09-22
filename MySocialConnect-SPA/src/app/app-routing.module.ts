import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SiteHomeComponent } from './site/site-home/site-home.component';
import { MemberListComponent } from './site/members/member-list/member-list.component';
import { MemberDetailComponent } from './site/members/member-detail/member-detail.component';
import { ListsComponent } from './site/lists/lists.component';
import { MessagesComponent } from './site/messages/messages.component';
import { TestErrorsComponent } from './site/errors/test-errors/test-errors.component';
import { NotFoundComponent } from './site/errors/not-found/not-found.component';
import { MemberEditComponent } from './site/members/member-edit/member-edit.component';

import { AuthGuard } from './core/guards/auth.guard';
import { ServerErrorComponent } from './site/errors/server-error/server-error.component';
import { PreventUnsavedChangesGuard } from './core/guards/prevent-unsaved-changes.guard';

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
      { path: 'members/detail/:guid/:name', component: MemberDetailComponent },
      { path: 'members/edit', component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard] },
      { path: 'lists', component: ListsComponent },
      { path: 'messages', component: MessagesComponent },
    ]
  },
  { path: 'test-errors', component: TestErrorsComponent },
  { path: 'server-error', component: ServerErrorComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', component:  NotFoundComponent, pathMatch: 'full'} //path match full is important here
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
