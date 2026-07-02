<script>
    import { onMount } from "svelte";
    import { NavigateTo } from "../../scripts/navigationScript.js";
    import NavBarPButton from "./navBarPButton.svelte";
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";

    let playlist = $state();
    let playlistLoading = $state(false);

    onMount(async () => {
        let content = JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke(
                "getLibraryPlaylists",
            ),
        );
        content = content.Result.items;
        content.splice(0, 1);

        playlist = content;

        console.log(playlist);

        playlistLoading = false;
    });
</script>

<main>
    <div class="page-button-container">
        <button class="page-button" onclick={() => NavigateTo("/")}>
            <img src="/assets/navbar/home.png" alt="" />
        </button>
        <button class="page-button" onclick={() => NavigateTo("/search")}>
            <img src="/assets/navbar/search.png" alt="" />
        </button>
        <button class="page-button" onclick={() => NavigateTo("/library")}>
            <img src="/assets/navbar/library.png" alt="" />
        </button>
        <button class="page-button" onclick={() => NavigateTo("/localfiles")}>
            <img src="/assets/navbar/folder.png" alt="" />
        </button>
    </div>

    <div class="divider"></div>

    <div class="library-elements">
        {#if !playlistLoading}
            {#each playlist as P}
                <NavBarPButton content={P} />
            {/each}
        {:else}
            <LoadingAnimation />
        {/if}
    </div>
</main>

<style>
    .library-elements {

        margin-top: 15px;

        height: 400px;
        overflow-y: scroll;
    }

    .library-elements::-webkit-scrollbar {
        display: none;
    }

    .divider {
        height: 3px;
        width: 30%;

        background: rgb(255, 255, 255);
        opacity: 0.4;

        border-radius: 10px;

        margin-top: 10px;
        margin-bottom: 10px;
    }

    .page-button {
        width: 55px;
        height: 55px;

        padding: 0px;

        background: transparent;
        border: none;

        overflow: hidden;

        opacity: 0.7;

        cursor: pointer;
    }

    .page-button img {
        width: 30px;
        height: 30px;
    }

    .page-button:hover {
        opacity: 1;
    }

    .page-button-container {
        display: flex;
        flex-direction: column;
    }

    main {
        position: relative;

        left: 0px;
        right: 0px;
        bottom: 0px;
        top: 0px;

        display: flex;
        flex-direction: column;

        align-items: center;
        justify-content: space-between;
    }
</style>
