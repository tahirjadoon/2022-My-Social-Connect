import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';

import { userDto } from '../../models/userDto.model';

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

  constructor(private apiUrlService: ApiUrlService, private toastrService: ToastrService, private router: Router) { }

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
      //remove the user from the usernames
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames, userName]);
      });
    });

    this.hubConnection.on('UserIsOffline', userName => {
      this.toastrService.warning(userName + ' has disconnected');
      //remove the user from online users and fire
      this, this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames.filter(x => x !== userName)]);
      });
    });

    //listen for event GetOnlineUsers, returns string array of user names
    this.hubConnection.on('GetOnlineUsers', (userNames: string[]) => {
      this.onlineUsersSource.next(userNames);
    });

    //message notification when the user is online but not on the same page
    //also provide a click to go to the messages
    this.hubConnection.on('NewMessageReceived', (user: userDto) => {
      this.toastrService.info(`${user.displayName} has sent you a new message!`)
        .onTap
        .pipe(take(1))
        .subscribe(() => {
          this.router.navigateByUrl(`/members/detail/${user.guId}/${user.userName}?tab=3`)
        })
        ;
    })
  }

  //stop hub Connection
  stopHubConnection() {
    if(this.hubConnection)
      this.hubConnection.stop().catch(error => console.log(error));
  }
}
