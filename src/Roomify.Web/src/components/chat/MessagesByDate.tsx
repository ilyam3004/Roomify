import {IMessage, IUser} from "../../types/types";
import {RoomMateMessage} from "./RoomMateMessage";
import {ServerMessage} from "./ServerMessage";
import React, {FC, useRef} from 'react';
import {OwnMessage} from "./OwnMessage";
import moment from "moment/moment";

interface DateMessagesProps {
    date: string;
    messagesByDate: IMessage[],
    user: IUser
}

export const MessagesByDate: FC<DateMessagesProps> = ({messagesByDate, user, date}) => {
    const endComponentRef = useRef<HTMLDivElement>(null);

    function formatTime(date: Date): string {
        return moment
            .utc(date)
            .local()
            .format("hh:mm A");
    }

    return (
        <div>
            <div className="message-date">
                {date}
            </div>
            {
                messagesByDate.map(message => {
                    return <div key={message.messageId} className="message-container">
                        {
                            message.fromUser
                                ?
                                (
                                    message.userId === user.userId
                                        ?
                                        <OwnMessage endComponentRef={endComponentRef}
                                                    message={message}
                                                    formatTime={formatTime}/>
                                        :
                                        <RoomMateMessage message={message}
                                                         formatTime={formatTime}
                                                         endComponentRef={endComponentRef}/>
                                )
                                :
                                (
                                    <ServerMessage message={message}/>
                                )
                        }
                    </div>
                })
            }
            <div ref={endComponentRef}/>
        </div>
    );
};
