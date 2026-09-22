import { ESend } from "./electronInvoker";

export async function ExportVideo(videoObj) {

    ESend("exportVideo", JSON.stringify(videoObj));

}
