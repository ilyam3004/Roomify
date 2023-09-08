import React, {FC, useEffect, useState} from 'react';
import {IMessage} from "../../types/types";

interface ICurrentUserMessageProps {
    message: IMessage;
    formatTime: (date: Date) => string;
    endComponentRef: React.RefObject<HTMLDivElement>;
}

export const OwnMessage: FC<ICurrentUserMessageProps> = ({message, formatTime, endComponentRef}) => {
    const [clickedImg, setClickedImg] = useState<string | null>(null);

    const onImgClick = (e: React.MouseEvent<HTMLImageElement>) => {
        setClickedImg(e.currentTarget.src);
    }

    const onImgExitClick = (e: React.MouseEvent<HTMLDivElement>) => {
        setClickedImg(null);
    }

    function handleImageLoad() {
        endComponentRef.current!.scrollIntoView({behavior: 'smooth'});
    }

    useEffect(() => {
        if (!message.isImage) {
            endComponentRef.current?.scrollIntoView({behavior: "smooth"})
        }
    }, []);

    return (
        <div className="message owner">
            <div className="message-info-container">
                <div className="message-info">
                    {
                        message.userAvatar
                            ?
                            <img
                                src={message.userAvatar}
                                alt=""/>
                            :
                            <img
                                src="https://media.istockphoto.com/id/1131164548/vector/avatar-5.jpg?s=612x612&w=0&k=20&c=CK49ShLJwDxE4kiroCR42kimTuuhvuo2FH5y_6aSgEo="
                                alt=""/>
                    }
                </div>
            </div>
            {
                message.isImage
                    ?
                    <div className="img-content">
                        <img src={message.imageUrl}
                             alt=""
                             onClick={onImgClick}
                             loading={"lazy"}
                             onLoad={handleImageLoad}
                        />
                    </div>
                    :
                    <div className="message-content">
                        <div className="message-text">
                            {message.text}
                            <div className="message-time">
                                {formatTime(message.date)}
                            </div>
                        </div>
                    </div>
            }
            {
                clickedImg &&
                (<div className="overlay" onClick={onImgExitClick}>
                    <img src={clickedImg}/>
                    <span onClick={onImgExitClick}>
                        X
                    </span>
                </div>)
            }
        </div>
    );
};
