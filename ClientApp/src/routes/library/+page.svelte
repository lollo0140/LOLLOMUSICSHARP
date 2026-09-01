<script module>
    import SquareButton from "../../svelte_components/reusable/SquareButton.svelte";
    import { SetPageButtons } from "../mainscreencomponents/UpperBar.svelte";
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";

    import { onMount } from "svelte";
    import { fly } from "svelte/transition";
    import { reloadSidebarList } from "../mainscreencomponents/navigation_bar.svelte";
    import { CreatePlaylist } from "../EditPLaylistMenu.svelte";
    import { derived } from "svelte/store";
    import { EInvokeJSON } from "../../scripts/electronInvoker";
    import DownloadedSquareButton from "../../svelte_components/single/DownloadedSquareButton.svelte";

    let content = $state(undefined);

    async function LoadPage() {
        const [playlists, albums, subscribed] = await Promise.all([
            EInvokeJSON("getLibraryPlaylists"),
            EInvokeJSON("getLibraryAlbums"),
            EInvokeJSON("getLibrarySubscribed"),
        ]);

        content = {
            playlists,
            albums,
            subscribed,
        };
    }

    export async function ReloadLibrary() {
        setTimeout(async () => {
            LoadPage();
        }, 1000);
    }
</script>

<script>
    export const snapshot = {
        capture: () => {
            return content;
        },
        restore: (saved) => {
            content = saved;
        },
    };

    let showDownloadedButton = $state();

    onMount(async () => {
        await LoadPage();

        //SetDefButtonsAndContent();
        const _ = (await EInvokeJSON("getDownloaded")) ?? [];
        console.log("downloaded ids");
        console.log(_);
        showDownloadedButton = _.length > 0;
    });
</script>

<main>
    <p class="page-title">YOUR LIBRARY</p>

    <div class="lib-content">
        {#if content == undefined || content == []}
            <LoadingAnimation />
        {/if}

        {#if content != undefined}
            <p class="label">PLAYLISTS</p>

            <div class="content-part">
                {#if showDownloadedButton}
                    <DownloadedSquareButton />
                {/if}

                {#each content.playlists.Result.items as p}
                    {#if p.type != "none"}
                        <SquareButton content={p} />
                    {/if}
                {/each}

                <button
                    class="new-playlist"
                    onclick={() => {
                        CreatePlaylist();
                    }}
                >
                    <div><p>+</p></div>
                </button>
            </div>

            {#if content.albums.Result.items.length > 0}
                <p class="label">ALBUMS</p>

                <div class="content-part">
                    {#each content.albums.Result.items as a}
                        <SquareButton content={a} />
                    {/each}
                </div>
            {/if}

            {#if content.subscribed.Result.items.length > 0}
                <p class="label">SUBSCRIBED</p>

                <div class="content-part">
                    {#each content.subscribed.Result.items as s}
                        {#if s.type != "channel"}
                            <SquareButton content={s} />
                        {/if}
                    {/each}
                </div>
            {/if}
        {/if}
    </div>

    <div class="spacer"></div>

</main>

<style>
    main {
        position: absolute;
        left: 0px;
        top: 0px;
        bottom: 0px;
        right: 0px;

        overflow-y: scroll;
        overflow-x: hidden;
    }

    .content-part {
        display: flex;
        flex-wrap: wrap;
    }

    .label {
        color: white;
        font-size: 40px;
        opacity: 0.7;
        font-weight: 800;

        margin: 20px;
    }

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

        flex-direction: column;

        align-items: start;
        gap: 10px;
    }
</style>
