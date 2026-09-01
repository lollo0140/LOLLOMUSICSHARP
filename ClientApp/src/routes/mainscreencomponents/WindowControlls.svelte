<script>
    import { ChangeWinState } from "../+layout.svelte";
    import { ESend } from "../../scripts/electronInvoker";
    import { WinStateToFullscreen } from "../../scripts/winStates";
    import { settings } from "../../stores/settingsStore";
    import LogISection from "./LogISection.svelte";
    import UpperBar from "./UpperBar.svelte";

    let isPill = $derived($settings?.appearence?.winStyle === "pill" ?? false);

    let { fullscreen } = $props();
</script>

<main>
    <div class="inner">
        <div class="drag-area">
            <p style="margin-left: 10px;">LOLLOMUSCX</p>
        </div>

        <div class="page-buttons">
            <UpperBar />
        </div>

        <div style="width: 100%; height: 35px;"></div>

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
</main>

<style>
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

        width: 270px;

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
