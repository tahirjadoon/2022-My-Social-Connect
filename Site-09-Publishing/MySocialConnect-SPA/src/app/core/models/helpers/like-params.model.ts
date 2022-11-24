import { HttpParams } from "@angular/common/http";

import { zUserLikeType } from "../../enums/zUserLikeType";

import { PageParams } from "./page-params.model";

export class LikeParams extends PageParams {

    userLikeType: zUserLikeType = zUserLikeType.liked;

    constructor() { 
        super();
    }
    
    //helper metod to build search params
    getSearchParams() : HttpParams {
        let params = super.getPaginationSearchParams();
        params = params.append('userLikeType', zUserLikeType[this.userLikeType]);
        return params;
    }
}
