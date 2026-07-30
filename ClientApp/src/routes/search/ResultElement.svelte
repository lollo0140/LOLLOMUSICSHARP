<script>
    import { SetCurrentPlaylist } from "../audioPlayer/playerStore";
    import ResultListElement from "./ResultListElement.svelte";

    let { content, from = "" } = $props();
</script>

<p class="section-title">{content.sectionTitle.toUpperCase()}</p>

{#each content.content as item}
    {#if item.type !== "podcast" && item.type !== "channel" && item.type !== "episode"}
        <div>
            <ResultListElement
                content={item}
                onclick={() => SetCurrentPlaylist([item], 0, from)}
            />
        </div>
    {/if}
{/each}

<style>
    .section-title {
        margin: 0px;

        margin-top: 70px;
        margin-bottom: 30px;

        color: rgba(255, 255, 255, 0.5);
        font-size: 40px;
        font-weight: 900;
    }
</style>
