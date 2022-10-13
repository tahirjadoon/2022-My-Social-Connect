import { HttpParams } from "@angular/common/http";
import { AppConstants } from "../../constants/app-constants";
import { UserTokenDto } from "../userTokenDto.model";

export class UserParams {
    gender: string;
    minAge: number = 18;
    maxAge: number = 99;
    pageNumber: number = 1;
    pageSize: number = 5;
    orderBy: string = AppConstants.membersOrderByLastActive;

    constructor(user: UserTokenDto) {
        //when logged in user is male then get the female members otherwise male members
        this.gender = user.gender === AppConstants.Female ? AppConstants.Male : AppConstants.Female;
    }

    //helper metod to build search params
    getPaginationSearchParams() : HttpParams {
        let params = new HttpParams();
        params = params.append('pageNumber', this.pageNumber.toString());
        params = params.append('pageSize', this.pageSize.toString());
        return params;
    }

    //helper metod to build search params
    getMemberSearchParams() : HttpParams {
        let params = this.getPaginationSearchParams();
        params = params.append('gender', this.gender);
        params = params.append('minAge', this.minAge.toString());
        params = params.append('maxAge', this.maxAge.toString());
        params = params.append('orderBy', this.orderBy);
        return params;
    }
}