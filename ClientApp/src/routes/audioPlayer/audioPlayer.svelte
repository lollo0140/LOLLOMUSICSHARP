<script module>
    let player = $state();

    let volume = $state(0.5);

    export function SetPlayerVolume(vol) {
        volume = vol / 100;
    }

    export function SetCurrentTimelineSec(sec) {
        player.currentTime = sec;
    }

    export function SetPlayState(state) {
        if (state) {
            player.play();
        } else {
            player.pause();
        }
    }
</script>

<script>
    import { onMount } from "svelte";

    import {
        playState,
        queue,
        index,
        NextTrack,
        repeatValue,
        loading,
    } from "./playerStore";
    import {
        SetCurrentTime,
        SetDurationTime,
    } from "../mainscreencomponents/Controlls.svelte";
    import { ESend } from "../../scripts/electronInvoker";
    import { SetLyricsCurrentTime } from "../../svelte_components/single/LyricsBox.svelte";

    let currentSong = $derived($queue[$index]);

    let audioSource = $derived.by(() => {
        if (currentSong != undefined && currentSong?.id != undefined) {
            const url = `http://localhost:8001/api/audio/${currentSong.id}`;

            console.log(url);

            return url ?? undefined;
        }

        return undefined;
    });
</script>

<audio
    ontimeupdate={() => {
        SetCurrentTime(player.currentTime);
        SetLyricsCurrentTime(player.currentTime);
    }}
    onplay={() => {
        $loading = false;
        SetDurationTime(player.duration);
        $playState = true;

        if ($queue?.[$index + 1]?.id != undefined) {
            ESend("cacheId", $queue?.[$index + 1]?.id);
        }

        ESend("setRpc", JSON.stringify(currentSong));
    }}
    onpause={() => {
        $playState = false;
    }}
    autoplay
    onloadstart={() => {
        $loading = true;

        if (currentSong && currentSong?.type === "none") {
            console.log("non valid track. skipping to the next");
            NextTrack();
        }
    }}
    onended={() => {
        console.log($repeatValue);

        if ($repeatValue === 2) {
            player.currentTime = 0;
            player.play();
        } else {
            NextTrack();
        }
    }}
    bind:this={player}
    src={audioSource != undefined ? audioSource : ""}
    {volume}
></audio>
