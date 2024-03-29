import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() placeHolder: string = '';
  @Input() maxDate!: Date;

  //Partial means every property inside BsDatepickerConfig is optional. 
  bsConfig!: Partial<BsDatepickerConfig>; 
  
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;

    this.bsConfig = {
      containerClass: 'theme-red', 
      dateInputFormat: 'YYYY-MM-DD',
      
    }
  }
  
  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }
  /*
  setDisabledState?(isDisabled: boolean): void {
  }
  */

}
