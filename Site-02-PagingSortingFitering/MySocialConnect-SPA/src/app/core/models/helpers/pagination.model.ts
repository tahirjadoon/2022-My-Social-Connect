//need to match exactally as Pagination response header
export class Pagination {
    constructor(public currentPage: number, public itemsPerPage: number, public totalItems: number, public totalPages: number){}
}
