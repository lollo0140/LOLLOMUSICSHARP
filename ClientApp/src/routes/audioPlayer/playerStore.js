import { writable, get } from "svelte/store";
import { SetPlayState } from "./audioPlayer.svelte";

export let queue = writable([]);
export let index = writable(0);
export let from = writable("none")
export let playState = writable(false);
export let shuffleValue = writable(false);
export let repeatValue = writable(0); // 0: no repeat | 1: repeat queue | 2: repeat song

// Variabile interna per salvare l'ordine originale prima dello shuffle
let originalQueue = [];

export function cycleRepeatMode() {
    if (get(repeatValue) === 0) {
        repeatValue.set(1);
    } else if (get(repeatValue) === 1) {
        repeatValue.set(2);
    } else if (get(repeatValue) === 2) {
        repeatValue.set(0);
    }

    console.log("repeat state: " + get(repeatValue));
}

export function toggleShuffleMode() {
    const isShuffled = get(shuffleValue);
    const currentQueue = get(queue);
    const currentIndex = get(index);
    const currentSong = currentQueue[currentIndex];

    if (!currentSong || currentQueue.length === 0) {
        shuffleValue.set(!isShuffled);
        return;
    }

    if (!isShuffled) {
        originalQueue = [...currentQueue];

        const otherSongs = currentQueue.filter(song => song.id !== currentSong.id);

        for (let i = otherSongs.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [otherSongs[i], otherSongs[j]] = [otherSongs[j], otherSongs[i]];
        }

        const newShuffledQueue = [currentSong, ...otherSongs];

        queue.set(newShuffledQueue);
        index.set(0);
        shuffleValue.set(true);

    } else {

        if (originalQueue.length > 0) {

            queue.set([...originalQueue]);

            const restoredIndex = originalQueue.findIndex(song => song.id === currentSong.id);

            index.set(restoredIndex !== -1 ? restoredIndex : 0);
        }

        shuffleValue.set(false);
    }

    console.log("shuffle state: " + get(shuffleValue));
}




// main buttons

export function NextTrack() {
    const currentQueue = get(queue);

    if (currentQueue.length === 0) return;

    index.update(i => {
        const next = i + 1;

        if (next >= currentQueue.length) {
            if (get(repeatValue) != 0) {
                return 0;
            }
        } else {
            return next;
        }


    });
}

export function PreviousTrack() {
    const currentQueue = get(queue);

    if (currentQueue.length === 0) return;

    index.update(i => {
        const prev = i - 1;
        if (prev < 0) {
            if (get(repeatValue) != 0) {
                return currentQueue.length - 1;
            }

        } else {
            return prev;
        }
    });
}

export function SetCurrentPlaylist(videos, i = 0, From = "") {
    queue.set(videos);
    index.set(i);
    playState.set(true);
    from.set(From)

    console.log(videos, i);

}

export function AddToQueue(video) {
    const itemsToAdd = Array.isArray(video) ? video : [video];

    queue.update(currentVideos => {
        return [...currentVideos, ...itemsToAdd];
    });
}
