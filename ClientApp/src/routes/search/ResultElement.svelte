<script>
    import ResultListElement from "./ResultListElement.svelte";

    let { content } = $props();
</script>

<p class="section-title">{content.sectionTitle}</p>

{#each content.content as item}
    {#if item.type !== "podcast" && item.type !== "channel" && item.type !== "episode"}
        <ResultListElement content={item} />
    {/if}
{/each}

<style>
    .section-title {
        margin: 0px;

        margin-top: 50px;
        margin-bottom: 10px;

        color: rgba(255, 255, 255, 0.5);
        font-size: 40px;
        font-weight: 900;
    }
</style>
