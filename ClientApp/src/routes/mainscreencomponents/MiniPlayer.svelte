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

                    {#if current?.album?.titleName}
                        <p class="track-album">
                            {"• " + current?.album?.titleName}
                        </p>
                    {/if}
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

    <div class="drag-region"></div>

    <button
        class="open-button"
        onclick={() => openCommand()}
        aria-label="Ingrandisci"
    >
        <img draggable="false" src="/assets/resize.png" alt="" />
    </button>
</div>

<style>
    .drag-region {
        position: absolute;

        right: 50px;

        background: transparent;
        width: 70px;
        height: 55px;

        app-region: drag;
    }

    .mini-player {
        position: relative;
        display: flex;
        flex-direction: row;
        width: 100%;
        height: 100%;
        margin: 0;
        padding: 0;
        border-radius: 34px;
        overflow: hidden !important;
    }

    .def-text {
        color: white;
        font-weight: 900;
        margin-left: 15px;
        display: flex;
        align-items: center;
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
        pointer-events: none; /* Evita che intercepti i click o il drag */
        border-radius: 20px;
    }

    .bg-img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        opacity: 0.5;
        filter: blur(18px);
        transform: scale(1.2);
    }

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
        max-width: 20%;
    }

    .track-album {
        margin: 0;
        margin-left: 5px;
        font-weight: 700;
        opacity: 0.5;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 60%;
    }

    /* --- PULSANTI E CONTROLLI (NO-DRAG) --- */
    .mainButtons {
        height: 55px;
        width: 143px;
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;
    }

    /* Disabilita il drag su TUTTI i bottoni per permettere i click */
    .mainButtons button,
    .open-button {
        -webkit-app-region: no-drag;
        background: none;
        border: none;
        opacity: 0.5;
        cursor: pointer;
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .mainButtons button:hover,
    .open-button:hover {
        transform: scale(1.1);
        opacity: 1;
    }

    .open-button {
        position: absolute;
        right: 0;
        top: 0;
        z-index: 1;
        margin: 3px;
        height: calc(100% - 6px);
        width: 45px;
        border-radius: 40px;
        display: flex;
        align-items: center;
        justify-content: center;
    }
</style>
