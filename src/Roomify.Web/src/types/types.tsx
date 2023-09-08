export interface IJoinRoomRequest {
    username: string;
    avatar: string;
    roomName: string;
}

export interface IMessage {
    messageId: string;
    roomId: string;
    username: string;
    userId: string;
    userAvatar: string;
    text: string;
    date: Date;
    fromUser: boolean;
    isImage: boolean;
    imageUrl: string;
}

export interface IUser {
    userId: string;
    username: string;
    connectionId: string;
    roomId: string;
    roomName: string;
    avatar: string;
}
export interface IError{
    status: number;
    title: string;
    type: string;
    errors: Record<string, string[]>;
}

export interface IUploadResult{
    publicId: string;
    imgUrl: string;
}

export interface ISendImgToRoomRequest{
    userId: string;
    roomId: string,
    imageUrl: string;
}