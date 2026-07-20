<script module>
    let player = $state();

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

    import { playState, queue, index, NextTrack } from "./playerStore";

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
    onplay={() => {
        $playState = true;
    }}
    onpause={() => {
        $playState = false;
    }}

    autoplay

    onended={() => {
        NextTrack();
    }}

    bind:this={player}
    src={audioSource != undefined ? audioSource : ""}
></audio>
