import { Pagination } from "./pagination.model";

export class PaginatedResult<T> {
    result!: T;
    pagination!: Pagination;

    constructor() {}
}
