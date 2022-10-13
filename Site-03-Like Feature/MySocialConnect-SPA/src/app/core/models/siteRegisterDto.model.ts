export class SiteRegisterDto {
    constructor(public userName: string = "",
        public password: string = "",
        public gender: string = "", 
        public displayName: string = "",
        public dateOfBirth: Date,
        public city: string = "",
        public country: string = ""
     ) {}
}
