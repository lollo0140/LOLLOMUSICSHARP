<script>
    import { SetPageButtons } from "../mainscreencomponents/UpperBar.svelte";
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";

    import { page } from "$app/state";
    import { onMount } from "svelte";
    import { fly } from "svelte/transition";
    import SongListRenderer from "../../svelte_components/reusable/SongListRenderer.svelte";
    import PageHeader from "../../svelte_components/reusable/PageHeader.svelte";
    import { SetPlaylistSave } from "../../scripts/savedElements";
    import { GetPlaylistPage } from "../../scripts/browser";

    let quary = $derived(page.url.searchParams.get("browseid"));
    let content = $state(undefined);

    let shareButtonText = $state(false);

    let contentVisible = $state(true);

    export const snapshot = {
        capture: () => {
            return content;
        },
        restore: (saved) => {
            contentVisible = false;

            content = saved;

            setTimeout(() => {
                contentVisible = true;
            }, 200);
        },
    };

    onMount(async () => {
        contentVisible = false;
        SetPageButtons([]);
        console.log("loading playlist: ", quary);
        content = await GetPlaylistPage(quary);

        if (quary === "VLLM") {
            content.data.title = "Liked music";
        }
        contentVisible = true;
        console.log(content);
    });
</script>

{#if content != undefined && contentVisible}
    <main in:fly={{ y: -50 }}>
        <PageHeader
            bgImmage={content.data.thumbnails[content.data.thumbnails.length - 1]}
            label={"PLAYLIST"}
        >
            {#if content?.data?.title}
                <p class="PL-title">
                    {(content.data.title ?? "").toUpperCase()}
                </p>
            {/if}

            {#if content?.data?.description}
                <p class="PL-desc">
                    {(content.data.description ?? "").toUpperCase()}
                </p>
            {/if}

            <div class="PL-subtitles">
                <p>
                    {content.data.subtitle.replace("Playlist \u2022 ", "")}
                </p>
                <p>{content.data.secondSubtitle}</p>
            </div>

            <div class="facepile">
                <div>
                    {#each content.data.facepile.profileIcons as icons, i}
                        <img
                            style="margin-left: {-20 * i}px;"
                            class="profile-icon"
                            src={icons}
                            alt=""
                        />
                    {/each}
                </div>
            </div>

            <div class="PL-actions">
                <button
                    onclick={() => {
                        if (!content.data.canDelete) {
                            content.data.saved = !content.data.saved;
                            SetPlaylistSave(
                                content.data.playlistId,
                                content.data.saved,
                            );
                        }
                    }}
                    style={content.data.saved ? "opacity: 1;" : "opacity: 0.3;"}
                >
                    <img src="./assets/buttons/bookmark.png" alt="" />
                </button>

                <button
                    onclick={() => {
                        navigator.clipboard
                            .writeText(content.data.shareLink)
                            .then(() => {
                                shareButtonText = true;

                                setTimeout(() => {
                                    shareButtonText = false;
                                }, 1300);
                            });
                    }}
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

        <div class="PL-elements">
            <SongListRenderer content={content.items} />
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
