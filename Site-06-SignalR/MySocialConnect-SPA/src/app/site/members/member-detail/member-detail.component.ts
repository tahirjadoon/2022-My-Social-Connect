import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, take } from 'rxjs';

import { ToastrService } from 'ngx-toastr';
import {NgxGalleryOptions} from '@kolkov/ngx-gallery';
import {NgxGalleryImage} from '@kolkov/ngx-gallery';
import {NgxGalleryAnimation} from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';

import { MembersService } from '../../../core/services/members.service';
import { MessageService } from '../../../core/services/message.service';
import { AccountService } from '../../../core/services/account.service';
import { PresenceHubService } from '../../../core/services/signalr/presence-hub.service';
import { MessageHubService } from '../../../core/services/signalr/message-hub.service';

import { userDto } from '../../../core/models/userDto.model';
import { MessageDto } from '../../../core/models/messageDto';

import { environment } from '../../../../environments/environment';
import { UserTokenDto } from 'src/app/core/models/userTokenDto.model';


@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {

  //getting the reference to memberTabs
  //adding static since now we are using route resolver to get memebr data
  //also removed the check for members in the html
  @ViewChild('memberTabs', {static: true}) memberTabs!: TabsetComponent;
  //each tab placed in the html
  activeTab!: TabDirective;
  //message subscription
  messageSubscription!: Subscription;
  //messages
  messages:MessageDto[] = []

  member: userDto = <userDto>{};
  guid: string = "";
  name: string = "";

  //ngx gallery
  galleryOptions!: NgxGalleryOptions[];
  galleryImages!: NgxGalleryImage[];

  paramSubscription!: Subscription;
  membersSubscription!: Subscription;
  queryParamSubscription!: Subscription;

  //signalr - presenceHub service - online users
  isUserOnline: boolean = false;
  onlineUsers: string[] = [];
  onlineUsersSubscription!: Subscription;
  //signalr - messageHub service - messages
  messagesHubSubscription!: Subscription;

  //logged in user
  user: UserTokenDto = <UserTokenDto>{};

  constructor(private memberService: MembersService,
    private route: ActivatedRoute,
    private router: Router,
    private toastrService: ToastrService,
    private messageService: MessageService,
    private presenceHubService: PresenceHubService,
    private messageHubService: MessageHubService,
    private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user: UserTokenDto) => {
        this.user = user;
      }
    });
    //do not use routeReusestrategy. When the user is on detail page for one user and received a new message toast for user two then
    //the page will not refresh the content
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    //gallery optioins for the images
    this.setGalleryOptions();

    //we have a route resolver so member is coming from there
    /*
    //there are two ways to get the id and name from the route
    ////we can get the param using snapshot, but this usually breaks when the item is clicked from inside the page and the url is the same as already showing
    //#1
    //this.route.snapshot.paramMap.get('id');
    //#2 way, preferred
    this.paramSubscription = this.route.params.subscribe(params => {
      this.guid = params['guid'];
      this.name = params['name'];
      if (environment.displayConsoleLog) {
        console.log(`Param guid: ${this.guid} name: ${this.name}`);
      }
      if (!this.guid) {
        this.toastrService.error("GUID not received on the page", "User Error")
        this.router.navigate(['/members/list'])
      }
      //we have the id so get the data
      this.initData();
    });
    */
    this.route.data.subscribe({
      next: data => {
        this.member = data['member'];
      }
    });

    this.queryParamSubscription = this.route.queryParams.subscribe({
      next: params => {
        //tab is being passed in as query param
        let tab: number = +params['tab'];
        if (isNaN(tab) || tab > 3) tab = 0;
        this.selectTab(tab);
      },
      error: e => { },
      complete: () => {}
    });

    //moved it here from initData since route resolver is now being used to pcik the member
    //image gallery
    this.galleryImages = this.getImages();

    this.getOnlineUsers();
  }

  ngOnDestroy(): void {
    if (this.paramSubscription) this.paramSubscription.unsubscribe();
    if (this.membersSubscription) this.membersSubscription.unsubscribe();
    if (this.messageSubscription) this.messageSubscription.unsubscribe();
    if (this.queryParamSubscription) this.queryParamSubscription.unsubscribe();
    if (this.onlineUsersSubscription) this.onlineUsersSubscription.unsubscribe();
    if (this.messagesHubSubscription) this.messagesHubSubscription.unsubscribe();

    //when the user moves away from the component then stop the message connection
    this.messageHubService.stopHubConnection();
  }

  //check https://www.npmjs.com/package/@kolkov/ngx-gallery
  setGalleryOptions() {
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
  }

  //check https://www.npmjs.com/package/@kolkov/ngx-gallery
  getImages(): NgxGalleryImage[] {
    const imageUrls: NgxGalleryImage[] = [];
    if (!this.member || !this.member.photos) return imageUrls;
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      })
    }
    if (environment.displayConsoleLog) console.log(imageUrls);
    return imageUrls;
  }

  //not using due to route resolver now. moved this.galleryImages = this.getImages(); to ngOnInit
  /*
  initData() {
    this.membersSubscription = this.memberService.getMemberByGuId(this.guid).subscribe({
      next: (member: userDto) => {
        if (environment.displayConsoleLog) console.log(member);
        if (member.userName.toLowerCase() != this.name.toLowerCase()) {
          this.toastrService.error('Mismatched user');
          this.router.navigate(['/members/list']);
        }
        this.member = member;
        //image gallery
        this.galleryImages = this.getImages();
      },
      error: e => { },
      complete: () => { }
    });
  }
  */

  loadMessages() {
    this.messageSubscription = this.messageService.getMessageThread(this.member.id).subscribe({
      next: (messages: MessageDto[]) => {
        this.messages = messages;
      },
      error: e => { },
      complete: () => { }
    });
  }

  onTabActivate(data: TabDirective) {
    this.activeTab = data;
    //check that the active tab is messages and only load messages when the messages have not already been loaded
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      //rather than loading the messages using message service now gettig the messages via MessageHub
      //this.loadMessages();
      //create the hub connection. note that we are not passing the messages from detail to messages any more
      this.messageHubService.createHubConnection(this.user, this.member.userName, this.member.id);
    }
    else {
      //when the user is not on the messages tab then disconnect from the hub as well
      this.messageHubService.stopHubConnection();
    }
  }

  //make message tab active when the user clicks on Message button in the left nav
  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

    //signalR - presence service
    getOnlineUsers() {
      this.isUserOnline = false;
      this.onlineUsersSubscription = this.presenceHubService.onlineUsers$.subscribe({
        next: (userNames: string[]) => {
          this.onlineUsers = userNames;
          if (userNames && userNames.length > 0) {
            this.isUserOnline = userNames.includes(this.member.userName);
          }
        }
      });
    }

}
