<script>
    import { fly, slide } from "svelte/transition";
    import SongButton from "../../svelte_components/reusable/SongButton.svelte";

    import { queue, index, from } from "../audioPlayer/playerStore.js";
    import QueueButton from "../../svelte_components/reusable/QueueButton.svelte";

    let alreadPlayed = $state(false);
</script>

<main in:fly={{ y: -20 }}>
    <div class="queue-title">
        <p class="page-title">QUEUE</p>
        <p class="from">{$from.toUpperCase()}</p>
    </div>

    <button class="alreadyPlayed" onclick={ () => {alreadPlayed = !alreadPlayed}}>
        {alreadPlayed ? "HIDE PLAYED" : "SHOW PLAYED"}
    </button>

    {#each $queue as song, i}
        {#if i === $index}
            <div class="current">
                <p class="current-song-label">CURRENT SONG</p>
                <SongButton
                    renderPhoto={false}
                    onclick={() => {
                        $index = i;
                    }}
                    content={song}
                    index={i}
                />
            </div>
        {:else if i > $index}
            <QueueButton
                onclick={() => {
                    $index = i;
                }}
                content={song}
                index={i}
            />
        {:else if i < $index}
            {#if alreadPlayed}
                <div style="width: 92%; opacity: 0.5;" transition:slide>
                    <SongButton
                        renderPhoto={false}
                        onclick={() => {
                            $index = i;
                        }}
                        content={song}
                        index={i}
                    />
                </div>
            {/if}
        {/if}
    {/each}
</main>

<style>
    .alreadyPlayed {
        background: rgba(255, 255, 255, 0.05);
        border: rgba(255, 255, 255, 0.3) solid 1px;

        border-radius: 15px;

        margin-top: 20px;
        margin-bottom: 10px;

        color: white;
        font-size: 15px;
        font-weight: 900;
        padding: 10px;

        cursor: pointer;

    }

    .current {
        margin-top: 50px;
        margin-bottom: 50px;
        width: 92%;
    }

    .current-song-label {
        color: white;
        font-weight: 800;

        font-size: 22px;
    }

    .from {
        color: white;

        font-size: 20px;
        opacity: 0.7;

        font-weight: 700;
    }

    .queue-title {
        display: flex;

        align-items: center;
        justify-self: start;
    }
</style>
