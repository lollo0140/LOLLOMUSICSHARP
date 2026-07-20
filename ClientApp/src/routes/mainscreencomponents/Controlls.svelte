<script>
    import { SetPlayState } from "../audioPlayer/audioPlayer.svelte";
    import { NextTrack, PreviousTrack } from "../audioPlayer/playerStore";

    import { playState } from "../audioPlayer/playerStore.js";
</script>

<div class="lollo-appstyle-DivContainer container">
    <div class="mainButtons">
        <button onclick={() => PreviousTrack()}> previous </button>

        <button
            onclick={() => {
                if ($playState) {
                    SetPlayState(false);
                } else {
                    SetPlayState(true);
                }
            }}
        >
            <p>{$playState ? "Pause" : "Play"}</p>
        </button>

        <button onclick={() => NextTrack()}> next </button>
    </div>
</div>

<style>
    .container {
        position: absolute;
        height: calc(100% - 2px);
        width: calc(100% - 2px);

        border-radius: 45px;
        background: transparent;

    }
</style>
