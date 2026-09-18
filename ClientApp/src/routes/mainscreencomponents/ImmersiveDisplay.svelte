<script>
    import { GetImmageUrl } from "../../scripts/immages";
    import { index, queue } from "../audioPlayer/playerStore";
    import Controlls from "./Controlls.svelte";

    let currentSong = $derived($queue[$index]);

    let currentSongImmage = $derived.by(() => {
        if (!currentSong?.thumbnails || currentSong.thumbnails.length === 0) {
            return "/assets/defpng/def_song_icon.png";
        }

        if (currentSong.type === "none") {
            return "/assets/defpng/def_song_icon.png";
        }

        const targetThumb =
            currentSong.thumbnails[1] ?? currentSong.thumbnails[0];

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

<main>
    <div class="infos">
        <img class="immage" src={GetImmageUrl(currentSongImmage, 0)} alt="" />

        <div class="text">
            <p class="title">{currentSong?.title?.toUpperCase()}</p>
            <p class="artists">
                {currentSong?.artists
                    ?.map((x) => x.artistName?.toUpperCase())
                    ?.join(", ")}
                {#if currentSong?.title != currentSong?.album?.titleName?.toUpperCase()}
                    ● {currentSong?.album?.titleName?.toUpperCase()}
                {/if}
            </p>
        </div>
    </div>

    <div class="controlls">
        <Controlls />
    </div>
</main>

<style>
    .immage {
        height: 422px;

        border: rgba(255, 255, 255, 0.25) solid 1px;
        border-radius: 30px;
    }

    .text {
        display: flex;
        flex-direction: column;
        align-items: start;

        gap: 0px;

        position: relative;
        width: 422px;
    }

    .title {
        font-weight: 900;
        font-size: 30px;
        margin: 0px;
    }

    .artists {
        font-size: 22px;
        font-weight: 800;
        opacity: 0.7;
        margin: 0px;
    }

    main {
        position: absolute;

        left: 0px;
        top: 0px;
        right: 0px;
        bottom: 0px;

        display: flex;
        flex-direction: column;

        justify-content: center;
        align-items: center;
    }

    .infos {
        color: white;

        overflow: hidden;

        margin-bottom: 150px;

        display: flex;
        flex-direction: column;
        align-items: start;
        justify-content: center;

        gap: 20px;
    }

    .controlls {
        opacity: 0.1;

        position: absolute;

        width: fit-content;
        height: fit-content;

        height: 70px;

        bottom: 70px;

        width: calc(100% - 400px);

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .controlls:hover {
        opacity: 1;
        transform: scale(1.01);
    }
</style>
