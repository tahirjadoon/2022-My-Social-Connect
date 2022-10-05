import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'validatorsTransform'
})
export class ValidatorsTransformPipe implements PipeTransform {

  transform(value: any, ...args: any[]): any {
    if (!value) return value;
    return Object.keys(value);
  }
}
