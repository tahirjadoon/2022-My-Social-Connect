import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';

import { AppConstants } from '../constants/app-constants';

import { HttpClientService } from './http-client.service';

import { PaginatedResult } from '../models/helpers/paginated-result.model';

@Injectable({
  providedIn: 'root'
})
export class PaginationService {

  constructor(private httpClientService: HttpClientService) { }

  getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    
    return this.httpClientService.getWithFullResponse<T>(url, params).pipe(
      map(response => {
        paginatedResult.result = response.body!;
        const paginationHeader = response.headers.get(AppConstants.PaginationHeader);
        if (paginationHeader !== null) {
          paginatedResult.pagination = JSON.parse(paginationHeader);
        }
        return paginatedResult;
      })
    );
  }
}