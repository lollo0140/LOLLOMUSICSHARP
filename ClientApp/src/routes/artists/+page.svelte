<script>
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";
    import SongListRenderer from "../../svelte_components/reusable/SongListRenderer.svelte";
    import SquareButton from "../../svelte_components/reusable/SquareButton.svelte";
    import { SetPageButtons } from "../mainscreencomponents/UpperBar.svelte";

    import { onMount } from "svelte";
    import { page } from "$app/state";
    import { GetDefPng } from "../../scripts/defPngManager";
    import { fly } from "svelte/transition";

    let quary = $derived(page.url.searchParams.get("browseid"));
    let content = $state(undefined);

    let artistDescription = $derived.by( () =>
    {
        let Pstrings = content.header.headerDescription.replace(content.header.wikipediaLink, "")

        let paragraph = Pstrings.split(".")

        paragraph.splice(paragraph.length - 1, 1)

        return paragraph.join(".<br><br>") + ".";

    })

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
        content = JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke(
                "getPageData",
                "artist",
                quary,
            ),
        );

        console.log(content);

        imgSrc = content.header.headerImg;
        subscribed = content.header.subscribed;

        contentVisible = true;
    });
</script>

{#if content != undefined && contentVisible}
    <div in:fly={{ y: -50 }}>
        <div class="header">
            <img
                src={imgSrc}
                alt=""
                onerror={async () => {
                    imgSrc = await GetDefPng("artist");
                }}
            />

            <div class="artist-info">
                <div>
                    <p class="title">{content.header.headerTitle}</p>

                    {#if content.header.subscribeCount != "0"}
                        <p class="subscribe-count">
                            {content.header.subscribeCount} subscribed
                        </p>
                    {/if}
                    {#if content.header.listenersCount != "no data"}
                        <p class="streams">{content.header.listenersCount}</p>
                    {/if}
                </div>

                <button class="sub-button">
                    {subscribed ? "Subscribed" : "Subscribe"}
                </button>
            </div>
        </div>

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
                    <p style="color: white; font-size: 18px; line-height: 20px; opacity: 0.7; font-weight: 500;">{@html artistDescription}</p>
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
        border: 1px solid rgba(255, 255, 255, 0.3);
        border-radius: 10px;
        background: rgba(255, 255, 255, 0.1);

        backdrop-filter: blur(10px);

        color: white;
        font-weight: 800;
        font-size: 15px;

        padding: 15px;

        cursor: pointer;

        margin-top: 35px;
    }

    .sub-button:hover {
        background: rgba(255, 255, 255, 0.3);
    }

    .title {
        font-size: 120px;
        font-weight: 900;

        margin: 0px;
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

    .artist-info {
        position: absolute;

        color: white;

        bottom: 20px;
        left: 20px;

        display: flex;
        flex-direction: column;

        justify-items: center;
        align-items: start;
    }

    .header {
        position: relative;

        border: 1px solid rgba(255, 255, 255, 0.3);
        background: rgba(255, 255, 255, 0.1);

        overflow: hidden;
        border-radius: 16px;

        height: 400px;
    }

    .header img {
        position: absolute;

        margin: 0px;

        width: 100%;
        object-fit: cover;

        -webkit-mask-image: linear-gradient(to bottom, black, transparent 70%);
        mask-image: linear-gradient(to left, black, transparent);
    }
</style>
