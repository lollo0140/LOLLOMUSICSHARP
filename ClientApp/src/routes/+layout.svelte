<script module>
    import {
        WinStateToFullscreen,
        WinStateToOpen,
        WinStateToPill,
        WINTOOPEN,
        WINTOPILL,
    } from "../scripts/winStates.js";

    import gsap from "gsap";

    let ipcRenderer;

    let opened = $state(false);

    let fullscreen = $state(false);
</script>

<script>
    import { onMount } from "svelte";
    import { fade, fly } from "svelte/transition";

    import { queue, index, playState } from "./audioPlayer/playerStore.js";
    import { SetupLolloMusic } from "../scripts/startUp.js";
    import { EOn, ESend } from "../scripts/electronInvoker.js";

    import AppContent from "./mainscreencomponents/AppContent.svelte";
    import MiniPlayer from "./mainscreencomponents/MiniPlayer.svelte";
    import AddToPlaylistMenu from "./AddToPlaylistMenu.svelte";
    import EditPLaylistMenu from "./EditPLaylistMenu.svelte";
    import AudioPlayer from "./audioPlayer/audioPlayer.svelte";
    import Controlls from "./mainscreencomponents/Controlls.svelte";
    import Background from "../svelte_components/single/background.svelte";
    import ContextMenu from "./ContextMenu.svelte";
    import LogISection from "./mainscreencomponents/LogISection.svelte";

    let currentSong = $derived.by(() => {
        return $queue[$index] ?? undefined;
    });

    async function ChangeWinState() {
        if (fullscreen) {
            await WinStateToFullscreen(false);
            fullscreen = false;
        }

        if (!opened) {
            WinStateToOpen();
            opened = true;
        } else {
            WinStateToPill();
            opened = false;
        }
    }

    onMount(async () => {
        EOn("showWin", async () => {
            ChangeWinState();
        });

        WinStateToPill();

        SetupLolloMusic();
    });

    let { children } = $props();
</script>

<!-- svelte-ignore a11y_no_static_element_interactions -->
<div
    style="display:{opened ? 'block' : 'none'};"
    class="closeHitbox"
    onmousedown={() => {
        ChangeWinState();
    }}
></div>

<main class="MainTag">
    {#if opened}
        <div in:fade class="contentAnimator">
            <AppContent>
                {@render children?.()}
            </AppContent>

            <AddToPlaylistMenu />
            <EditPLaylistMenu />

            <LogISection/>

            <div class="window-buttons">


                <button
                    onclick={() => {
                        ChangeWinState();
                    }}

                    ><img
                        src="./assets/windowbuttons/minimize.png"
                        alt=""
                    /></button
                >

                <button
                    onclick={() => {
                        WinStateToFullscreen(!fullscreen);
                        fullscreen = !fullscreen;
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

                <button onclick={() => {

                    ESend("setHideWinValue", true);

                    }}>
                    <img style="height: 17px; width: 17px;" src="./assets/windowbuttons/close.png" alt="" />
                </button>
            </div>
        </div>
    {:else}
        <div in:fade class="contentAnimator">
            <MiniPlayer openCommand={ChangeWinState} />
        </div>
    {/if}
    <AudioPlayer />

    <Background />
</main>

{#if opened && currentSong != undefined}
    <div class="controllsWrapper" transition:fly={{ y: -20 }}>
        <Controlls />
        <Background />
    </div>
{/if}

<ContextMenu />

<style>
    @import "./lollo_appstyles.css";

    :global(body) {
        background: transparent;
    }

    .window-buttons {
        background: rgba(255, 255, 255, 0.05);
        border: solid rgba(255, 255, 255, 0.1) 1px;
        border-radius: 40px;

        backdrop-filter: blur(10px);

        position: absolute;

        right: 15px;
        top: 15px;

        display: flex;
        flex-direction: row;

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

    .window-buttons button img
    {
        width: 20px;
        height: 20px;
    }


    .controllsWrapper {
        position: absolute;

        z-index: 2;

        height: 65px;

        border-radius: 45px;

        top: calc(100% - 178px);
        left: 300px;
        right: 300px;

        background: rgba(0, 0, 0, 0.98);
        border: solid rgba(255, 255, 255, 0.3) 1px;
    }

    main {
        background: rgba(0, 0, 0, 0.98);
        border: solid rgba(255, 255, 255, 0.3) 1px;
        border-radius: 40px;

        position: fixed;

        left: calc(50% - 300px / 2);
        right: calc(50% - 300px / 2);
        top: 0;
        height: 52;
    }

    .closeHitbox {
        position: fixed;

        left: 0px;
        right: 0px;
        top: 0px;
        bottom: 0px;

        background: transparent;
    }

    .contentAnimator {
        width: 100%;
        height: 100%;

        position: absolute;

        left: 0px;
        top: 0px;
        bottom: 0px;
        right: 0px;
    }
</style>
