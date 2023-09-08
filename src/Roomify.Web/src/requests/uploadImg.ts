import axios from "axios";
import {IUploadResult} from "../types/types";

export async function uploadImg(image: File, isAvatar: string):Promise<IUploadResult> {
    let data = new FormData();
    data.append('image', image);
    data.append('isAvatar', isAvatar);

    const url = `${process.env.REACT_APP_API_URL}/img/uploadImage`;

    const result = await axios.post(url, data).then((response) => {
        return response.data;
    }).catch((error) => {
        console.log(error);
    });

    return result;
}