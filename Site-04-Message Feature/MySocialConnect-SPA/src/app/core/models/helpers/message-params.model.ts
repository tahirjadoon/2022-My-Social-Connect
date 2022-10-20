import { HttpParams } from "@angular/common/http";
import { zMessageType } from "../../enums/zMessageType";
import { PageParams } from "./page-params.model";

export class MessageParams extends PageParams {

    messageType: zMessageType = zMessageType.inboxUnread;

    constructor() {
        super();
     }

    //helper metod to build search params
    getSearchParams() : HttpParams {
        let params = super.getPaginationSearchParams();
        params = params.append('messageType', zMessageType[this.messageType]);
        return params;
    }
}
