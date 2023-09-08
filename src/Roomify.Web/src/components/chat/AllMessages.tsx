import React, {FC, useEffect, useState} from 'react';
import {IMessage, IUser} from "../../types/types";
import {MessagesByDate} from "./MessagesByDate";
import moment from "moment";
import "../../App.scss";

interface MessagesProps {
    messages: IMessage[];
    user: IUser;
}

export const AllMessages: FC<MessagesProps> = ({messages, user}) => {

    const [messagesByDate, setMessagesByDate] = useState<Record<string, IMessage[]> | null>(null);

    function groupMessagesByDate() {
        const result: Record<string, IMessage[]> = groupBy(messages, m => getDateInFormat(m.date));
        setMessagesByDate(result);
    }

    const groupBy = <T, K extends keyof any>(arr: T[], key: (i: T) => K) =>
        arr.reduce((groups, item) => {
            (groups[key(item)] ||= []).push(item);
            return groups;
        }, {} as Record<K, T[]>);

    const getDateInFormat = (date: Date):string => {
        return moment(date)
            .utc()
            .local()
            .format("MMM D, yyyy");
    }

    useEffect(() => {
        groupMessagesByDate();
    }, [messages]);
    

    return (
        <div className="messages">
            {
                messagesByDate
                ?
                    Object.keys(messagesByDate).map((date:string) => {
                        return <MessagesByDate key={date} date={date}
                                               messagesByDate={messagesByDate[date]}
                                               user={user}/>})
                :
                    <div></div>
            }
        </div>
    );
};
