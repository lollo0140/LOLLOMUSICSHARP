<script>
    import { fly } from "svelte/transition";
    import { ChangeWinState, HideControllsInHome } from "../+layout.svelte";
    import { ESend } from "../../scripts/electronInvoker";
    import { WinStateToFullscreen } from "../../scripts/winStates";
    import { settings } from "../../stores/settingsStore";
    import { SetImmersiveMode } from "./AppContent.svelte";
    import LogISection from "./LogISection.svelte";
    import UpperBar from "./UpperBar.svelte";
    import { SetFilters } from "../../svelte_components/single/background.svelte";
    import { index, queue } from "../audioPlayer/playerStore";

    let isPill = $derived($settings?.appearence?.winStyle === "pill" ?? false);

    let current = $derived($queue[$index]);
    $effect(() => {
        if (current == undefined) {
            ImmersiveMode = false;
            SetImmersiveMode(ImmersiveMode);

            if (ImmersiveMode) {
                SetFilters(0.6, 70);
            } else {
                SetFilters(0.3, 40);
            }

            HideControllsInHome(ImmersiveMode);
        }
    });

    let ImmersiveMode = $state(false);

    let { fullscreen } = $props();
</script>

<main>
    <div class="inner">
        <div class="wraper" style="justify-content: flex-start; gap: 0px;">
            <div class="drag-area">
                <p style="margin-left: 10px;">LOLLOMUSCX</p>
            </div>

            {#if !ImmersiveMode}
                <div
                    in:fly={{ duration: 400, x: -50, delay: 100 }}
                    out:fly={{ duration: 400, x: -50 }}
                    class="page-buttons"
                >
                    <UpperBar />
                </div>
            {/if}
        </div>

        <div class="wraper" style="justify-content: flex-end;">
            {#if current != undefined}
                <div
                    class="immersive-mode"
                    in:fly={{ x: 50 }}
                    out:fly={{ x: 50 }}
                >
                    <button
                        onclick={() => {
                            ImmersiveMode = !ImmersiveMode;
                            SetImmersiveMode(ImmersiveMode);

                            if (ImmersiveMode) {
                                SetFilters(0.6, 70);
                            } else {
                                SetFilters(0.3, 40);
                            }

                            HideControllsInHome(ImmersiveMode);
                        }}
                        style="app-region: no-drag;"
                    >
                        <img src="/assets/windowbuttons/landscape.png" alt="" />
                    </button>
                </div>
            {/if}

            <div class="log">
                <LogISection />
            </div>

            <div class="window-buttons">
                <button
                    onclick={() => {
                        if (isPill) {
                            ChangeWinState();
                        } else {
                            ESend("setWinState", "minimize");
                        }
                    }}
                    ><img
                        src="./assets/windowbuttons/minimize.png"
                        alt=""
                    /></button
                >

                <button
                    onclick={() => {
                        if (isPill) {
                            WinStateToFullscreen(!fullscreen);
                            fullscreen = !fullscreen;
                        } else {
                            ESend("setWinState", "maximize");
                        }
                    }}
                >
                    <img
                        style="width: 19px;"
                        src={fullscreen
                            ? "./assets/windowbuttons/close_fullscreen.png"
                            : "./assets/windowbuttons/open_in_full.png"}
                        alt=""
                    />
                </button>

                <button
                    onclick={() => {
                        if (isPill) {
                            ESend("setHideWinValue", true);
                        } else {
                            ESend("setWinState", "exit");
                        }
                    }}
                >
                    <img
                        style="height: 17px; width: 17px;"
                        src="./assets/windowbuttons/close.png"
                        alt=""
                    />
                </button>
            </div>
        </div>
    </div>
</main>

<style>
    .wraper {
        display: flex;
        flex-direction: row;
        gap: 14px;

        align-items: start;
    }

    .immersive-mode {
        height: 35px;
        width: 35px !important;

        left: 0px;
        bottom: 0px;

        background: rgba(255, 255, 255, 0.05);
        border: solid rgba(255, 255, 255, 0.1) 1px;
        border-radius: 40px;
    }

    .immersive-mode button {
        width: 100%;
        height: 100%;

        display: flex;
        align-items: center;
        justify-content: center;

        overflow: hidden;
        background: none;

        padding: none;
        border: none;

        opacity: 0.7;

        cursor: pointer;
    }

    .immersive-mode button:hover {
        opacity: 1;
    }

    .log {
        app-region: no-drag;
    }

    .page-buttons {
        margin-left: 10px;
    }

    .inner {
        display: flex;

        align-items: start;
        justify-content: space-between;

        gap: 15px;

        margin: 15px;
    }

    main {
        position: fixed;

        left: 0px;
        top: 0px;
        right: 0px;

        app-region: drag;
    }

    .drag-area {
        color: white;
        font-weight: 800;
        font-size: 20px;

        padding-right: 13px;

        display: flex;
        align-items: center;

        height: 35px;
    }

    .window-buttons {
        background: rgba(255, 255, 255, 0.05);
        border: solid rgba(255, 255, 255, 0.1) 1px;
        border-radius: 40px;

        backdrop-filter: blur(10px);

        height: 35px;

        display: flex;
        flex-direction: row;

        app-region: no-drag;

        gap: 5px;
    }

    .window-buttons button {
        background: transparent;
        border: none;

        color: white;

        cursor: pointer;

        width: 35px;
        height: 35px;

        display: flex;
        align-items: center;
        justify-content: center;

        opacity: 0.5;
    }

    .window-buttons button:hover {
        opacity: 1;
    }

    .window-buttons button img {
        width: 20px;
        height: 20px;
    }
</style>
