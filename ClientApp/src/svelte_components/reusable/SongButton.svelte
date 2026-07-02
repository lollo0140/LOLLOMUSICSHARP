<script>
    import { onMount } from "svelte";
    import { NavigateTo } from "../../scripts/navigationScript";
    import { GetDefPng } from "../../scripts/defPngManager";

    let { content, index, renderPhoto = true } = $props();

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

<button>
    <div class="button-content">
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

        <div class="data-strip">
            <div
                style="display: flex; flex-direction: row; align-items: center;"
            >
                {#if content.explicit}
                    <img
                        class="badge"
                        src="/assets/badge/explicitbadge.png"
                        alt=""
                    />
                {/if}

                <p class="text">{content.title ?? content.itemTitle}</p>
            </div>

            <div class="subtext">
                {#each content.artists as art}
                    {#if art.artistId}
                        <!-- svelte-ignore node_invalid_placement_ssr -->
                        <button
                            class="link"
                            onclick={(e) => {
                                e.stopPropagation();
                                NavigateTo("/artists", [
                                    `browseid=${art.artistId}`,
                                ]);
                            }}
                        >
                            {art.artistName}</button
                        >
                    {:else if art.channelId}
                        <p>{art.channelName}</p>
                    {/if}
                {/each}

                {#if content.album}
                    <p>•</p>
                    <!-- svelte-ignore node_invalid_placement_ssr -->
                    <button
                        class="link"
                        onclick={(e) => {
                            e.stopPropagation();
                            NavigateTo("/album", [
                                `browseid=${content.album.albumId}`,
                            ]);
                        }}
                    >
                        {content.album.titleName}</button
                    >
                {/if}

                {#if content.type === "album"}
                    <p>•</p>
                    <p>Album</p>
                {/if}

                {#if content.type === "video"}
                    <p>•</p>
                    <p>Video</p>
                {/if}
            </div>
        </div>

        <p class="song-index">{index + 1}</p>
    </div>
</button>

<style>
    .link {
        background: none;
        border: none;
        width: auto;
        height: auto;

        color: rgba(255, 255, 255, 1);
        font-weight: 800;

        margin-bottom: -1px;
        margin-left: 0px;
        margin-right: 5px;



        backdrop-filter: none;
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

    button {
        position: relative;

        border: 1px solid rgba(255, 255, 255, 0.3);
        background: rgba(255, 255, 255, 0.1);
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

        margin-right: -1px;

        width: 41x;
        height: 41px;

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
        font-size: 17px;
        font-weight: 800;
        margin: 0px;
    }

    .subtext {
        display: flex;

        font-weight: 500;
        font-size: 15px;

        opacity: 0.7;
    }

    .subtext p {
        color: white;

        margin: 0px;
        margin-right: 5px;
    }

    button:hover {
        transform: translateX(15px);

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
