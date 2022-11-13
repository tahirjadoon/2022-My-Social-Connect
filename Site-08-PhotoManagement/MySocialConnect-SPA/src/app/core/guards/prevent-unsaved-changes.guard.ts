import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { ConfirmService } from '../services/confirm.service';
import { MemberEditComponent } from '../../site/members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

  constructor(private confirmService: ConfirmService){}
  /*
  canDeactivate(component: MemberEditComponent): boolean {
    if (component.editForm.dirty)
    {
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost.');
    }
    return true;
  }
  */
  canDeactivate(component: MemberEditComponent): Observable<boolean> | boolean {
    if (component.editForm.dirty)
    {
      return this.confirmService.confirm();
    }
    return true;
  }
}
