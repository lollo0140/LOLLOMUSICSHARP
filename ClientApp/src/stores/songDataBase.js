import { get, writable } from "svelte/store";
import { EInvoke, EInvokeJSON } from "../scripts/electronInvoker";

export let likedSongs = writable(new Set());
export let downloaded = writable(new Set());

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
    let savedIds = await EInvokeJSON("scanDownloaded");
    console.log("saved Ids", savedIds);

    downloaded.set(new Set(savedIds));
}

export async function setLikedSongs(likedList) {
    let idList = likedList.map(item => item.id);
    likedSongs.set(new Set(idList));
}

// --- LIKE MANAGEMENT ---

async function LikeSong(id) {
    await EInvoke("setVideoLike", id, "LIKE");

    likedSongs.update(set => {
        set.add(id);
        return set;
    });
}

async function SetSongNeutral(id) {
    await EInvoke("setVideoLike", id, "NEUTRAL");

    likedSongs.update(set => {
        set.delete(id);
        return set;
    });
}

export async function SetVideoLike(id, like) {
    if (!id) return;

    console.log(id);

    if (like) {
        await LikeSong(id);
    } else {
        await SetSongNeutral(id);
    }
}

// --- DOWNLOAD MANAGEMENT ---

export async function DownloadList(items, concurrency = 3) {
    const downloadedSet = get(downloaded);

    const queue = items.filter(item => {
        const itemId = item?.id ?? item;
        return !downloadedSet.has(itemId);
    });

    console.log(`Brani totali: ${items.length} | Già presenti: ${items.length - queue.length} | Da scaricare: ${queue.length}`);

    if (queue.length === 0) return;

    async function worker() {
        while (queue.length > 0) {
            const item = queue.shift();
            try {
                await DownloadSong(item);
            } catch (error) {
                console.error(`Errore durante il download di ${item?.id ?? item}:`, error);
            }
        }
    }

    const workers = Array.from(
        { length: Math.min(concurrency, queue.length) },
        () => worker()
    );

    await Promise.all(workers);
}

export async function DownloadSong(jsonContent) {
    const stringified = JSON.stringify(jsonContent);
    const result = await EInvoke("downloadSong", stringified);

    console.log("result of download", result);

    if (result) {
        downloaded.update(set => {
            set.add(jsonContent.id);
            return set;
        });
    }
}

export async function DeleteLocal(id) {
    await EInvoke("removeLocal", id);

    downloaded.update(set => {
        set.delete(id);
        return set;
    });
}
