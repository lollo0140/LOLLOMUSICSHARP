<script>
    import SquareButton from "../../svelte_components/reusable/SquareButton.svelte";
    import { SetPageButtons } from "../mainscreencomponents/UpperBar.svelte";
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";

    import { onMount } from "svelte";
    import { fly } from "svelte/transition";
    import { reloadSidebarList } from "../mainscreencomponents/navigation_bar.svelte";
    import { CreatePlaylist } from "../EditPLaylistMenu.svelte";

    let content = $state(undefined);

    let filter = $state("all");

    async function OnlyPlaylists() {
        content = undefined;
        filter = "playlists";

        const customButtons = [
            {
                text: "Playlists",
                onclick: () => {
                    SetDefButtonsAndContent();
                },
            },
        ];

        content = JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke(
                "getLibraryPlaylists",
            ),
        );
        content = content.Result.items;
        content.splice(0, 1);

        SetPageButtons(customButtons);
    }

    async function OnlyAlbums() {
        content = undefined;
        filter = "albums";

        const customButtons = [
            {
                text: "Albums",
                onclick: () => {
                    SetDefButtonsAndContent();
                },
            },
        ];

        content = JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke("getLibraryAlbums"),
        );
        content = content.Result.items;

        SetPageButtons(customButtons);
    }

    async function OnlyArtists() {
        content = undefined;
        filter = "artists";

        const customButtons = [
            {
                text: "Artists",
                onclick: () => {
                    SetDefButtonsAndContent();
                },
            },
        ];

        content = JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke(
                "getLibrarySubscribed",
            ),
        );
        content = content.Result.items;

        SetPageButtons(customButtons);
    }

    async function OnlySubscribed() {
        content = undefined;
        filter = "subscribed";

        const customButtons = [
            {
                text: "Subscibed",
                onclick: () => {
                    SetDefButtonsAndContent();
                },
            },
        ];

        content = JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke(
                "getLibrarySubscribed",
            ),
        );

        content = content.Result.items;

        SetPageButtons(customButtons);
    }

    async function SetDefButtonsAndContent() {
        content = undefined;
        filter = "all";

        const customButtons = [
            {
                text: "Playlists",
                onclick: () => {
                    OnlyPlaylists();
                },
            },
            {
                text: "Albums",
                onclick: () => {
                    OnlyAlbums();
                },
            },
            {
                text: "Artists",
                onclick: () => {
                    OnlyArtists();
                },
            },
            {
                text: "Subscribed",
                onclick: () => {
                    OnlySubscribed();
                },
            },
        ];

        SetPageButtons(customButtons);

        content = JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke("getLibraryPage"),
        );
        content = content.Result.items;
    }

    export const snapshot = {
        capture: () => {
            return content;
        },
        // 2. Ripristina lo stato quando l'utente torna indietro
        restore: (saved) => {
            content = saved;
        },
    };

    onMount(async () => {
        SetDefButtonsAndContent();
    });
</script>

<p class="page-title">YOUR LIBRARY</p>

<div class="lib-content">
    {#if filter === "all" || filter === "playlists"}
        <button
            class="new-playlist"
            onclick={() => {
                CreatePlaylist();
            }}
        >
            <div><p>+</p></div>
        </button>
    {/if}

    {#if content == undefined}
        <LoadingAnimation />
    {/if}

    {#each content as item, index}
        {#if item.type != "channel"}
            <div in:fly={{ y: 8, delay: 20 * (index + 1) }}>
                <SquareButton content={item} />
            </div>
        {/if}
    {/each}
</div>

<style>
    .new-playlist {
        background: none;
        border: none;

        width: 160px;
        height: 250px;

        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: start;

        margin-left: 10px;
        margin-right: 12px;
        margin-top: 5px;

        cursor: pointer;
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    }

    .new-playlist div {
        color: white;
        font-size: 40px;

        height: 210px;
        width: 155px;

        background: rgba(255, 255, 255, 0);
        border: solid 1px rgba(255, 255, 255, 0.2);

        border-radius: 15px;

        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }

    .new-playlist div:hover {
        background: rgba(255, 255, 255, 0.1);
    }
    .new-playlist:hover {
        transform: translateY(-4px);
    }

    .lib-content {
        margin: 0px;
        height: fit-content;
        width: 100%;

        display: flex;
        flex-wrap: wrap;

        align-items: start;
        gap: 10px;
    }
</style>
