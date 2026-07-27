<script>
    import { GetDefPng } from "../../scripts/defPngManager";
    import {
        NavigateToAlbum,
        NavigateToArtist,
    } from "../../scripts/navigationScript";
    import NextUpPannel from "../../svelte_components/single/NextUpPannel.svelte";
    import { queue, index, playState } from "../audioPlayer/playerStore.js";

    let currentSongImmage = $derived.by(() => {
        if (!currentSong?.thumbnails || currentSong.thumbnails.length === 0) {
            return "/assets/defpng/def_song_icon.png";
        }

        if (currentSong.type === "none") {
            return "/assets/defpng/def_song_icon.png";
        }

        const targetThumb =
            currentSong.thumbnails[1] ?? currentSong.thumbnails[0];

        if (typeof targetThumb === "string") {
            return targetThumb.replace(
                "=w120-h120-l90-rj",
                "=w512-h512-l90-rj",
            );
        }

        if (targetThumb?.url) {
            return targetThumb.url.replace(
                "=w120-h120-l90-rj",
                "=w512-h512-l90-rj",
            );
        }

        return "/assets/defpng/def_song_icon.png";
    });

    let currentSong = $derived.by(() => {
        return $queue[$index] ?? undefined;
    });
</script>

{#if currentSong != undefined}
    <main>
        <div class="current">
            <img
                class="currentImg"
                onerror={(e) => {
                    currentSongImmage = "/assets/defpng/def_song_icon.png";
                }}
                src={currentSongImmage}
                alt=""
            />
            <div class="currentInfo">
                <p class="currentTitile">{currentSong.title.toUpperCase()}</p>
                <div class="currentArtist">
                    {#each currentSong.artists ?? [] as A, i}
                        {#if A?.artistId}
                            <button
                                class="artButton"
                                onclick={() => {
                                    NavigateToArtist(A.artistId);
                                }}
                                >{currentSong.artists[i + 1] != undefined
                                    ? A.artistName.toUpperCase() + ","
                                    : A.artistName.toUpperCase()}</button
                            >
                        {:else if A?.channelId}
                            <button class="artButton"
                                >{currentSong.artists[i + 1] != undefined
                                    ? A.channelName.toUpperCase() + ","
                                    : A.channelName.toUpperCase()}</button
                            >
                        {/if}
                    {/each}
                </div>

                {#if currentSong?.album?.albumId}
                    <button
                        onclick={() => {
                            NavigateToAlbum(currentSong.album.albumId);
                        }}
                        class="albButton"
                    >
                        {currentSong.album.titleName.toUpperCase()}
                    </button>
                {/if}
            </div>
        </div>

        <div style="padding: 10px;">
            <NextUpPannel />
        </div>
    </main>
{/if}

<style>
    .current {
        display: flex;
        flex-direction: column;
        gap: 10px;
        padding: 10px;
    }

    .currentInfo {
        color: white;

        display: flex;
        flex-direction: column;
        gap: 10px;
    }

    .currentImg {
        width: 284px;
        height: 288px;

        border-radius: 15px;

        border: 1px solid rgba(255, 255, 255, 0.1);

        object-fit: cover;
    }

    .currentTitile {
        font-size: 20px;
        font-weight: 800;

        margin: 0px;
    }

    .currentArtist {
        display: flex;
        flex-direction: row;
        gap: 6px;
    }

    .artButton {
        padding: 0px;
        margin: 0px;
        background: none;
        border: none;
        color: white;
        font-size: 17px;
        opacity: 0.7;
        font-weight: 800;

        cursor: pointer;
    }

    .albButton {
        padding: 0px;
        margin: 0px;
        background: none;
        border: none;
        color: white;
        font-size: 17px;
        opacity: 0.5;
        font-weight: 700;

        cursor: pointer;

        text-align: start;
    }

    .artButton:hover,
    .albButton:hover {
        text-decoration: underline;
    }

    main {
        display: flex;
        flex-direction: column;
    }

    .currentInfo {
        display: flex;
        flex-direction: column;

        align-items: start;
        justify-content: center;
    }
</style>
