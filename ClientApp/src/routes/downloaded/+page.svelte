<script>
    import { onMount } from "svelte";
    import { EInvokeJSON } from "../../scripts/electronInvoker";
    import { setLocalSongs } from "../../stores/songDataBase";
    import PageHeader from "../../svelte_components/reusable/PageHeader.svelte";
    import SongListRenderer from "../../svelte_components/reusable/SongListRenderer.svelte";
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";
    import { fly } from "svelte/transition";

    let content = $state();

    onMount(async () => {
        setLocalSongs();
        content = await EInvokeJSON("getDownloaded");
    });
</script>

{#if content != undefined}
    <main in:fly={{ y: -50 }}>
        <PageHeader
            bgImmage={"/assets/defpng/def_playlist_icon.png"}
            label={"PLAYLIST"}
        >
            <p class="PL-title">DOWNLOADED</p>

            <p class="PL-desc">DOWNLOADED SONGS</p>

            <div class="PL-subtitles">
                <p> {`${content.length} ${ content.length > 1 ? "songs" : "song"}  \u2022`}  AUTOMATIC PLAYLIST</p>
            </div>

            <div class="facepile">
                <div>
                    <img class="profile-icon" src="/Icon.png" alt="" />
                </div>
            </div>

            <div class="PL-actions">
                <button
                    class="page-menu"
                    onclick={(e) => {
                        openPageContextMenu(e, content, "playlist");
                    }}
                >
                    <img src="./assets/buttons/more_options.png" alt="" />
                </button>
            </div>
        </PageHeader>

        <div class="PL-elements">
            <SongListRenderer
                {content}
                playlistId={undefined}
                from={"DOWNLOADED"}
            />
        </div>
    </main>
{:else}
    <div style="height: min-content;">
        <LoadingAnimation />
    </div>
{/if}

<style>
    .PL-actions {
        height: 56px;
        min-width: 100px;

        border-top: 1px solid rgba(255, 255, 255, 0.3);
        border-bottom: 1px solid rgba(255, 255, 255, 0.3);

        margin-top: 20px;
        margin-bottom: 20px;

        display: flex;
        align-items: center;
        justify-content: center;

        padding-left: 5px;
        padding-right: 5px;

        gap: 15px;
    }

    .PL-actions button {
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;

        gap: 10px;

        width: fit-content;
        height: 43px;

        padding: 0px;

        color: white;

        background: none;
        border: none;

        cursor: pointer;

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;

        margin-left: 3px;
        margin-right: 3px;
    }

    .PL-elements {
        margin-top: 5px;
    }

    main {
        overflow: hidden;
    }

    .PL-title {
        font-size: 64px;
        font-weight: 900;
        color: white;

        margin-top: 20px;
        margin-bottom: 20px;

        max-width: 600px;
    }
    .PL-desc {
        font-size: 20px;
        font-weight: 800;
        color: white;
        opacity: 0.6;

        margin: 0px;

        margin-top: 20px;
        margin-bottom: 20px;

        max-width: 340px;
    }
    .PL-subtitles {
        display: flex;
        flex-direction: column;

        margin-top: 20px;
        margin-bottom: 20px;
    }

    .PL-subtitles p {
        margin: 0px;
        margin-top: 3px;
        margin-bottom: 3px;

        color: white;

        opacity: 0.6;

        font-weight: 700;
    }

    .facepile {
        position: absolute;

        display: flex;
        flex-direction: row;

        align-items: center;
        justify-content: start;

        top: 23px;
        left: 165px;
    }

    .profile-icon {
        width: 30px;

        border-radius: 20px;
        border: 1px rgba(255, 255, 255, 0.1) solid;
    }
</style>
