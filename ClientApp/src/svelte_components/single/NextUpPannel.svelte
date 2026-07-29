<script>
    import { goto } from "$app/navigation";
    import { queue, index } from "../../routes/audioPlayer/playerStore.js";
    import { GetDefPng } from "../../scripts/defPngManager.js";

    let nextSong = $derived($queue?.[$index + 1]);

    let src = $derived.by(() => {
        if (!nextSong?.thumbnails || nextSong.thumbnails.length === 0) {
            return "/assets/defpng/def_song_icon.png";
        }

        if (nextSong.type === "none") {
            return "/assets/defpng/def_song_icon.png";
        }

        const targetThumb =
            nextSong.thumbnails[1] ?? nextSong.thumbnails[0];

        if (typeof targetThumb === "string") {
            return targetThumb.replace(
                "=w120-h120-l90-rj",
                "=w512-h512-l90-rj",
            );
        }

        if (targetThumb?.url) {
            return targetThumb.url.replace(
                "=w120-h120-l90-rj",
                "=w512-h512-l90-rj",
            );
        }

        return "/assets/defpng/def_song_icon.png";
    });
</script>

{#if nextSong != undefined}
    <button
        onclick={() => {goto("/queue")}}
        class="up-next lollo-appstyle-DivContainer"
    >
        <p class="sec-title">UP NEXT</p>

        <div style="display: flex; flex-direction: row;">
            <img
                alt=""
                {src}
                onerror={async () => {
                    src = await GetDefPng("track");
                }}
            />

            <div class="text">
                <p class="next-title">{nextSong?.title?.toUpperCase()}</p>

                {#if nextSong?.artists?.[0].artistName}
                    <p class="next-main-artist">
                        {nextSong?.artists?.[0]?.artistName?.toUpperCase() ??
                            nextSong?.artists?.[0]?.channelName?.toUpperCase()}
                    </p>
                {/if}
            </div>
        </div>
    </button>
{:else}
    <button
        onclick={() => goto("/queue")}
        class="up-next lollo-appstyle-DivContainer"
        ><p class="open-queue-indic">OPEN QUEUE</p></button
    >
{/if}

<style>
    .open-queue-indic {
        font-weight: 800;
        margin: 7px;
    }

    .sec-title {
        margin: 0px;
        opacity: 0.5;

        margin-left: 7px;
        margin-top: 5px;

        font-weight: 800;
    }

    .text p {
        margin: 0px;
        font-weight: 800;
    }

    .text {
        display: flex;
        flex-direction: column;

        justify-content: center;
        align-items: start;
    }

    .up-next {
        width: 100%;

        border-radius: 15px;

        padding: 0px;

        color: white;

        display: flex;
        flex-direction: column;

        justify-content: center;
        align-items: start;

        cursor: pointer;
    }

    .up-next:hover {
        transform: scale(1.01) translateY(-4px);
    }

    .up-next img {
        height: 50px;
        width: 50px;

        margin: 7px;

        border-radius: 8px;

        object-fit: cover;

        border: solid rgba(255, 255, 255, 0.3) 1px;
    }

    .next-title {
        height: 20px;

        max-width: 195px;
        min-width: 0;
        width: fit-content;
        text-overflow: ellipsis;
        overflow: hidden;
        text-align: start;
        white-space: nowrap;

        opacity: 1;

        font-size: 15px;
    }

    .next-main-artist {
        opacity: 0.7;
    }
</style>
