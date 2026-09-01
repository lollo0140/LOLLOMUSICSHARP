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

    export async function ChangeWinState() {
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
    import { GetSettings, settings } from "../stores/settingsStore.js";
    import WindowControlls from "./mainscreencomponents/WindowControlls.svelte";

    let currentSong = $derived.by(() => {
        return $queue[$index] ?? undefined;
    });

    onMount(async () => {
        EOn("showWin", async () => {
            $settings.appearence.winStyle === "pill";
            ChangeWinState();
        });

        await GetSettings();

        await SetupLolloMusic();

        if ($settings.appearence.winStyle === "pill") {
            console.log("pill");

            WinStateToPill();
        } else {
            gsap.to(".MainTag", {
                left: 0,
                top: 0,
                bottom: 0,
                right: 0,
                height: "auto",
                transform: "",
                borderRadius: 0,
                border: "none",
            });

            gsap.to(".bg-wrapper", {
                borderRadius: 0,
            });

            opened = true;

            console.log("float");

            if (!fullscreen) {
                WinStateToFullscreen();
            }

        }
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

<main class="MainTag" style="overflow: hidden;">
    {#if opened}
        <div in:fade class="contentAnimator">
            <AppContent>
                {@render children?.()}
            </AppContent>

            <AddToPlaylistMenu />
            <EditPLaylistMenu />

            <WindowControlls {fullscreen} />
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
    <div
        class={$settings.appearence.winStyle === "pill"
            ? "controllsWrapperPill"
            : "controllsWrapperFloat"}
        transition:fly={{ y: -20 }}
    >
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
    .controllsWrapperPill {
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

    .controllsWrapperFloat {
        position: absolute;

        z-index: 2;

        height: 65px;

        border-radius: 45px;

        bottom: 46px;
        left: 99px;
        right: 375px;

        background: rgba(0, 0, 0, 0.98);
        box-shadow: 0px 0px 50px 0px black;
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
        overflow: hidden;

        width: 100%;
        height: 100%;

        position: absolute;

        left: 0px;
        top: 0px;
        bottom: 0px;
        right: 0px;
    }
</style>
