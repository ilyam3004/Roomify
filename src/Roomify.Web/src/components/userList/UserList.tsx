import React, {FC} from 'react';
import {IUser} from "../../types/types";
import {User} from "./User";
import "../../App.scss";

interface UserListProps {
    users: IUser[]
}

export const UserList: FC<UserListProps> = ({users}) => {
    return (
        <div className="users">
            {
                users.map(user => <User key={user.userId} user={user}/>)
            }
        </div>
    );
};