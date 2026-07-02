<script>
    import SquareButton from "../../svelte_components/reusable/SquareButton.svelte";
    import { SetPageButtons } from "../mainscreencomponents/UpperBar.svelte";
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";

    import { onMount } from "svelte";
    import { fly } from "svelte/transition";

    let content = $state(undefined);

    async function OnlyPlaylists() {
        content = undefined;

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
        content.splice(0,1)

        SetPageButtons(customButtons);
    }

    async function OnlyAlbums() {
        content = undefined;

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

<p class="page-title">Your Library</p>

<div class="lib-content">
    {#if content == undefined}
        <LoadingAnimation />
    {/if}

    {#each content as item, index}
        <div in:fly={{ y: 8, delay: 20 * (index + 1) }}>
            <SquareButton content={item} />
        </div>
    {/each}
</div>

<style>
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
