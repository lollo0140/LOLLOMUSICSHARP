<script module>
    import gsap from "gsap";

    let ipcRenderer;

    let opened = $state(false);
    let canAnimate = true;

    export async function WinStateToOpen() {
        if (canAnimate) {
            canAnimate = false;
            gsap.to(".MainTag", {
                opacity: 0,
                duration: 0.2,
                onComplete: () => {
                    opened = true;
                    ipcRenderer.send("setWinState", "open");
                    setTimeout(() => {
                        gsap.to(".MainTag", {
                            left: 100,
                            right: 100,
                            top: 20,
                            height: " calc(100% - 198px - 15px)",
                            duration: 0.4,
                            ease: "back.out(1)",
                            opacity: 1,
                            onComplete: () => {
                                canAnimate = true;
                            },
                        });
                    }, 50);
                },
            });
        }
    }

    export async function WinStateToPill() {
        if (canAnimate) {
            opened = false;
            canAnimate = false;
            gsap.to(".MainTag", {
                left: "calc(50% - 300px / 2)",
                right: "calc(50% - 300px / 2)",
                top: 0,
                height: 52,
                duration: 0.4,
                opacity: 0,
                ease: "back.out(0.2)",
                onComplete: () => {
                    ipcRenderer.send("setWinState", "close");
                    gsap.to(".MainTag", {
                        opacity: 1,
                        duration: 0.2,
                    });
                    canAnimate = true;
                },
            });
        }
    }
</script>

<script>
    import { accountData } from "../stores/settingsStore.js";
    import { onMount } from "svelte";
    import AppContent from "./mainscreencomponents/AppContent.svelte";
    import { fade, fly } from "svelte/transition";
    import MiniPlayer from "./mainscreencomponents/MiniPlayer.svelte";
    import { setUpLikedList } from "../stores/songDataBase.js";
    import ContextMenu, {
        closeContextMenu,
        forceCloseMenu,
    } from "./ContextMenu.svelte";
    import AddToPlaylistMenu from "./AddToPlaylistMenu.svelte";
    import EditPLaylistMenu from "./EditPLaylistMenu.svelte";
    import AudioPlayer from "./audioPlayer/audioPlayer.svelte";
    import Controlls from "./mainscreencomponents/Controlls.svelte";

    import { queue, index, playState } from "./audioPlayer/playerStore.js";

    let currentSong = $derived.by(() => {
        return $queue[$index] ?? undefined;
    });

    async function ChangeWinState() {
        if (!opened) {
            WinStateToOpen();
        } else {
            WinStateToPill();
        }
    }

    onMount(async () => {
        //SPAWN EVENT

        // @ts-ignore
        ipcRenderer = window.electron.ipcRenderer;

        ipcRenderer.on("showWin", async () => {
            ChangeWinState();
        });

        ipcRenderer.on("reloadLogInfo", async () => {
            const logged = await ipcRenderer.lolloInvoke("loginYT");

            if (logged) {
                let logInfo = JSON.parse(
                    await ipcRenderer.lolloInvoke("getLogInfo"),
                );

                const logData = {
                    imgUrl: logInfo.imgUrl,
                    name: logInfo.name,
                    username: logInfo.username,
                    logged: true,
                };

                accountData.set(logData);
            }
        });

        WinStateToPill();

        const logged = await ipcRenderer.lolloInvoke("loginYT");

        if (logged) {
            setUpLikedList();

            let logInfo = JSON.parse(
                await ipcRenderer.lolloInvoke("getLogInfo"),
            );

            const logData = {
                imgUrl: logInfo.imgUrl,
                name: logInfo.name,
                username: logInfo.username,
                logged: true,
            };

            accountData.set(logData);
        }

        document.addEventListener("click", (e) => {
            closeContextMenu(e);
        });

        document.addEventListener(
            "scroll",
            () => {
                forceCloseMenu();
            },
            true,
        );
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
        </div>
    {:else}
        <div in:fade class="contentAnimator">
            <MiniPlayer openCommand={ChangeWinState} />
        </div>
    {/if}
    <AudioPlayer />
</main>

{#if opened && currentSong != undefined}
    <div class="controllsWrapper" transition:fly={{ y: -20 }}>
        <Controlls />
    </div>
{/if}

<ContextMenu />

<style>
    @import "./lollo_appstyles.css";

    :global(body) {
        background: transparent;
    }

    .controllsWrapper {
        position: absolute;

        z-index: 2;

        height: 65px;

        border-radius: 45px;

        top: calc(100% - 178px );
        left: 300px;
        right: 300px;

        background: rgba(0, 0, 0, 0.95);

    }

    main {
        background: rgba(0, 0, 0, 0.95);
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
