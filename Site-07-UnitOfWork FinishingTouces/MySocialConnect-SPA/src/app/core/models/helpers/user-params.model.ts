import { HttpParams } from "@angular/common/http";
import { AppConstants } from "../../constants/app-constants";
import { UserTokenDto } from "../userTokenDto.model";
import { PageParams } from "./page-params.model";

export class UserParams  extends PageParams {
    gender: string;
    minAge: number = 18;
    maxAge: number = 99;
    orderBy: string = AppConstants.membersOrderByLastActive;

    constructor(user: UserTokenDto) {
        super();
        //when logged in user is male then get the female members otherwise male members
        this.gender = user.gender === AppConstants.Female ? AppConstants.Male : AppConstants.Female;
    }

    //helper metod to build search params
    getSearchParams() : HttpParams {
        let params = super.getPaginationSearchParams();
        params = params.append('gender', this.gender);
        params = params.append('minAge', this.minAge.toString());
        params = params.append('maxAge', this.maxAge.toString());
        params = params.append('orderBy', this.orderBy);
        return params;
    }
}