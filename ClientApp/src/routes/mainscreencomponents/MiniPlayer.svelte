<script>
    let { openCommand } = $props();

    import { SetPlayState } from "../audioPlayer/audioPlayer.svelte";
    import { NextTrack, PreviousTrack } from "../audioPlayer/playerStore";
    import { playState, queue, index } from "../audioPlayer/playerStore.js";

    let showControlls = $state(false);

    let current = $derived($queue[$index]);
</script>

<!-- svelte-ignore a11y_no_static_element_interactions -->
<div
    class="mini-player"
    onmouseenter={() => {
        showControlls = true;
    }}
    onmouseleave={() => {
        showControlls = false;
    }}
>
    {#if current != undefined}
        <div class="bg-wrapper">
            <img
                class="bg-img"
                src={current?.thumbnails?.[0] ?? undefined}
                alt=""
            />
        </div>

        <div
            class="button-container"
            style="transform: translateY({showControlls ? -53 : 0}px);"
        >
            <div class="current-info">
                <p class="track-title">{current?.title?.toUpperCase()}</p>

                <div style="display: flex; align-items: center;">
                    <p class="track-artist">
                        {current?.artists?.[0]?.artistName ?? ""}
                    </p>

                    <p class="track-album"> { "• " + current?.album?.titleName ?? undefined}</p>
                </div>
            </div>

            <div class="mainButtons">
                <button
                    onclick={() => PreviousTrack()}
                    aria-label="Traccia precedente"
                >
                    <img
                        draggable="false"
                        src="assets/controlls/previous.png"
                        alt=""
                    />
                </button>

                <button
                    onclick={() => {
                        if ($playState) {
                            SetPlayState(false);
                        } else {
                            SetPlayState(true);
                        }
                    }}
                    aria-label={$playState ? "Pausa" : "Play"}
                >
                    <img
                        draggable="false"
                        src={$playState
                            ? "assets/controlls/pause.png"
                            : "assets/controlls/play.png"}
                        alt=""
                    />
                </button>

                <button
                    onclick={() => NextTrack()}
                    aria-label="Traccia successiva"
                >
                    <img
                        draggable="false"
                        src="assets/controlls/next.png"
                        alt=""
                    />
                </button>
            </div>
        </div>
    {:else}
        <p class="def-text">NOTHING IN QUEUE</p>
    {/if}

    <button
        class="open-button"
        onclick={() => openCommand()}
        aria-label="Ingrandisci"
    >
        <img draggable="false" src="/assets/resize.png" alt="" />
    </button>
</div>

<style>
    .def-text {
        color: white;
        font-weight: 900;
        margin-left: 15px;
    }

    /* --- CONTENITORE PRINCIPALE --- */
    .mini-player {
        position: relative;
        display: flex;
        flex-direction: row;
        width: 100%;
        height: 100%;
        margin: 0;
        padding: 0;
        border-radius: 20px;
        overflow: hidden !important;
        border: 100px;
    }

    /* --- SFONDO BLUR --- */
    .bg-wrapper {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 0;
        overflow: hidden;
        pointer-events: none;

        border-radius: 100px;
    }

    .bg-img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        opacity: 0.5;
        filter: blur(18px);
        transform: scale(1.2);
    }

    /* --- CONTENUTI E PULSANTI (Livello superiore) --- */
    .button-container {
        position: relative;
        z-index: 1;

        margin-left: 5px;

        width: 100%;
        transition: transform 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .current-info {
        display: flex;
        flex-direction: column;
        align-items: flex-start;
        justify-content: center;
        height: 52px;
        padding-left: 10px;
        color: white;
    }

    .track-title {
        margin: 0;
        font-weight: 900;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 85%;
    }

    .track-artist {
        margin: 0;
        font-weight: 900;
        opacity: 0.7;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 100%;
    }

    .track-album {
        margin: 0px;
        margin-left: 5px;
        font-weight: 700;

        opacity: 0.5;
    }

    .mainButtons {
        height: 55px;
        width: 143px;
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;
    }

    .mainButtons button {
        background: none;
        border: none;
        opacity: 0.5;
        cursor: pointer;
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .mainButtons button:hover {
        transform: scale(1.1);
        opacity: 1;
    }

    .open-button {
        right: 0px;

        position: absolute;
        z-index: 1;
        margin: 3px;
        height: calc(100% - 6px);
        width: 45px;
        border-radius: 40px;
        background: none;
        border: none;
        display: flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        opacity: 0.5;
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .open-button:hover {
        transform: scale(1.1);
        opacity: 1;
    }
</style>
