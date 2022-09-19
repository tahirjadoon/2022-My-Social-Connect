import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { ToastrService } from 'ngx-toastr';
import {NgxGalleryOptions} from '@kolkov/ngx-gallery';
import {NgxGalleryImage} from '@kolkov/ngx-gallery';
import {NgxGalleryAnimation} from '@kolkov/ngx-gallery';

import { MembersService } from '../../../core/services/members.service';

import { userDto } from '../../../core/models/userDto.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {

  member: userDto = <userDto>{};
  guid: string = "";
  name: string = "";

  //ngx gallery
  galleryOptions!: NgxGalleryOptions[];
  galleryImages!: NgxGalleryImage[];

  paramSubscription!: Subscription;
  membersSubscription!: Subscription;

  constructor(private memberService: MembersService, private route: ActivatedRoute, private router: Router, private toastrService: ToastrService) {}

  ngOnInit(): void {
    //gallery optioins for the images
    this.setGalleryOptions();
    
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
  }

  ngOnDestroy(): void {
    if (this.paramSubscription) this.paramSubscription.unsubscribe();
    if (this.membersSubscription) this.membersSubscription.unsubscribe();
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
      }, error: e => {

      }, complete: () => {

      }
    });
  }

}
