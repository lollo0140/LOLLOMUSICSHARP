<script>
    import { onMount } from "svelte";
    import SquareButton from "../svelte_components/reusable/SquareButton.svelte";
    import SongButton from "../svelte_components/reusable/SongButton.svelte";
    import { fly } from "svelte/transition";
    import SongListRenderer from "../svelte_components/reusable/SongListRenderer.svelte";

    let { content } = $props();
    console.log(content);

    let horizontalScroller = $state();
</script>

<div class="section">
    <div class="info-container">
        <div class="info-container-data">
            {#if content.thumbnail}
                <img src={content.thumbnail} alt="" />
            {/if}

            <div class="text">
                <p class="subtitle">{content?.subtitle?.toUpperCase() ?? ""}</p>
                <p class="title">{content?.title?.toUpperCase()}</p>
            </div>
        </div>

        {#if content.displayType == "carousel"}
            <div>
                <button
                    class="scroll-button"
                    onclick={() => {
                        horizontalScroller.scrollBy({ left: -250, behavior: 'smooth' });
                    }}
                >
                    <img
                        style="transform: scaleX(-1);"
                        src="/assets/arrow.png"
                        alt=""
                    />
                </button>

                <button
                    class="scroll-button"
                    onclick={() => {
                        horizontalScroller.scrollBy({ left: +250, behavior: 'smooth' });
                    }}
                >
                    <img src="/assets/arrow.png" alt="" />
                </button>
            </div>
        {/if}
    </div>

    {#if content.displayType == "carousel"}
        <div bind:this={horizontalScroller} class="item-renderer">
            {#each content.items as item, index}
                <div in:fly={{ y: 8, delay: 20 * (index + 1) }}>
                    <SquareButton content={item} />
                </div>
            {/each}
        </div>
    {:else if content.displayType == "itemList"}
        <div class="item-renderer-list">
            <SongListRenderer content={content.items} />
        </div>
    {/if}
</div>

<style>
    .scroll-button {
        width: 40px;
        height: 40px;

        border: 1px solid rgba(255, 255, 255, 0.1);
        background: rgba(255, 255, 255, 0.05);
        border-radius: 30px;

        padding: 0px;
    }

    .scroll-button img {
        height: 15px;
        width: 20px;

        margin-left: 9px;
        margin-top: 5px;

        border: none;
        background: none;

        opacity: 0.3;
    }

    .section {
        margin-bottom: 60px;

        width: 100%;

        display: flex;
        flex-direction: column;
    }

    .info-container-data {
        display: flex;
        height: fit-content;

        align-items: center;
    }

    .info-container {
        display: flex;
        flex-direction: row;

        justify-content: left;
        align-items: center;

        margin: 20px;

        justify-content: space-between;
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
        margin-top: 5px;
        font-size: 48px;
        font-weight: 900;
    }
    .item-renderer {
        margin: 0px;
        height: 270px;
        width: 100%;

        display: flex;
        flex-wrap: nowrap;

        overflow-x: scroll;
        overflow-y: hidden;

        align-items: start;
        gap: 10px;
    }

    .item-renderer::-webkit-scrollbar-thumb {
        background: rgba(245, 222, 179, 0.1);
    }

    .item-renderer::-webkit-scrollbar-track {
        background: rgba(245, 222, 179, 0);
    }

    .item-renderer-list {
        width: 100%;
    }
</style>
