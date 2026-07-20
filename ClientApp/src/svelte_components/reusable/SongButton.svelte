<script>
    import { onMount } from "svelte";
    import { NavigateTo } from "../../scripts/navigationScript";
    import { GetDefPng } from "../../scripts/defPngManager";
    import { likedSongs, SetVideoLike } from "../../stores/songDataBase.js";
    import { openContextMenu } from "../../routes/ContextMenu.svelte";

    let { onclick, content, index, renderPhoto = true } = $props();

    let Liked = $derived.by(() => {
        if (content === undefined) {
            return false;
        }

        let itemfound = $likedSongs.find((id) => content.id === id);

        if (itemfound != undefined) {
            return true;
        }

        return false;
    });

    let IsLocal = false;

    let imgurl = $state();

    async function SetDefault() {
        imgurl = await GetDefPng("track");
    }

    onMount(() => {
        if (renderPhoto) {
            imgurl = content.thumbnails[0];
        }
    });
</script>

<!-- svelte-ignore a11y_no_static_element_interactions -->
<div
    class="button"
    oncontextmenu={(e) => {
        e.preventDefault();
        openContextMenu(e, content, false);
    }}
>
    <div class="button-content">
        <button
            {onclick}
            class="song-button">.</button
        >

        {#if renderPhoto}
            <img
                class={content.type === "video" ? "videoImg" : ""}
                src={imgurl}
                alt=""
                onerror={() => {
                    SetDefault();
                }}
            />
        {/if}

        <div class="data-strip" style="pointer-events: none;">
            <div
                style="display: flex; flex-direction: row; align-items: center; pointer-events: none;"
            >
                {#if content.explicit}
                    <img
                        class="badge"
                        src="/assets/badge/explicitbadge.png"
                        alt=""
                    />
                {/if}

                <p class="text">
                    {content?.title?.toUpperCase() ??
                        content?.itemTitle?.toUpperCase()}
                </p>
            </div>

            <div class="subtext" style="pointer-events: none;">
                {#each content.artists as art, i}
                    {#if art.artistId}
                        <!-- svelte-ignore node_invalid_placement_ssr -->
                        <button
                            style="pointer-events: all;"
                            class="link"
                            onclick={(e) => {
                                e.stopPropagation();
                                NavigateTo("/artists", [
                                    `browseid=${art.artistId}`,
                                ]);
                            }}
                        >
                            {content.artists[i + 1]
                                ? art?.artistName?.toUpperCase() + ","
                                : art?.artistName?.toUpperCase()}</button
                        >
                    {:else if art.channelId}
                        <p>
                            {art?.channelName?.toUpperCase() ??
                                art?.artistName?.toUpperCase()}
                        </p>
                    {/if}
                {/each}

                {#if content.album}
                    <p style="pointer-events: none;">•</p>
                    <!-- svelte-ignore node_invalid_placement_ssr -->
                    <button
                        class="link"
                        style="pointer-events: all; margin-left: -5px; pointer-events: all;"
                        onclick={(e) => {
                            e.stopPropagation();
                            NavigateTo("/album", [
                                `browseid=${content.album.albumId}`,
                            ]);
                        }}
                    >
                        {content.album.titleName.toUpperCase()}</button
                    >
                {/if}

                <p style="pointer-events: none;">•</p>
                <p style="pointer-events: none;" class="cont-type">
                    {content.type.toUpperCase()}
                </p>
            </div>
        </div>

        <div class="actions-div" style="pointer-events: all;">
            <button
                style="opacity: {Liked ? '1' : '0.3'};"
                onclick={() => {
                    SetVideoLike(content.id, !Liked);
                }}
            >
                <img src="/assets/badge/like.png" alt="" />
            </button>
            <button style="opacity: {IsLocal ? '1' : '0.3'};">
                <img src="/assets/badge/local.png" alt="" />
            </button>
        </div>

        <p style="pointer-events: none;" class="song-index">{index + 1}</p>
    </div>
</div>

<style>
    .actions-div {
        position: absolute;

        display: flex;
        flex-direction: row;
        align-content: center;
        justify-content: space-between;

        right: 70px;

        gap: 5px;
    }

    .actions-div button {
        width: 30px;
        height: 30px;

        background: rebeccapurple;

        margin: 0px;
        padding: 0px;

        border: none;
        background: none;

        display: flex;

        align-items: center;
        align-content: center;
        justify-content: center;

        cursor: pointer;

        transition: all 200ms;
    }

    .actions-div button:hover {
        opacity: 1 !important;
    }

    .actions-div button img {
        width: 22px;
        height: 22px;

        margin: 0px;
        border: none;
        border-radius: 0px;
    }

    .song-button {
        position: absolute;

        left: 0px;
        top: 0px;
        right: 0px;
        bottom: -3px;

        border: none;
        padding: 0px;
        margin: 0px;

        opacity: 0;

        cursor: pointer;
    }

    .link {
        background: none;
        border: none;
        height: 21px;

        color: rgba(255, 255, 255, 1);
        font-weight: 800;

        margin-bottom: -1px;
        margin-left: 0px;
        margin-right: 0px;

        max-width: 170px;
        width: max-content;

        white-space: nowrap;
        white-space: nowrap;

        text-overflow: ellipsis;
        overflow: hidden;

        backdrop-filter: none;
        text-align: start;
        cursor: pointer;
    }

    .link:hover {
        transform: translateX(0px);
        box-shadow: none;

        text-decoration: underline;
    }

    .song-index {
        position: absolute;

        right: 15px;

        font-size: 16px;
        font-weight: 600;
        color: rgba(255, 255, 255, 0.6);
    }

    .badge {
        width: 12px;
        height: 17px;

        margin: 0px;
        margin-right: 5px;

        border: none;
        background: none;
    }

    .button {
        display: flex;

        position: relative;

        border-bottom: 1px solid rgba(255, 255, 255, 0.3);
        background: rgba(255, 255, 255, 0);
        border-radius: 15px;

        width: 100%;
        height: 55px;
        padding: 0px;

        margin-bottom: 5px;

        cursor: pointer;

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .button-content {
        position: relative;

        display: flex;
        flex-direction: row;

        align-items: center;

        width: 100%;
    }

    img {
        margin: 5px;

        margin-top: 5px;
        margin-right: -1px;
        margin-left: 7px;

        width: 40px;
        height: 40px;

        border-radius: 10px;
        border: 1px solid rgba(255, 255, 255, 0.3);
    }

    .data-strip {
        display: flex;
        flex-direction: column;

        align-items: start;

        justify-content: center;

        color: white;

        margin-left: 9px;
    }

    .text {
        display: inline-block; /* Garantisce che max-width venga rispettato */
        max-width: 500px;
        height: 20px;

        overflow: hidden;
        white-space: nowrap; /* Massima compatibilità per l'effetto */
        text-overflow: ellipsis;

        font-size: 17px;
        font-weight: 800;
        margin: 0px;
    }

    .cont-type {
        margin-top: 0px !important;

        font-size: 15px;
        font-weight: 800;

        opacity: 0.55;
    }

    .subtext {
        display: flex;

        align-items: center;
        justify-content: start;
        justify-items: center;

        font-weight: 500;
        font-size: 15px;

        max-width: 600px;

        opacity: 0.7;
    }

    .subtext p {
        color: white;

        margin: 0px;
        margin-right: 5px;
    }

    .button:hover {
        transform: translateX(5px);

        background: rgba(255, 255, 255, 0.1);
        box-shadow: 0px 10px 30px 10px rgba(0, 0, 0, 0.2);

        z-index: 2;
    }

    .videoImg {
        width: 72px;
    }

    a:link {
        color: #ffffff;
        margin-right: 6px;
    }

    /* 2. Stato visitato (link già cliccati in passato) */
    a:visited {
        color: #ffffff;
    }

    /* 3. Stato Hover (quando ci passi sopra con il mouse) */
    a:hover {
        color: #ffffffa7;
    }

    /* 4. Stato Attivo (mentre l'utente ci sta cliccando sopra) */
    a:active {
        color: #e74c3c;
    }
</style>
