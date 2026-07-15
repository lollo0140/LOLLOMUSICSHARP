<script>
    import { goto, invalidateAll } from "$app/navigation";
    import { onMount } from "svelte";
    import { NavigateTo } from "../../scripts/navigationScript.js";
    import { GetDefPng } from "../../scripts/defPngManager.js";
    import { openContextMenu } from "../../routes/ContextMenu.svelte";

    let { content } = $props();

    let explicitBadge = $state(false);
    let immage = $state();

    onMount(() => {
        immage.src = content.thumbnails[1];
    });

    async function imgError() {
        if (immage.src == content.thumbnails[0]) {
            immage.src = await GetDefPng(content.type);
        } else {
            immage.src = content.thumbnails[0];
        }
    }

    async function onclick() {
        console.log(content.type);

        if (content.type === "playlist") {
            NavigateTo("/playlist", [`browseid=${content.browseId}`]);
        } else if (content.type === "artist") {
            NavigateTo("/artists", [`browseid=${content.browseId}`]);
        } else if (content.type === "album") {
            NavigateTo("/album", [`browseid=${content.browseId}`]);
        }
    }
</script>

<button {onclick} oncontextmenu={ (e) => {
    e.preventDefault();
    openContextMenu(e, content)
}}>
    <div class="Bcontent">
        <img
            bind:this={immage}
            onerror={() => {
                imgError();
            }}
            draggable="false"
            alt=""
        />

        <div
            class="title-strip"
            style="display: flex; flex-direction: row; align-items: center;"
        >
            {#if content.title}
                <p
                    class="title"
                    style="font-size: {22 - content.title.length / 4}px;"
                >
                    {content.title.toUpperCase()}
                </p>
            {:else}
                <p
                    class="title"
                    style="font-size: {22 - content.itemTitle.length / 4}px;"
                >
                    {content.itemTitle.toUpperCase()}
                </p>
            {/if}
        </div>

        <div class="subtitle-div">
            <p class="subtitle">{content.type.toUpperCase()}</p>

            {#if content.artists}

                <p class="dot-divider">•</p>

                <!-- svelte-ignore node_invalid_placement_ssr -->
                <button
                    class="link"
                    onclick={(e) => {
                        e.stopPropagation();
                        NavigateTo("/artists", [
                            `browseid=${content.artists[0].artistId}`,
                        ]);
                    }}
                >
                    {content.artists[0].artistName.toUpperCase()}</button
                >
            {/if}
        </div>
    </div>
</button>

<style>

    .Bcontent {
        background: transparent;

        height: 250px;

        display: flex;
        flex-direction: column;
        gap: 3px;

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    }

    button img {
        height: 160px;
        border-radius: 10px;

        pointer-events: none;
    }

    button {

        position: relative;

        color: white;
        background: none;
        border: none;
        margin: 5px;


        cursor: pointer;
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    }

    button:hover {
        transform: translateY(-4px);
    }

    .title {

        text-align: start;

        margin: 0px;

        font-weight: 900;

        min-height: 25px;
        width: 155px;

        pointer-events: none;
    }

    .dot-divider {
        margin: 0px;

        margin-left: 5px !important;
        margin-right: 5px !important;
    }

    .subtitle-div {

        display: flex;
        align-items: center;

        flex-wrap: wrap;

        font-weight: 700;
        font-size: 15px;

        text-align: start;



        height: 17px;
        width: 155px;

        margin: 0px;

        opacity: 0.5;
    }

    .subtitle-div p {
        width: fit-content;
        margin: 0px;
    }

    .subtitle-div button {

        margin: 0px;

        padding: 0px;
        font-weight: 700;
        font-size: 15px;

        text-wrap: nowrap;
    }

    .subtitle-div button:hover {
        transform: translateY(0px);

        text-decoration: underline;
        font-weight: 900;
    }

</style>
