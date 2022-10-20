import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";

import { userDto } from "../models/userDto.model";

import { MembersService } from "../services/members.service";

@Injectable({
    providedIn: 'root'
})
    
//user for /site/members/member-detail
//passing the query param tab which means to make the tab active
//recolver will help with that
//look at the routing module as well
export class MemberDetailResolver implements Resolve<userDto>{

    constructor(private membersService: MembersService){}

    /*
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): userDto | Observable<userDto> | Promise<userDto> {
        throw new Error("Method not implemented.");
    }
    */
    resolve(route: ActivatedRouteSnapshot): Observable<userDto> {
        //get the route params
        const guid = route.paramMap.get('guid'); 
        const name = route.paramMap.get('name');

        return this.membersService.getMemberByGuId(guid!);
    }
}