<script>
    import { slide } from "svelte/transition";
    import { RemoveFromQueue } from "../../routes/audioPlayer/playerStore";
    import SongButton from "./SongButton.svelte";

    let { onclick, content, index } = $props();
</script>

<div transition:slide class="button-wrapper">
    <div class="song-button">
        <SongButton
            {content}
            {index}
            {onclick}
            renderPhoto={false}
            fatherId={undefined}
        />
    </div>

    <!-- svelte-ignore a11y_consider_explicit_label -->
    <button
        class="remove-button"
        onclick={() => {
            RemoveFromQueue(index);
        }}
    >

    </button>
</div>

<style>
    .song-button {
        width: 92%;
    }

    .remove-button {
        height: 100%;
        width: 50px;
        padding: 0px;
        margin: 0px;

        font-size: 16px;
        border: none;
        background: none;

        color: white;
        font-weight: 800;

        margin-right: 30px;

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
        cursor: pointer;
    }

    .remove-button::before {
        text-overflow: clip;
        width: 150px;
        content: "✕";
    }

    .remove-button:hover {
        width: 150px;
    }

    .remove-button:hover::before {
        content: "REMOVE";

    }


    .button-wrapper {
        display: flex;
        flex-direction: row;

        align-items: center;
        justify-content: space-between;

        height: 56px;
        width: 100%;
    }
</style>
