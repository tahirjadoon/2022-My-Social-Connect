import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmModalComponent } from '../../site/modals/confirm-modal/confirm-modal.component';

//also check site/admin/user-management/user-management.component.ts for another modal use

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  private bsModalRef!: BsModalRef;

  private defaultTitle: string = 'Confirmation';
  private defaultMessage: string = 'Are you sure you want to do this?';
  private defaultButtonOkText: string = 'Ok';
  private defaultButtonCancelText: string = 'Cancel';

  constructor(private modalService: BsModalService) { }

  confirm(title = this.defaultTitle,
                  message = this.defaultMessage,
                  btnOkText = this.defaultButtonOkText,
                  btnCancelText = this.defaultButtonCancelText) : Observable<boolean> {
    const config = {
      initialState: { title, message, btnOkText, btnCancelText }
    };
    this.bsModalRef = this.modalService.show(ConfirmModalComponent, config);

    return new Observable<boolean>(this.getResult());
  }

  private getResult() {
    return (observer: any) => {
      const subscription = this.bsModalRef.onHidden?.subscribe(() => {
        observer.next(this.bsModalRef.content?.result);
        observer.complete();
      });

      return {
        unsubscribe() {
          subscription?.unsubscribe();
        }
      };
    }
  }

}
