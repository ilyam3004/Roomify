import React, {FC} from 'react';
import {IMessage} from "../../types/types";

interface ServerMessageProps{
    message: IMessage;
}

export const ServerMessage: FC<ServerMessageProps> = ({message}) => {
    return (
        <div>
            <div className="message from-server">
                <div className="message-text">
                    {message.text}
                </div>
            </div>
        </div>
    );
};
