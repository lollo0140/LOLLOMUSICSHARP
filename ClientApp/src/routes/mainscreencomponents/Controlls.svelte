<script module>
    let CurrentTime = $state(1);
    let songDuration = $state(1);

    let hovering = false;

    export function SetCurrentTime(sec) {
        if (!hovering) {
            CurrentTime = Math.floor(sec);
        }
    }

    export function SetDurationTime(sec) {
        songDuration = Math.floor(sec);
    }
</script>

<script>
    import { SecTotimeDisplay } from "../../scripts/misc";
    import {
        SetCurrentTimelineSec,
        SetPlayerVolume,
        SetPlayState,
    } from "../audioPlayer/audioPlayer.svelte";
    import {
        cycleRepeatMode,
        NextTrack,
        PreviousTrack,
        toggleShuffleMode,
    } from "../audioPlayer/playerStore";
    import {
        playState,
        repeatValue,
        shuffleValue,
    } from "../audioPlayer/playerStore.js";

    let volumeValue = $state();
    let savedVolumeValue = 0;

    let CurrentTimeDisplay = $derived(SecTotimeDisplay(CurrentTime));
    let songDurationDisplay = $derived(SecTotimeDisplay(songDuration));

    let volumeImgSrc = $derived.by(() => {
        if (volumeValue === 0) {
            return "/assets/controlls/volume_none.png";
        } else if (volumeValue < 30) {
            return "/assets/controlls/volume_low.png";
        } else if (volumeValue < 60) {
            return "/assets/controlls/volume_medium.png";
        } else if (volumeValue <= 100) {
            return "/assets/controlls/volume_max.png";
        }
    });
</script>

<div class="container">
    <div class="timeline">
        <div class="timeline-text">
            <p>{CurrentTimeDisplay}</p>

            <p>{songDurationDisplay}</p>
        </div>

        <input
            onmousedown={() => {
                hovering = true;
            }}
            onmouseup={() => {
                hovering = false;
                SetCurrentTimelineSec(CurrentTime);
            }}
            bind:value={CurrentTime}
            min="0"
            max={songDuration}
            type="range"
        />
    </div>

    <div class="mainButtons">
        <button onclick={() => PreviousTrack()}>
            <img src="/assets/controlls/previous.png" alt="" />
        </button>

        <button
            onclick={() => {
                if ($playState) {
                    SetPlayState(false);
                } else {
                    SetPlayState(true);
                }
            }}
        >
            <img
                src={`/assets/controlls/${$playState ? "pause.png" : "play.png"}`}
                alt=""
            />
        </button>

        <button onclick={() => NextTrack()}>
            <img src="/assets/controlls/next.png" alt="" />
        </button>
    </div>

    <div class="secondary-button">
        <button
            onclick={() => {
                cycleRepeatMode();
            }}
        >
            <img
                style={$repeatValue != 0 ? "opacity: 1;" : "opacity: 0.2;"}
                src={$repeatValue === 2
                    ? "/assets/controlls/repeat_one.png"
                    : "/assets/controlls/repeat.png"}
                alt=""
            />
        </button>

        <button
            onclick={() => {
                toggleShuffleMode();
            }}
        >
            <img
                style={$shuffleValue ? "opacity: 1" : "opacity: 0.2"}
                src="/assets/controlls/shuffle.png"
                alt=""
            />
        </button>

        <div class="volume-section">
            <button
                onclick={() => {
                    if (volumeValue == 0) {
                        volumeValue = savedVolumeValue;
                        SetPlayerVolume(volumeValue);
                    } else {
                        savedVolumeValue = volumeValue;
                        volumeValue = 0;
                        SetPlayerVolume(volumeValue);
                    }
                }}
            >
                <img src={volumeImgSrc} alt="" />
            </button>

            <input
                class=""
                min="0"
                max="100"
                bind:value={volumeValue}
                type="range"
                oninput={() => {
                    SetPlayerVolume(volumeValue);
                }}
            />
        </div>
    </div>
</div>

<style>
    .volume-section {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: center;

        margin-left: 20px;
    }

    .mainButtons {
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .mainButtons button {
        width: 50px;
        height: 50px;

        background: none;
        border: none;

        padding: 0px;

        cursor: pointer;

        transition: all 0.1s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;

        opacity: 0.5;
    }

    .mainButtons button:hover {
        transform: scale(1.05) translateY(-3px);
        opacity: 1;
    }

    .mainButtons button img {
        width: calc(100% - 10px);
        height: calc(100% - 10px);

        margin: 5px;
    }

    .timeline {
        display: flex;
        flex-direction: column;

        justify-content: space-between;

        height: 32px;
        width: 50%;

        margin-left: 15px;
    }

    .timeline-text {
        color: white;
        font-weight: 700;

        opacity: 0.6;

        display: flex;
        flex-direction: row;

        justify-content: space-between;
        align-items: center;

        margin: 0px;
        height: 15px;
    }

    .container {
        position: absolute;
        height: calc(100% - 2px);
        width: calc(100% - 2px);

        border-radius: 45px;
        background: transparent;

        display: flex;
        flex-direction: row;

        justify-content: space-between;
        align-items: center;
    }

    .secondary-button {
        margin-right: 10px;

        gap: 10px;

        display: flex;
        flex-direction: row;
    }

    .secondary-button button {
        width: 40px;
        height: 40px;

        background: none;
        border: none;

        padding: 0px;

        cursor: pointer;

        transition: all 0.1s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;

        opacity: 0.5;
    }

    .secondary-button button:hover {
        transform: scale(1.05) translateY(-3px);
        opacity: 1;
    }

    .volume-section button {
        width: 40px;
        height: 40px;
    }

    .volume-section button img {
        height: calc(100% - 10px);

        margin: 5px;
    }
</style>
