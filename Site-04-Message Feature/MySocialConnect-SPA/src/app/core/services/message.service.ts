import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiUrlService } from './api-url.service';
import { HttpClientService } from './http-client.service';
import { PaginationService } from './pagination.service';

import { MessageParams } from '../models/helpers/message-params.model';
import { environment } from '../../../environments/environment';
import { MessageDto } from '../models/messageDto';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private apiUrlService: ApiUrlService,
    private httpClientService: HttpClientService,
    private pageService: PaginationService) { }

  getMessages(msgParams: MessageParams) {
    //add the pagination and filtering parameters
    const params = msgParams.getSearchParams();
    var url = this.apiUrlService.messagesGet;
    if (environment.displayConsoleLog) console.log(`getMessages url: ${url} params: ${params}`);

    return this.pageService.getPaginatedResult<MessageDto[]>(url, params);
  }

  getMessageThread(otherUserId: number) : Observable<MessageDto[]> {
    var url = this.apiUrlService.messageThread.replace(this.apiUrlService.messageRecipIdReplace, otherUserId.toString());
    if (environment.displayConsoleLog) console.log(`getMessageThread url: ${url}`);
    return this.httpClientService.get<MessageDto[]>(url);
  }

  sendMessage(receipentUserId: number, content: string) {
    var url = this.apiUrlService.messageSend;
    return this.httpClientService.post<MessageDto>(url, { receipientUserId: receipentUserId, content: content });
  }

  deleteMessage(msgId: number) {
    var url = this.apiUrlService.messageDelete.replace(this.apiUrlService.messageDelIdReplace, msgId.toString());
    return this.httpClientService.delete(url);
  }

}
