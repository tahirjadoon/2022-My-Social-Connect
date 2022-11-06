import { Injectable } from '@angular/core';
import { zEnumType } from '../enums/zEnumType';

@Injectable({
  providedIn: 'root'
})
export class EnumService {

  constructor() { }

    /**
   * Use for the string enum
   * @T : enum
   * @param obj: pass the enum <T>
   * @param enumType: is this number (default) or string enum
   * @returns key value pair array
    To use
    let keys = this.enumService.enumToKeyValuePairs<zMemberGetBy>(zMemberGetBy, zEnumType.number);
    <select placeholder="mode" [(ngModel)]="mode" name="mode">
      <option *ngFor="let c of keys" [value]="c.value">{{ c.name }}</option>
    </select>
  */
    enumToKeyValuePairs<T>(obj: any, enumType: zEnumType = zEnumType.number): { name: string, value: string}[] {
      if(enumType === zEnumType.string)
        return Object.keys(obj as keyof T).map(key => ({ name: key, value: obj[key] }));
      return Object.keys(obj as keyof T).filter((v) => isNaN(Number(v))).map(key => ({ name: key, value: obj[key] }));
    }

    /**
     * get the string description for enum
     * @T : enum
     * @param obj: pass the enum <T>
     * @param enumVal: the enaum value
     * @returns enum string description or undefined
      To use
      let key = this.enumService.enumToString<zRoles>(zRoles, zRoles.Moderator)
    */
    enumToString<T>(obj: any, enumVal: T) : string | undefined {
      for (var item in obj) {
        if (obj[item] === enumVal) return item;
      }
      return undefined;
    }

    /**
     * get the rnum from string description
     * @T : enum
     * @param obj: pass the enum <T>
     * @param val: the description
     * @returns enum or undefined
      To use
      let key = this.enumService.stringToEnum<zRoles>(zRoles, "Moderator")
    */
    stringToEnum<T>(obj: any, val: string,): T | undefined{
      for (var item in obj) {
        if (item === val) return obj[item];
      }
      return undefined;
    }
}
