<script module>
    import { fade } from "svelte/transition";

    let content = $state();

    let ids = $state([]);

    let menuVisible = $state(false);

    export async function AddToPlaylist(songs) {
        if (Array.isArray(songs) && songs.length > 0) {
            content = JSON.parse(
                await window.electron.ipcRenderer.lolloInvoke(
                    "addToPlaylistMenu",
                ),
            );
            console.log("add to playlist option");
            console.log(content);

            ids = songs;
            menuVisible = true;
        }
    }

    async function AddSongsToTarget(playlistId) {
        const newArray = [...ids].filter(
            (id) => id !== undefined && id !== null,
        );

        await window.electron.ipcRenderer.lolloInvoke(
            "addToplaylist",
            newArray,
            playlistId,
        );

        ids = [];
        menuVisible = false;
    }
</script>

{#if menuVisible}
    <main transition:fade={{ duration: 300 }}>
        <div class="add-to-playlist">
            {#if content != null}
                <div class="all-playlist">
                    <p class="secTitle">ADD TO PLAYLIST</p>

                    <div class="pl-list-vertical">
                        {#each content.content.playlists as P}
                            <button
                                class="playlist-button"
                                onclick={() => {
                                    AddSongsToTarget(P.browseId);
                                }}
                            >
                                <img src={P.thumbnails[0]} alt="" />

                                <div>
                                    <p class="title">{P.title.toUpperCase()}</p>
                                </div>
                            </button>
                        {/each}
                    </div>
                </div>
            {/if}
        </div>

        <button
            class="close-button"
            onclick={() => {
                ids = [];
                menuVisible = false;
            }}
        >
            .
        </button>
    </main>
{/if}

<style>
    .close-button {
        width: 100%;
        height: 100%;

        background: none;
        border: none;

        color: transparent;
    }

    .secTitle {
        font-size: 35px;
        font-weight: 900;
        margin: 0px;

        margin-top: 10px;

        margin-bottom: 30px;
    }

    /* USER ITEMS --------------- */

    .all-playlist {
        display: flex;
        flex-direction: column;



    }

    .pl-list-vertical {
        max-height: 350px;
        overflow-x: hidden;
        overflow-y: auto;

        display: flex;
        flex-direction: column;

        gap: 10px;

        width: 100%;
    }

    .playlist-button {
        width: 100%;
        height: 50px;

        display: flex;

        align-items: center;
        justify-content: start;

        padding: 0px;

        cursor: pointer;

        border: none;
        background: none;
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .playlist-button:hover {
        transform: translateX(8px);
    }

    .playlist-button img {
        height: 50px;
        width: 50px;
        object-fit: cover;
        border-radius: 7px;
    }

    .playlist-button div {
        color: white;
        display: flex;
        flex-direction: column;
        align-items: start;
        justify-content: center;


        font-weight: 800;

        margin-left: 10px;
    }

    .playlist-button div p {
        margin: 0px;
    }

    .title {
        font-size: 20px;
    }


    /* RECENT ITEMS --------------- */

    .add-to-playlist {
        padding: 15px;

        background: black;
        border: rgba(255, 255, 255, 0.3) 1px solid;
        border-radius: 20px;

        position: absolute;

        width: 600px;
        height: fit-content;

        left: 50%;
        top: 50%;
        transform: translate(-50%, -50%);

        color: white;
    }

    main {
        position: fixed;
        z-index: 3;

        background: rgba(0, 0, 0, 0.758);

        backdrop-filter: blur(3px);

        border-radius: 39px;

        width: 100%;
        height: 100%;
    }
</style>
