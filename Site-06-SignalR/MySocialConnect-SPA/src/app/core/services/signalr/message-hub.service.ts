import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';

import { MessageDto } from '../../models/messageDto';
import { UserTokenDto } from '../../models/userTokenDto.model';

import { ApiUrlService } from '../api-url.service';

@Injectable({
  providedIn: 'root'
})
export class MessageHubService {

  private hubConnection!: HubConnection;

  private messageThreadSource = new BehaviorSubject<MessageDto[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private apiUrlService: ApiUrlService) { }

  //create Hub Connection
  createHubConnection(user: UserTokenDto, otherUserName: string, otherUserId: number) {
    //query string params for the other user
    const otherParams = `?otherUserName=${otherUserName}&otherUserId=${otherUserId}`;
    const url = this.apiUrlService.signalr_messageUrl + otherParams;
    //build the connection
    this.hubConnection = new HubConnectionBuilder()
                              .withUrl(url, {
                                accessTokenFactory: () => user.token
                              })
                              .withAutomaticReconnect()
                              .build();
    //start the connection
    this.hubConnection.start().catch(error => console.log(error));

    //listen for events UserIsOnline and UserIsOffline. both return userName
    this.hubConnection.on('ReceiveMessageThread', (messages: MessageDto[]) => {
      this.messageThreadSource.next(messages);
    });

    //on new message add
    this.hubConnection.on('NewMessage', (message: MessageDto) => {
      //add the new message to messageThreadSource
      this.messageThread$.pipe(take(1)).subscribe({
        next: (messages: MessageDto[]) => {
          if (message) {
            //without mutating the array add on the new message. this is a new array
            var newMessages = [...messages, message];
            this.messageThreadSource.next(newMessages);
          }
        }
      });
    });
  }

  //stop hub Connection
  stopHubConnection() {
    if(this.hubConnection)
      this.hubConnection.stop().catch(error => console.log(error));
  }

  //for sending the messages. Not using the message-service sendMessage function any more
  async sendMessage(receipentUserId: number, content: string) {
    return this.hubConnection
      .invoke('SendMessage', { receipientUserId: receipentUserId, content: content })
      .catch(error => console.log(error));
  }
}
