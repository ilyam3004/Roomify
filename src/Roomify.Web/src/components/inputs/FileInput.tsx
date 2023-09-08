import React, {ChangeEvent, FC} from 'react'
import './FileInput.scss';

interface FileInputProps {
    imgInputRef: React.RefObject<HTMLInputElement>;
    selectedFile: File | null | undefined;
    setSelectedFile: React.Dispatch<React.SetStateAction<File | null | undefined>>;
    caption: string;
    parentComponent: string;
}

export const FileInput: FC<FileInputProps> = ({imgInputRef, setSelectedFile, selectedFile, caption, parentComponent}) => {

    const onImgInputChange = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            setSelectedFile(e.target.files[0]);
        }
    }

    const getFormattedName = (fileName: string): string => {
        return fileName.length > 20 ? `${fileName.slice(0, 20)}...` : fileName;
    }

    const removeAvatar = (e: React.MouseEvent<HTMLElement>) => {
        e.preventDefault();
        setSelectedFile(null);
    }

    return (
        <label className={parentComponent === "lobby" ? "lobby-file-input" : "chat-file-input" }>
            <input type="file"
                   ref={imgInputRef}
                   accept="image/png, image/gif, image/jpeg"
                   onChange={onImgInputChange}/>
            <span>
                {
                    selectedFile
                        ?
                        <div>
                            {getFormattedName(selectedFile.name)}
                        </div>
                        :
                        <div>
                            {caption}
                        </div>
                }
            </span>
            <button className="remove-button"
                    type="button"
                    onClick={removeAvatar}>
                x
            </button>
        </label>
    )
}
