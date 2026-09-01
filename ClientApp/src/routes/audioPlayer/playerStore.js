import { writable, get } from "svelte/store";
import { SetPlayState } from "./audioPlayer.svelte";

export let queue = writable([]);
export let index = writable(0);
export let from = writable("none");
export let loading = writable(false);
export let playState = writable(false);
export let shuffleValue = writable(false);
export let repeatValue = writable(0); // 0: no repeat | 1: repeat queue | 2: repeat song


let originalQueue = [];

export function cycleRepeatMode() {
    repeatValue.update(val => (val + 1) % 3);
    console.log("repeat state: " + get(repeatValue));
}

function shuffleArray(array) {
    const shuffled = [...array];
    for (let i = shuffled.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
    }
    return shuffled;
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

        const otherSongs = currentQueue.filter((_, idx) => idx !== currentIndex);
        const shuffledOthers = shuffleArray(otherSongs);

        queue.set([currentSong, ...shuffledOthers]);
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


export function NextTrack() {
    const currentQueue = get(queue);
    if (currentQueue.length === 0) return;

    const rep = get(repeatValue);
    const currentIndex = get(index);

    if (rep === 2) return;

    let next = currentIndex + 1;

    if (next >= currentQueue.length) {
        if (rep === 1) {
            index.set(0);
        }
    } else {
        index.set(next);
    }
}

export function PreviousTrack() {
    const currentQueue = get(queue);
    if (currentQueue.length === 0) return;

    const rep = get(repeatValue);
    const currentIndex = get(index);

    let prev = currentIndex - 1;

    if (prev < 0) {
        if (rep === 1) {
            index.set(currentQueue.length - 1);
        }
    } else {
        index.set(prev);
    }
}


export function SetCurrentPlaylist(videos, i = 0, From = "") {
    originalQueue = [...videos];
    const isShuffled = get(shuffleValue);

    if (isShuffled && videos.length > 0) {
        const selectedSong = videos[i];
        const otherSongs = videos.filter((_, idx) => idx !== i);
        queue.set([selectedSong, ...shuffleArray(otherSongs)]);
        index.set(0);
    } else {
        queue.set([...videos]);
        index.set(i);
    }

    playState.set(true);
    from.set(From);
}

export function AddToQueue(video) {
    const itemsToAdd = Array.isArray(video) ? video : [video];

    queue.update(currentVideos => [...currentVideos, ...itemsToAdd]);

    if (get(shuffleValue)) {
        originalQueue = [...originalQueue, ...itemsToAdd];
    }
}

export function RemoveFromQueue(id) {
    const currentQueue = get(queue);
    const removeIdx = currentQueue.findIndex(x => x.id === id);

    if (removeIdx === -1) return;

    const currentIndex = get(index);


    if (removeIdx < currentIndex) {
        index.update(i => i - 1);
    }

    queue.update(songs => songs.filter(s => s.id !== id));
    originalQueue = originalQueue.filter(s => s.id !== id);
}
