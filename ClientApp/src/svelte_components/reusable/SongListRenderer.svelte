<script>
    import { onMount } from "svelte";
    import SongButton from "./SongButton.svelte";

    let { content, renderPhoto= true } = $props();

    onMount( () =>
    {
        console.log(renderPhoto);

    })

</script>

<div class="list-renderer">
    {#each content as item, index}
        <SongButton content={item} {index} renderPhoto={renderPhoto}/>
    {/each}
</div>

<style>
    .list-renderer {
        display: flex;
        flex-direction: column;

        width: 100%;
    }
</style>
