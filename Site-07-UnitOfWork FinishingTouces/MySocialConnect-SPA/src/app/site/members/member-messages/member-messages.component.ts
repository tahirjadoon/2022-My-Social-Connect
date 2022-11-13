import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';

import { MessageService } from '../../../core/services/message.service';
import { MessageHubService } from '../../../core/services/signalr/message-hub.service';

import { MessageDto } from '../../../core/models/messageDto';
import { Subscription } from 'rxjs';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit, OnDestroy {
  @ViewChild('messageForm') messageForm!: NgForm;
  @Input() userId!: number;
  @Input() messages: MessageDto[] = [];
  messageContent!: string;

  messageSubscription!: Subscription;
  messagesHubSubscription!: Subscription;
  messageHubSendSubscription!: Subscription;

  constructor(private messageService: MessageService, private messageHubService: MessageHubService) { }

  ngOnInit(): void {
    this.loadMessagesFromMessageHub();
  }

  ngOnDestroy(): void {
    if (this.messageSubscription) this.messageSubscription.unsubscribe();
    if (this.messagesHubSubscription) this.messagesHubSubscription.unsubscribe();
    if (this.messageHubSendSubscription) this.messageHubSendSubscription.unsubscribe();
  }

  loadMessagesFromMessageHub() {
    this.messagesHubSubscription = this.messageHubService.messageThread$.subscribe({
      next: (messages: MessageDto[]) => {
        this.messages = messages;
      }
    });
  }

  //this not being used any more. below method using hub is being used now
  onSendMessage() {
    this.messageSubscription = this.messageService.sendMessage(this.userId, this.messageContent).subscribe({
      next: (message: MessageDto) => {
        this.messages.push(message);
        this.messageForm.reset();
      },
      error: e => { },
      complete: () => { }
    });
  }

  onSendMessageUsingHub() {
    //this is returning a promise. Then new message added will be show via loadMessagesFromMessageHub
    this.messageHubService.sendMessage(this.userId, this.messageContent).then(() => {
      this.messageForm.reset();
    });
  }
}
