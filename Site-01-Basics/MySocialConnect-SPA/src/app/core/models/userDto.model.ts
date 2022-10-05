import { PhotoDto } from "./photoDto.model";

export class userDto {
    constructor(public id: number, public guId: string, public userName: string, public photoUrl: string, public age: number,
                public displayName: string, public gender: string, public introduction: string,
                public lookingFor: string, public interests: string, public city: string, public country: string,
                public photos: PhotoDto[],
                public lastActive: Date, public createdOn: Date, public updatedOn: Date) { }
}