import React, {FC} from 'react';
import {AllMessages} from "./AllMessages";
import {ChatInput} from "../inputs/ChatInput";
import {HubConnection} from "@microsoft/signalr";
import {IMessage, IUser} from "../../types/types";
import "../../App.scss";

interface ChatProps {
    connection: HubConnection;
    messages: IMessage[];
    userData: IUser;
    membersCount: number;
}

export const Chat: FC<ChatProps> = ({messages, userData, connection, membersCount}) => {
    return (
        <div className="chat">
            <div className="chat-info">
                <span>{userData.roomName}</span>
                {
                    membersCount > 1
                        ?
                            <div className="members-count">{membersCount} members</div>
                        :
                            <div className="members-count">{membersCount} member</div>
                }
            </div>
            <AllMessages messages={messages}
                         user={userData}/>
            <ChatInput connection={connection}
                       userData={userData}/>
        </div>
    );
};