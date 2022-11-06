import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs';
import { UserTokenDto } from '../models/userTokenDto.model';
import { AccountService } from '../services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];
  user: UserTokenDto = <UserTokenDto>{};

  constructor(private viewContaineRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService) {

    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user: UserTokenDto) => {
        this.user = user;
      },
      error: e => { },
      complete: () => { }
    });

  }
  ngOnInit(): void {
    //clear the view if no roles
    if (!this.user?.roles || this.user == null) {
      this.viewContaineRef.clear();
      return;
    }

    //if the user has roles and are in passed then keep it other wise clear it
    if (this.user?.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContaineRef.createEmbeddedView(this.templateRef);
    }
    else {
      this.viewContaineRef.clear();
    }

  }

}
