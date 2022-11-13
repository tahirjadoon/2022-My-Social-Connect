import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

import { Subscription } from 'rxjs';

import { zMessageType } from '../../core/enums/zMessageType';

import { PaginatedResult } from '../../core/models/helpers/paginated-result.model';
import { MessageParams } from '../../core/models/helpers/message-params.model';
import { Pagination } from '../../core/models/helpers/pagination.model';

import { MessageDto } from '../../core/models/messageDto';

import { MessageService } from '../../core/services/message.service';
import { ConfirmService } from '../../core/services/confirm.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit, OnDestroy {
  isGettingMessage: boolean = false;
  messages!: MessageDto[];
  pagination!: Pagination;

  msgParams!: MessageParams;
  msgTypeString!: string;
  zMessageType = zMessageType;

  messageSubscription!: Subscription;

  constructor(private msgService: MessageService, private toastrService: ToastrService, private confirmService: ConfirmService) {
    this.msgParams = new MessageParams();
    this.msgTypeString = zMessageType[this.msgParams.messageType];
  }

  ngOnInit(): void {
    this.loadMessages();
  }

  ngOnDestroy(): void {
    if (this.messageSubscription) this.messageSubscription.unsubscribe();
  }

  loadMessages() {
    this.pagination = <Pagination>{};
    this.isGettingMessage = true;
    this.messages = [];
    const msgTypeString = zMessageType[this.msgParams.messageType];

    //if the user is asked for a different type then reset the number
    if (this.msgTypeString !== msgTypeString) {
      this.msgTypeString = msgTypeString;
      this.msgParams.pageNumber = 1;
    }

    this.messageSubscription = this.msgService.getMessages(this.msgParams).subscribe({
      next: (response: PaginatedResult<MessageDto[]>) => {
        this.messages = response.result;
        if(this.messages && this.messages.length > 0)
          this.pagination = response.pagination;
      },
      error: e => { },
      complete: () => {
        this.isGettingMessage = false;
      }
    });
  }

  isOutBox() : boolean {
    return this.msgParams.messageType === zMessageType.outbox;
  }

  memberDetailLink(message: MessageDto): string{
    let url = '/members/detail/';
    if (this.isOutBox())
      url += message.receipientGuid + '/' + message.receipientUsername;
    else
      url += message.senderGuid + '/' + message.senderUsername;
    return url;
  }

  memberPhotoUrl(message: MessageDto): string{
    console.log(this.isOutBox());
    let url = message.senderPhotoUrl;
    if (this.isOutBox())
      url = message.receipientPhotoUrl;
    return url;
  }

  memberUserName(message: MessageDto): string{
    let name = message.senderUsername;
    if (this.isOutBox())
      name = message.receipientUsername;
    return name;
  }

  onPageChanged(event: any) {
    if (this.msgParams && this.msgParams.pageNumber !== event.page) {
      this.msgParams.pageNumber = event.page;
      this.loadMessages();
    }
  }

  onDeleteMessage(msgId: number) {
    //call the confirmService and overwrite the defaults
    const title = 'Confirm Delete';
    const message = 'Are you sure to delete the message? This cannot be undone.';
    const btnDelete = 'Delete';
    this.confirmService.confirm(title, message, btnDelete).subscribe({
      next: result => {
        //only delete when true is returned
        if (result) this.performDelete(msgId);
      },
      error: e => { },
      complete: () => {}
    })
  }

  private performDelete(msgId: number) {
    this.msgService.deleteMessage(msgId).subscribe({
      next: () => {
        //remove the message from the messages by passing the index and total number of messages to delete which is one in this case
        this.messages.splice(this.messages.findIndex(x => x.id === msgId), 1);
        this.toastrService.success("Message deleted", 'Delete Result');
      },
      error: e => { },
      complete: () => { }
    });
  }
}
