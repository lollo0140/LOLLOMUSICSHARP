import { writable, get } from "svelte/store";
import { SetPlayState } from "./audioPlayer.svelte";

export let queue = writable([]);
export let index = writable(0);
export let playState = writable(false);

export function NextTrack() {
    const currentQueue = get(queue);

    if (currentQueue.length === 0) return;

    index.update(i => {
        const next = i + 1;
        return next >= currentQueue.length ? 0 : next;
    });
}

export function PreviousTrack() {
    const currentQueue = get(queue);

    if (currentQueue.length === 0) return;

    index.update(i => {
        const prev = i - 1;
        return prev < 0 ? currentQueue.length - 1 : prev;
    });
}

export function SetCurrentPlaylist(videos, i = 0) {
    queue.set(videos);
    index.set(i);
    playState.set(true);

    console.log(videos, i);

}

export function AddToQueue(video) {
    const itemsToAdd = Array.isArray(video) ? video : [video];

    queue.update(currentVideos => {
        return [...currentVideos, ...itemsToAdd];
    });
}
