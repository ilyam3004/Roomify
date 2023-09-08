import React, { FC } from 'react';
import { useNavigate } from "react-router-dom";
import { IUser } from "../../types/types";
import "../../App.scss";

interface NavbarProps {
    closeConnection: () => void;
    clearAllRoomData: () => void;
    userData: IUser;
}

export const Navbar: FC<NavbarProps> = ({ closeConnection, userData, clearAllRoomData }) => {

    let navigate = useNavigate();

    const logOut = () => {
        closeConnection();
        clearAllRoomData();
        navigate('../');
    }

    return (
        <div className="navbar">
            <span className="logo">Roomify</span>
            <div className="user">
                {
                    userData.avatar
                        ?
                        <img
                            src={userData.avatar}
                            alt="" />
                        :
                        <img src="https://media.istockphoto.com/id/1131164548/vector/avatar-5.jpg?s=612x612&w=0&k=20&c=CK49ShLJwDxE4kiroCR42kimTuuhvuo2FH5y_6aSgEo="
                            alt="" />
                }
                <span>{userData.username}</span>
                <button className="log-out-button"
                    onClick={logOut}>
                    Leave
                </button>
            </div>
        </div>
    );
};
