import { HttpParams } from "@angular/common/http";

export class PageParams{
    pageNumber: number = 1;
    pageSize: number = 5;

    constructor() { }
    

    getPaginationSearchParams() : HttpParams {
        let params = new HttpParams();
        params = params.append('pageNumber', this.pageNumber.toString());
        params = params.append('pageSize', this.pageSize.toString());
        return params;
    }
}