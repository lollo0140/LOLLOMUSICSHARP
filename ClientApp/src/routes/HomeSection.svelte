<script>
    import { onMount } from "svelte";
    import SquareButton from "../svelte_components/reusable/SquareButton.svelte";
    import SongButton from "../svelte_components/reusable/SongButton.svelte";
    import { fly } from "svelte/transition";
    import SongListRenderer from "../svelte_components/reusable/SongListRenderer.svelte";

    let { content } = $props();
    console.log(content);

</script>

<div class="section">
    <div class="info-container">
        {#if content.thumbnail}
            <img src={content.thumbnail} alt="" />
        {/if}

        <div class="text">
            <p class="subtitle">{content.subtitle ?? ""}</p>
            <p class="title">{content.title}</p>
        </div>
    </div>

    {#if content.displayType == "carousel"}
        <div class="item-renderer">
            {#each content.items as item, index}
                <div in:fly={{ y: 8, delay: 20 * (index + 1) }}>
                    <SquareButton content={item} />
                </div>
            {/each}
        </div>
    {:else if content.displayType == "itemList"}
        <div class="item-renderer-list">

            <SongListRenderer content={content.items}/>
        </div>
    {/if}
</div>

<style>
    .section {
        margin-bottom: 60px;

        width: 100%;

        display: flex;
        flex-direction: column;
    }
    .info-container {
        display: flex;
        flex-direction: row;

        justify-content: left;
        align-items: center;

        margin: 20px;
    }
    .text {
        display: flex;
        flex-direction: column;

        font-size: 20px;

        color: white;
    }
    img {
        width: 60px;
        height: 60px;

        border-radius: 50px;
        border: 1px solid rgba(255, 255, 255, 0.3);

        margin-right: 30px;
    }
    .subtitle {
        font-weight: 900;
        opacity: 0.7;
        margin: 0px;
        margin-bottom: -10px;
    }
    .title {
        margin: 0px;
        font-size: 64px;
        font-weight: 900;
    }
    .item-renderer {
        margin: 0px;
        height: fit-content;
        width: 100%;

        display: flex;
        flex-wrap: wrap;

        align-items: start;
        gap: 10px;
    }

    .item-renderer-list {
        width: 100%;
    }
</style>
