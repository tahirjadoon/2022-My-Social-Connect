export interface MessageDto {
    id: number;
    senderId: number;
    senderGuid: string,
    senderUsername: string;
    senderPhotoUrl: string;
    receipientId: number;
    receipientGuid: string;
    receipientUsername: string;
    receipientPhotoUrl: string;
    messageContent: string;
    dateMessageRead?: Date;
    dateMessageSent: Date;
}
