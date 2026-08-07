import { get, writable } from "svelte/store";
import { EInvoke } from "../scripts/electronInvoker";


export let likedSongs = writable([]);
export let downloaded = writable([]);

export async function setUpLikedList() {

    let content = JSON.parse(
        await window.electron.ipcRenderer.lolloInvoke(
            "getPageData",
            "playlist",
            "VLLM",
        ),
    );

    setLikedSongs(content.items);
}

export async function setLocalSongs() {
    let savedIds = JSON.parse(
        await window.electron.ipcRenderer.lolloInvoke("scanDownloaded")
    );

    downloaded.set(savedIds);
}


export async function setLikedSongs(likedList) {
    let idList = likedList.map(Liked => Liked.id)
    likedSongs.set(idList);
}


// LIKE
// DISLIKE
// NEUTRAL


//like status
async function LikeSong(id) {
    await window.electron.ipcRenderer.lolloInvoke("setVideoLike", id, "LIKE");
    likedSongs.update((ids => [...ids, id]));
}

async function SetSongNeutral(id) {
    await window.electron.ipcRenderer.lolloInvoke("setVideoLike", id, "NEUTRAL");

    likedSongs.update((ids) => {

        let indexElement = ids.indexOf(id);

        if (indexElement !== -1) {
            ids.splice(indexElement, 1);
        }

        return ids
    })

}

export async function SetVideoLike(id, like) {

    if (id === undefined) {
        return;
    }

    console.log(id);

    if (like) {
        LikeSong(id);
    } else {
        SetSongNeutral(id);
    }

}


export async function DownloadSong(id, jsonContent) {

    console.log("downloading: " + id);

    await EInvoke("downloadSong", id, jsonContent)

    if (get(downloaded).find(x => x === id) != undefined) {
        downloaded.update(ids => {

            return [...ids].push(id);

        })
    }

}
