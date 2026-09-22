<script>
    import { page } from "$app/state";
    import { onMount } from "svelte";
    import { fly } from "svelte/transition";
    import { openPageContextMenu } from "../../../routes/ContextMenu.svelte";
    import { render } from "svelte/server";
    import LoadingAnimation from "../LoadingAnimation.svelte";
    import SongListRenderer from "../SongListRenderer.svelte";
    import PageHeader from "../PageHeader.svelte";
    import {
        SetCurrentPlaylist,
        toggleShuffleMode,
    } from "../../../routes/audioPlayer/playerStore";

    let { children, content, playlistId = undefined } = $props();

    let searchFilter = $state(undefined);

    let pageScroll = $state(0);
    let mainElement = $state(null);

    let headerHeight = $derived(
        document.getElementById("header-content").clientHeight,
    );

    function handleScroll(e) {
        pageScroll = e.currentTarget.scrollTop - headerHeight;
    }
</script>

{#if content != undefined}
    <main bind:this={mainElement} onscroll={handleScroll} in:fly={{ y: -50 }}>
        <div id="header-content">
            <PageHeader
                bgImmage={content.data.thumbnails[
                    content.data.thumbnails.length - 1
                ]}
                label={content?.data?.saveParam != undefined
                    ? "ALBUM"
                    : "PLAYLIST"}
            >
                {@render children()}
            </PageHeader>
        </div>

        <div class="quick-actions">
            <button
                class="play-def"
                onclick={() => {
                    SetCurrentPlaylist(content.items, 0, content.data.title);
                }}
            >
                <img
                    src="/assets/controlls/play.png"
                    alt=""
                    draggable="false"
                />
            </button>

            {#if content?.data?.saveParam === undefined}
                <button
                    class="play-shuffled"
                    onclick={() => {
                        const randInt = Math.floor(
                            Math.random() * content?.items?.length,
                        );
                        SetCurrentPlaylist(
                            content.items,
                            randInt,
                            content.data.title,
                        );
                        toggleShuffleMode(true);
                    }}
                >
                    <img
                        src="/assets/controlls/shuffle.png"
                        alt=""
                        draggable="false"
                    />
                </button>
            {/if}

            <input
                placeholder="SEARCH"
                class="title-search"
                type="text"
                bind:value={searchFilter}
            />
        </div>

        <div class="PL-elements">
            <SongListRenderer
                scroll={pageScroll}
                content={content.items}
                {playlistId}
                from={(content.data.title ?? "").toUpperCase()}
                searchFilter={searchFilter != "" ? searchFilter : undefined}
            />
        </div>

        <div class="spacer"></div>
    </main>
{:else}
    <div style="height: min-content;">
        <LoadingAnimation />
    </div>
{/if}

<style>
    .title-search {
        border: none;
        border-bottom: 2px solid rgba(255, 255, 255, 0.3);
        background: none;

        font-size: 17px;

        height: 23px;

        color: white;

        margin-left: 25px;
    }

    .quick-actions {
        height: 60px;
        width: min-content;

        display: flex;
        align-items: center;
        justify-content: center;

        gap: 10px;

        padding-left: 10px;
        padding-right: 10px;
        margin: 15px;

        border-radius: 15px;

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .quick-actions button {
        background: none;
        border: none;
        cursor: pointer;

        padding: 0px;
        margin: 0px;

        margin-top: 3px;

        opacity: 0.5;
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .quick-actions button img {
        width: 40px;
    }

    .quick-actions button:hover {
        transform: scale(1.1);
        opacity: 1;
    }

    .PL-elements {
        margin-top: 5px;
    }

    main {
        height: 100%;
        overflow-y: scroll;
        scrollbar-width: none !important;
        overflow-x: hidden;
    }
</style>
