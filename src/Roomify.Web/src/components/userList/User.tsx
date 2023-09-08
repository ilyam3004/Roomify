import {IUser} from "../../types/types";
import React, {FC} from "react";
import "../../App.scss";

interface UserProps {
    user: IUser;
}

export const User: FC<UserProps> = ({user}) => {
    return (
        <div key={user.userId} className="connected-user">
            <div className="user-img">
                {
                    user.avatar
                        ?
                        <img
                            src={user.avatar}
                            alt=""/>
                        :
                        <img src="https://media.istockphoto.com/id/1131164548/vector/avatar-5.jpg?s=612x612&w=0&k=20&c=CK49ShLJwDxE4kiroCR42kimTuuhvuo2FH5y_6aSgEo="
                             alt=""/>
                }
            </div>
            <div className="username">
                {user.username}
            </div>
        </div>
    );
};