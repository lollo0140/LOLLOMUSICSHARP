<script>
    import { onMount } from "svelte";
    import SongButton from "./SongButton.svelte";
    import { SetCurrentPlaylist } from "../../routes/audioPlayer/playerStore";
    import { derived } from "svelte/store";

    let {
        content,
        renderPhoto = true,
        from = "",
        playlistId = undefined,
        scroll = undefined,
    } = $props();

    onMount(() => {
        console.log(renderPhoto);
    });

    let container = $state();

    let H = $derived.by(() => {
        return container?.clientHeight;
    });

    let itemHeight = 60;

    let showedItem = $derived.by(() => {
        if (scroll != undefined) {
            return Math.floor(
                scroll / itemHeight + window.innerHeight / itemHeight,
            );
        }
        return 0;
    });

    let hiddenBehind = $derived.by(() => {
        if (scroll != undefined) {
            return Math.ceil(scroll / itemHeight) - 2;
        }
        return 0;
    });
</script>

<div class="list-renderer" bind:this={container}>
    {#each content as item, index}
        <div class="place-holder">
            {#if scroll === undefined || (index > hiddenBehind && index <= showedItem)}
                <SongButton
                    onclick={() => {
                        SetCurrentPlaylist(content, index, from);
                    }}
                    content={item}
                    {index}
                    {renderPhoto}
                    fatherId={playlistId}
                />
            {/if}
        </div>
    {/each}
</div>

<style>
    .list-renderer {
        position: relative;

        display: flex;
        flex-direction: column;

        gap: 5px;

        width: 100%;
    }

    .place-holder {
        height: 55px;
    }
</style>
