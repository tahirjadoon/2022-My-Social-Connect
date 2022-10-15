import { HttpParams } from "@angular/common/http";
import { zUserLikeType } from "../../enums/zUserLikeType";

export class LikeParams {
    pageNumber: number = 1;
    pageSize: number = 5;
    userLikeType: zUserLikeType = zUserLikeType.liked;

    constructor() { }
    
    //helper metod to build search params
    getPaginationSearchParams() : HttpParams {
        let params = new HttpParams();
        params = params.append('pageNumber', this.pageNumber.toString());
        params = params.append('pageSize', this.pageSize.toString());
        return params;
    }
    
    //helper metod to build search params
    getLikesSearchParams() : HttpParams {
        let params = this.getPaginationSearchParams();
        params = params.append('userLikeType', zUserLikeType[this.userLikeType]);
        return params;
    }
}
