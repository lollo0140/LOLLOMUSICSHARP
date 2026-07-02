<script>
    import { goto, invalidateAll } from "$app/navigation";
    import { onMount } from "svelte";
    import { NavigateTo } from "../../scripts/navigationScript.js";
    import { GetDefPng } from "../../scripts/defPngManager.js";

    let { content } = $props();

    let explicitBadge = $state(false);
    let immage = $state();

    onMount(() => {
        immage.src = content.thumbnails[1];
    });

    async function imgError() {
        if (immage.src == content.thumbnails[0]) {
            immage.src = await GetDefPng(content.type)
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

<button {onclick}>
    <div>
        <img
            bind:this={immage}
            onerror={() => {
                imgError();
            }}
            alt=""
            draggable="false"
        />

        <div
            class="title-strip"
            style="display: flex; flex-direction: row; align-items: center;"
        >
            {#if content.title}
                <p
                    class="title"
                    style="font-size: {20 - content.title.length / 3}px;"
                >
                    {content.title}
                </p>
            {:else}
                <p
                    class="title"
                    style="font-size: {20 - content.itemTitle.length / 3}px;"
                >
                    {content.itemTitle}
                </p>
            {/if}
        </div>

        {#if content.artists}
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
                {content.artists[0].artistName}</button
            >
        {:else}
            <p class="subtitle">{content.type}</p>
        {/if}
    </div>
</button>

<style>
    .link {
        background: none;
        border: none;
        width: auto;
        height: auto;

        color: rgba(255, 255, 255, 0.7);
        font-weight: 800;
        margin-left: 5px;

        font-size: 15px;

        backdrop-filter: none;
    }

    .link:hover {
        transform: translateX(0px);
        box-shadow: none;

        text-decoration: underline;
    }

    .title-strip {
        height: 14px;

        margin: 0px;
        margin-bottom: 8px;
    }


    .title {
        margin-top: 0px;
        margin-bottom: 0px;

        font-weight: 700;

        width: 100%;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;

        line-height: 25px;
    }

    .subtitle {
        margin-top: 0px;
        margin-left: 4px;

        width: 100%;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;

        font-weight: 700;

        font-size: 15px;

        opacity: 0.7;
    }

    button {
        position: relative;

        border: 1px solid rgba(255, 255, 255, 0.3);
        background: rgba(255, 255, 255, 0.1);

        padding: 0px;

        width: 160px;
        height: 200px;

        display: flex;
        flex-direction: column;
        justify-content: start;

        border-radius: 12px;

        text-align: left;
        color: white;

        cursor: pointer;
        backdrop-filter: brightness(0.6);
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    button:hover {
        transform: translateY(-20px);
        box-shadow: 0px 10px 30px 10px rgba(0, 0, 0, 0.491);

        z-index: 2;
    }

    button div {
        position: relative;

        left: 4px;
        top: 4px;

        width: calc(100% - 8px);
        height: calc(100% - 8px);
    }

    img {
        position: relative;

        border-radius: 7px;

        width: calc(100% - 2px);
        height: 138px;

        object-fit: cover;

        border: 1px solid rgba(255, 255, 255, 0.3);
    }
</style>
