<script>
    import { fly } from "svelte/transition";
    import SongButton from "../../svelte_components/reusable/SongButton.svelte";

    import { queue, index, from } from "../audioPlayer/playerStore.js";
</script>

<main in:fly={{ y: -20 }}>
    <div class="queue-title">
        <p class="page-title">QUEUE</p>
        <p class="from">{$from.toUpperCase()}</p>
    </div>

    {#each $queue as song, i}
        <SongButton
            onclick={() => {
                $index = i;
            }}
            renderPhoto={false}
            content={song}
            index={i}
        />
    {/each}
</main>

<style>
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
