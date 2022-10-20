import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';

import { MessageService } from '../../../core/services/message.service';

import { MessageDto } from '../../../core/models/messageDto';
import { Subscription } from 'rxjs';


@Component({
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
  
  constructor(private messageService: MessageService) { }

  ngOnInit(): void {

  }

  ngOnDestroy(): void {
    if (this.messageSubscription) this.messageSubscription.unsubscribe();
  } 

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
}
