import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, retry } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class HttpClientService {

  retries: number = 0;

  constructor(private httpClient: HttpClient) { }

  //for passing the params check the following thread, accepted answer
  //https://stackoverflow.com/questions/51885155/httpparams-use-with-put-and-post-change-return-type-to-observablehttpeventt/51889413?noredirect=1#comment90730527_51889413

  /**
   * A GET method
   * @param url api url
   * @param params pass empty meaning do not pass when not used, will cover stuff like ?x=1&y=2,
   *        instead use HttpParams  pass as { params: { sessionTimeOut: 'y' } } or
   *          const params = new HttpParams().set('id', 1).set('name','tahir');
   *        or as following 3 lines
   *        let params = {};
   *        params['params'] = new HttpParams().set('groupType', this.groupType.toString());
   *        params['observe'] = 'body';
   * @returns returns TReturn string/number/model
   */
  get<TReturn>(url: string, params = {}) : Observable<TReturn> {
    return this.httpClient
      .get<TReturn>(url, params )
      .pipe(retry(this.retries));
  }

    /**
   * A GET method
   * @param url api url
   * @param params pass empty meaning do not pass when not used, will cover stuff like ?x=1&y=2,
   *        instead use HttpParams
   *        let params = new HttpParams();
   *        params = params.append('pageNumber', page.toString());
   *        params = params.append('pageSize', itemsPerPage.toString());
   * @returns rreturns the full HttpResponse
   */
  getWithFullResponse<TReturn>(url: string, params = {}) : Observable<HttpResponse<TReturn>> {
    return this.httpClient
      .get<TReturn>(url, { observe: 'response', params })
      .pipe(retry(this.retries));
  }

  /**
   * A POST method
   * @param url api url
   * @param body model posting
   * @param params pass empty meaning do not pass when not used, will cover stuff like ?x=1&y=2,
   *        let params = {};
   *        params['params'] = new HttpParams().set('groupType', this.groupType.toString());
   *        params['observe'] = 'body';
   * @returns returns TReturn string/number/model
   */
  post<TReturn>(url: string, body: any, params = {}): Observable<TReturn> {
    //console.log(url);
    return this.httpClient
    .post<TReturn>(url, body, params)
    .pipe(retry(this.retries));
  }

  /**
   * A PUT method
   * @param url  api url
   * @param body model posting
   * @param params pass empty meaning do not pass when not used, will cover stuff like ?x=1&y=2,
   *        let params = {};
   *        params['params'] = new HttpParams().set('groupType', this.groupType.toString());
   *        params['observe'] = 'body';
   * @returns returns TReturn string/number/model
   */
  put<TReturn>(url: string, body: any, params = {}): Observable<TReturn> {
    return this.httpClient
      .put<TReturn>(url, body, params)
      .pipe(retry(this.retries));
  }

  /**
   * A DELETE method
   * @param url  api url without leading / and MWAPI/ as well
   * @param params pass empty meaning do not pass when not used, will cover stuff like ?x=1&y=2,
   *        let params = {};
   *        params['params'] = new HttpParams().set('groupType', this.groupType.toString());
   *        params['observe'] = 'body';
   */
  delete(url: string, params = {}): Observable<object> {
    return this.httpClient
      .delete(url, params)
      .pipe(retry(this.retries));
  }

}
