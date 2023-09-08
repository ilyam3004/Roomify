import React, {FC, useEffect, useState} from 'react';
import {Sidebar} from "../components/bars/Sidebar";
import {HubConnection} from "@microsoft/signalr";
import {IMessage, IUser} from "../types/types";
import 'react-toastify/dist/ReactToastify.css';
import {Chat} from "../components/chat/Chat";
import {MoonLoader} from "react-spinners";
import "../App.scss";

interface RoomProps {
    userData: IUser | null
    userList: IUser[]
    messages: IMessage[]
    connection: HubConnection | null;
    closeConnection: () => void;
    clearAllRoomData: () => void;
}

export const Room: FC<RoomProps> = ({userData, userList, messages, connection, closeConnection, clearAllRoomData}) => {

    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        setLoading(false);
    }, []);

    return (
        <div className="room">
            {
                loading
                    ?
                    <div>
                        <MoonLoader
                            loading={loading}
                            color="#000000"
                            size={30}
                            aria-label="Loading Spinner"
                            data-testid="loader"/>
                    </div>
                    :
                    (
                        connection && userData
                            ?
                            (<div className="room-container">
                                <Sidebar userData={userData}
                                         userList={userList}
                                         closeConnection={closeConnection}
                                         connection={connection}
                                         clearAllRoomData={clearAllRoomData}/>
                                <Chat messages={messages}
                                      userData={userData}
                                      connection={connection}
                                      membersCount={userList.length}/>
                            </div>)
                            :
                            (<div>
                                Server error occurred. Connection wasn't opened.
                            </div>)
                    )
            }
        </div>
    );
};
