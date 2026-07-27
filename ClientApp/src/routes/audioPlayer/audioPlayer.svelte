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

    import { playState, queue, index, NextTrack, repeatValue } from "./playerStore";
    import { SetCurrentTime, SetDurationTime } from "../mainscreencomponents/Controlls.svelte";

    let audioSource = $derived.by(() => {
        const current = $queue[$index];

        if (current != undefined && current?.id != undefined) {
            const url = `http://localhost:8001/api/audio/${current.id}`;

            console.log(url);

            return url ?? undefined;
        }

        return undefined;
    });
</script>

<audio

    ontimeupdate={ () => {
        SetCurrentTime(player.currentTime)
    }}

    onplay={() => {
        SetDurationTime(player.duration)
        $playState = true;
    }}
    onpause={() => {
        $playState = false;
    }}

    autoplay


    

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
