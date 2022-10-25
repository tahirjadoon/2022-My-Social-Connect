import { Component, Input, OnInit } from '@angular/core';
import { ValidationErrors } from '@angular/forms';

@Component({
  selector: 'app-display-formgroup-errors',
  templateUrl: './display-formgroup-errors.component.html',
  styleUrls: ['./display-formgroup-errors.component.css']
})
export class DisplayFormgroupErrorsComponent implements OnInit {
  @Input() key!: string;
  @Input() errors: ValidationErrors | null | undefined;

  constructor() { }

  ngOnInit(): void {
    
  }

}
