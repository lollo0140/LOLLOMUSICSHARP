<script module>
    let content = $state(undefined);
    let loading = $state(false);
    let searchKey = $state("");

    // @ts-ignore
    export async function SPageSearch(key) {
        loading = true;
        searchKey = key;

        content = undefined;

        content = JSON.parse(
            // @ts-ignore
            await window.electron.ipcRenderer.lolloInvoke("search", key, "all"),
        );

        // @ts-ignore
        content = content.Result;

        console.log(content);
        loading = false;
    }
</script>

<script>
    import { SetPageButtons } from "../mainscreencomponents/UpperBar.svelte";
    import ResultElement from "./ResultElement.svelte";
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";
    import ResultListElement from "./ResultListElement.svelte";
    import SongListRenderer from "../../svelte_components/reusable/SongListRenderer.svelte";

    import { onMount } from "svelte";
    // @ts-ignore
    import { fade, fly } from "svelte/transition";
    import { goto } from "$app/navigation";

    let activeFilter = $state("all");

    // @ts-ignore
    async function SearchFilter(filter) {
        activeFilter = filter;
        loading = true;
        content = undefined;

        content = JSON.parse(
            // @ts-ignore
            await window.electron.ipcRenderer.lolloInvoke(
                "search",
                searchKey,
                filter,
            ),
        );

        // @ts-ignore
        content = content.Result;

        console.log(content);

        loading = false;
    }

    onMount(() => {
        SetPageButtons([]);
    });

    async function onclick() {
        // @ts-ignore
        if (content.bestResult.type === "artist") {
            // @ts-ignore
            goto(`/artists?browseid=${content.bestResult.browseId}`);
        }
        // @ts-ignore
        if (content.bestResult.type === "album") {
            // @ts-ignore
            goto(`/album?browseid=${content.bestResult.browseId}`);
        }
    }

    //all traks albums artists playlists videos
</script>

<main>
    <p class="page-title">
        {#if content != undefined}{`Results for ${searchKey}`}{:else}Search{/if}
    </p>

    {#if content != undefined}
        <div in:fly={{y:-50}}>
            <div class="filters">
                <button
                    onclick={() => {
                        SearchFilter("all");
                    }}
                    class="filter-button">All results</button
                >
                <button
                    onclick={() => {
                        SearchFilter("traks");
                    }}
                    class="filter-button">Songs</button
                >
                <button
                    onclick={() => {
                        SearchFilter("videos");
                    }}
                    class="filter-button">Videos</button
                >
                <button
                    onclick={() => {
                        SearchFilter("albums");
                    }}
                    class="filter-button">Albums</button
                >
                <button
                    onclick={() => {
                        SearchFilter("artists");
                    }}
                    class="filter-button">Artists</button
                >
                <button
                    onclick={() => {
                        SearchFilter("playlists");
                    }}
                    class="filter-button">Playlists</button
                >
            </div>

            {#if activeFilter === "all"}
                <div>
                    <p class="section-title">Best Result</p>

                    <div class="best-result">
                        <button {onclick} class="best-result-button">
                            <img
                                class="best-result-img"
                                src={content.bestResult.thumbnails[
                                    content.bestResult.thumbnails.length - 1
                                ]}
                                alt=""
                            />
                            <div class="best-result-text">
                                <p class="best-result-title">
                                    {content.bestResult.itemTitle}
                                </p>

                                <div style="display: flex; ">
                                    {#if content.bestResult.artists}
                                        {#each content.bestResult.artists as art}
                                            <a
                                                href={`/artists?browseid=${art.artistId}`}
                                                >{art.artistName}</a
                                            >
                                        {/each}
                                    {/if}

                                    <p class="best-result-subtitle">
                                        {content.bestResult.type}
                                    </p>
                                </div>
                            </div>
                        </button>

                        <div class="best-result-content">
                            {#if content.bestResult.content && content.bestResult.content.length > 1}
                                {#each content.bestResult.content as item}
                                    <ResultListElement content={item} />
                                {/each}
                            {/if}
                        </div>
                    </div>

                    <div>
                        {#each content.sections as section}
                            <ResultElement content={section} />
                        {/each}
                    </div>
                </div>
            {:else if activeFilter === "traks" || activeFilter === "videos"}
                <p class="section-title">{activeFilter}</p>
                <SongListRenderer content={content.results} />
            {:else}
                <p class="section-title">{activeFilter}</p>

                {#each content.results as item}
                    <ResultListElement content={item} />
                {/each}
            {/if}
        </div>
    {:else if loading}
        <LoadingAnimation />
    {:else}
        <p class="suggestion">Search something</p>
    {/if}
</main>

<style>
    a:link {
        color: rgba(255, 255, 255, 0.7);

        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: start;

        margin-left: 4px;
        margin-right: 10px;

        font-size: 20px;

        font-weight: 900;

        height: 100%;
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

    .filter-button {
        border: 1px solid rgba(255, 255, 255, 0.3);
        background: rgba(255, 255, 255, 0.1);

        border-radius: 10px;

        padding: 6px;
        padding-left: 12px;
        padding-right: 12px;

        color: white;
        font-weight: 800;
        font-size: 15px;

        cursor: pointer;
    }

    .filter-button:hover {
        background: rgba(255, 255, 255, 0.2);
    }

    main {
        height: max-content;
    }

    .suggestion {
        margin: 8px;

        color: white;
        font-size: 25px;

        font-weight: 900;

        opacity: 0.7;
    }

    .best-result {
        border-radius: 22px;

        width: calc(100% - 2px);
        height: auto;
    }
    .best-result-button {
        background: transparent;
        border: 1px solid transparent;

        border-radius: 15px;

        display: flex;
        flex-direction: row;

        justify-content: start;
        align-items: center;

        margin: 10px;

        padding: 0px;

        width: calc(100% - 20px);

        cursor: pointer;

        transition: all 200ms;
    }

    .best-result-button:hover {
        border: 1px solid rgba(255, 255, 255, 0.3);
        background: rgba(255, 255, 255, 0.1);
    }

    .best-result-img {
        border: 1px solid rgba(255, 255, 255, 0.3);

        margin: 7px;
        width: 100px;
        border-radius: 10px;
    }
    .best-result-text {
        color: white;

        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: start;

        margin-left: 5px;

        height: 100%;
    }

    .best-result-content {
        margin: 0px;
        margin-bottom: 5px;
    }

    .best-result-title {
        font-size: 40px;
        font-weight: 900;
        margin: 0px;
    }
    .best-result-subtitle {
        font-size: 20px;
        margin: 0px;

        font-weight: 800;

        opacity: 0.7;
    }
    .section-title {
        color: rgba(255, 255, 255, 0.5);
        font-size: 40px;
        font-weight: 900;
    }
</style>
