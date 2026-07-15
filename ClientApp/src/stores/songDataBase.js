import { writable } from "svelte/store";


export let likedSongs = writable([]);

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
