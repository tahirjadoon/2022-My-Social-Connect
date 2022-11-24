import { Component, OnInit } from '@angular/core';
import { PhotoForApprovalDto } from '../../../core/models/photoForApprovalDto';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos!: PhotoForApprovalDto[];

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe({
      next: (photos: PhotoForApprovalDto[]) => {
        this.photos = photos;
      },
      error: e => { },
      complete: () => { }
    });
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () => {
        this.photos.slice(this.photos.findIndex(p => p.id == photoId), 1);
      }
    });
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () => {
        this.photos.slice(this.photos.findIndex(p => p.id == photoId), 1);
      }
    });
  }

}
