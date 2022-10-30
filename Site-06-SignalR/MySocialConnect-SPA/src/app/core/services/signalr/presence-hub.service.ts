import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';

import { UserTokenDto } from '../../models/userTokenDto.model';

import { ApiUrlService } from '../api-url.service';

@Injectable({
  providedIn: 'root'
})
export class PresenceHubService {

  private hubConnection!: HubConnection;

  //create a generic subject for getting the online users, it will get the string array of our user names
  //initilize the array as well
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private apiUrlService: ApiUrlService, private toastrService: ToastrService) { }

  //create Hub Connection
  createHubConnection(user: UserTokenDto) {
    //build the connection
    this.hubConnection = new HubConnectionBuilder()
                              .withUrl(this.apiUrlService.signalr_presenceUrl, {
                                accessTokenFactory: () => user.token
                              })
                              .withAutomaticReconnect()
                              .build();
    //start the connection
    this.hubConnection.start().catch(error => console.log(error));

    //listen for events UserIsOnline and UserIsOffline. both return userName
    this.hubConnection.on('UserIsOnline', userName => {
      this.toastrService.info(userName + ' has connected');
    });

    this.hubConnection.on('UserIsOffline', userName => {
      this.toastrService.warning(userName + ' has disconnected');
    });

    //listen for event GetOnlineUsers, returns string array of user names
    this.hubConnection.on('GetOnlineUsers', (userNames: string[]) => {
      this.onlineUsersSource.next(userNames);
    });
  }

  //stop hub Connection
  stopHubConnection() {
    if(this.hubConnection)
      this.hubConnection.stop().catch(error => console.log(error));
  }
}
