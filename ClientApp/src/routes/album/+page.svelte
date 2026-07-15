<script>
    import { SetPageButtons } from "../mainscreencomponents/UpperBar.svelte";
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";

    import { page } from "$app/state";
    import { onMount } from "svelte";
    import { fly } from "svelte/transition";
    import PageHeader from "../../svelte_components/reusable/PageHeader.svelte";
    import { NavigateToArtist } from "../../scripts/navigationScript";
    import SongListRenderer from "../../svelte_components/reusable/SongListRenderer.svelte";
    import { SetPlaylistSave } from "../../scripts/savedElements";
    import { GetAlbumPage } from "../../scripts/browser";

    let quary = $derived(page.url.searchParams.get("browseid"));
    let content = $state(undefined);

    let shareButtonText = $state(false);

    export const snapshot = {
        capture: () => {
            return content;
        },
        restore: (saved) => {
            content = saved;
        },
    };

    onMount(async () => {
        SetPageButtons([]);
        console.log("loading album: ", quary);
        content = await GetAlbumPage(quary);

        console.log(content);
    });
</script>

{#if content != undefined}
    <main in:fly={{ y: -50 }}>
        <PageHeader
            bgImmage={content.data.thumbnails[
                content.data.thumbnails.length - 1
            ]}
            label={content.data.subtitle.split("\u2022")[0].toUpperCase()}
        >
            {#if content?.data?.title}
                <p class="PL-title">
                    {(content.data.title ?? "").toUpperCase()}
                </p>
            {/if}

            {#if content?.data?.artist}
                <button
                    class="PL-artist"
                    onclick={() => {
                        NavigateToArtist(content?.data?.artist.browseId);
                    }}
                >
                    {content?.data?.artist?.name}
                </button>
            {/if}

            {#if content?.data?.description}
                <p class="PL-desc">
                    {(content.data.description ?? "").toUpperCase()}
                </p>
            {/if}

            <div class="PL-subtitles">
                <p>
                    {content.data.subtitle.replace("EP \u2022 ", "").replace("Album \u2022 ", "")}
                </p>
                <p>{content.data.secondSubtitle}</p>
            </div>

            <div class="PL-actions">
                <button
                    style={content.data.saved ? "opacity: 1;" : "opacity: 0.3;"}
                    onclick={() => {
                        content.data.saved = !content.data.saved;
                        SetPlaylistSave(
                            content.data.saveParam,
                            content.data.saved,
                        );
                    }}
                >
                    <img src="./assets/buttons/bookmark.png" alt="" />
                </button>

                <button
                    onclick={navigator.clipboard
                        .writeText(content.data.shareLink)
                        .then(() => {
                            shareButtonText = true;

                            setTimeout(() => {
                                shareButtonText = false;
                            }, 1300);
                        })}
                >
                    <img src="./assets/buttons/share.png" alt="" />

                    {#if shareButtonText}
                        <p transition:fly={{ x: -10 }}>Share link copied!</p>
                    {/if}
                </button>

                <button>
                    <img src="./assets/buttons/download.png" alt="" />
                </button>

                <button>
                    <img src="./assets/buttons/more_options.png" alt="" />
                </button>
            </div>
        </PageHeader>

        <div style="margin-top: 5px;">
            <SongListRenderer
                content={content.items ?? []}
                renderPhoto={false}
            />
        </div>
    </main>
{:else}
    <div style="height: min-content;">
        <LoadingAnimation />
    </div>
{/if}

<style>
    .PL-artist {
        padding: 0px;
        background: transparent;
        border: none;

        color: white;
        font-weight: 800;
        font-size: 37px;
        opacity: 0.7;

        margin-top: -20px;
        margin-bottom: 20px;

        cursor: pointer;
    }

    .PL-artist:hover {
        text-decoration: underline;
    }

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

    .PL-actions button:hover {
        transform: translateY(-2px);
    }

    .PL-actions button img {
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
</style>
