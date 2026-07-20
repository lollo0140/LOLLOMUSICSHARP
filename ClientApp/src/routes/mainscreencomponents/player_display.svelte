<script>
    import {
        NavigateToAlbum,
        NavigateToArtist,
    } from "../../scripts/navigationScript";
    import { queue, index, playState } from "../audioPlayer/playerStore.js";

    let currentSong = $derived.by(() => {
        return $queue[$index] ?? undefined;
    });
</script>

{#if currentSong != undefined}
    <main>
        <div class="current">
            <img class="currentImg" src={currentSong.thumbnails[1].replace("=w120-h120-l90-rj", "=w512-h512-l90-rj")} alt="" />
            <div class="currentInfo">
                <p class="currentTitile">{currentSong.title.toUpperCase()}</p>
                <div class="currentArtist">
                    {#each currentSong.artists as A, i}
                        <button
                            class="artButton"
                            onclick={() => {
                                NavigateToArtist(A.artistId);
                            }}
                            >{currentSong.artists[i + 1] != undefined
                                ? A.artistName.toUpperCase() + ","
                                : A.artistName.toUpperCase()}</button
                        >
                    {/each}
                </div>
                <button
                    onclick={() => {
                        NavigateToAlbum(currentSong.album.albumId);
                    }}
                    class="albButton"
                >
                {currentSong.album.titleName.toUpperCase()}
                </button>
            </div>
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
        width: 100%;
        border-radius: 15px;

        border: 1px solid rgba(255, 255, 255, 0.1);
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
    }

    .artButton:hover, .albButton:hover {
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
