<script>
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";
    import SongListRenderer from "../../svelte_components/reusable/SongListRenderer.svelte";
    import SquareButton from "../../svelte_components/reusable/SquareButton.svelte";
    import { SetPageButtons } from "../mainscreencomponents/UpperBar.svelte";

    import { onMount } from "svelte";
    import { page } from "$app/state";
    import { GetDefPng } from "../../scripts/defPngManager";
    import { fly } from "svelte/transition";
    import { SetArtistSubscribe } from "../../scripts/savedElements";
    import PageHeader from "../../svelte_components/reusable/PageHeader.svelte";
    import { GetArtistPage } from "../../scripts/browser";

    let quary = $derived(page.url.searchParams.get("browseid"));
    let content = $state(undefined);

    let artistDescription = $derived.by(() => {
        let Pstrings = content.header.headerDescription.replace(
            content.header.wikipediaLink,
            "",
        );

        let paragraph = Pstrings.split(".");

        paragraph.splice(paragraph.length - 1, 1);

        return paragraph.join(".<br><br>") + ".";
    });

    let imgSrc = $state();
    let subscribed = $state();

    let contentVisible = $state();

    export const snapshot = {
        capture: () => {
            return content;
        },
        restore: (saved) => {
            contentVisible = false;
            content = saved;
            imgSrc = content.header.headerImg;
            setTimeout(() => {
                contentVisible = true;
            }, 200);
        },
    };

    onMount(async () => {
        SetPageButtons([]);
        console.log("loading artist: ", quary);
        content = await GetArtistPage(quary);

        console.log(content);

        imgSrc = content.header.headerImg;
        subscribed = content.header.subscribed;

        contentVisible = true;
    });
</script>

{#if content != undefined && contentVisible}
    <div in:fly={{ y: -50 }}>
        <PageHeader bgImmage={imgSrc} label={"ARTIST"}>
            <div class="artist-info">
                <div>
                    <p class="type"></p>

                    <p class="title">
                        {content.header.headerTitle.toUpperCase()}
                    </p>

                    <div class="stats">
                        {#if content.header.subscribeCount != "0"}
                            <p class="subscribe-count">
                                {content.header.subscribeCount} SUBSCRIBED
                            </p>
                        {/if}
                        {#if content.header.listenersCount != "no data"}
                            <p class="streams">
                                {content.header.listenersCount.toUpperCase()}
                            </p>
                        {/if}
                    </div>
                </div>

                <div class="PL-actions">
                    <button
                        style=" {subscribed
                            ? 'color: black; background: white;'
                            : ''}"
                        class="sub-button"
                        onclick={() => {
                            SetArtistSubscribe(quary, !subscribed);
                            subscribed = !subscribed;
                        }}
                    >
                        {subscribed ? "SUBSCRIBED" : "SUBSCRIBE"}
                    </button>
                </div>
            </div>
        </PageHeader>

        <div class="sections">
            {#each content.sections as sec}
                <div class="sec">
                    <p class="sec-title">{sec.title}</p>

                    <div class="sec-content">
                        {#if sec.items[0].type === "track" || sec.items[0].type === "video"}
                            <SongListRenderer content={sec.items} />
                        {:else}
                            {#each sec.items as item}
                                <SquareButton content={item} />
                            {/each}
                        {/if}
                    </div>
                </div>
            {/each}
        </div>

        {#if content.header.headerDescription && content.header.headerDescription != ")"}
            <div>
                <p class="sec-title">About {content.header.headerTitle}</p>

                <div style="margin: 20px;">
                    <p
                        style="color: white; font-size: 18px; line-height: 20px; opacity: 0.7; font-weight: 500;"
                    >
                        {@html artistDescription}
                    </p>
                </div>
            </div>
        {/if}
    </div>
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

    .type {
        opacity: 0.8;
        font-size: 24px;
        font-weight: 800;
        color: white;
    }

    .sec-title {
        margin-top: 60px;

        color: white;
        font-size: 27px;
        font-weight: 800;

        opacity: 0.7;
    }
    .sec-content {
        display: flex;
        flex-grow: 0;
        flex-wrap: wrap;

        gap: 10px;
    }

    .sub-button {
        border-radius: 20px;
        background: rgba(255, 255, 255, 0.15);

        height: 33px;

        color: white;
        font-weight: 800;
        font-size: 20px;

        padding-left: 10px;
        padding-right: 10px;

        cursor: pointer;

        transition: all 100ms;
    }

    .sub-button:hover {
        background: rgba(255, 255, 255, 0.3);
    }

    .title {

        max-width: 300px;

        font-size: 120px;
        font-weight: 900;

        margin-bottom: 20px;
        margin-top: 20px;
    }
    .streams {
        font-size: 18px;
        font-weight: 700;

        opacity: 0.7;

        margin: 0px;
    }
    .subscribe-count {
        font-size: 22px;
        font-weight: 700;

        opacity: 0.7;

        margin: 0px;
    }

    .stats {
        margin-bottom: 20px;
        margin-top: 20px;
    }

    .artist-info {
        color: white;

        display: flex;
        flex-direction: column;

        justify-items: center;
        align-items: start;
    }
</style>
