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
                            top: 50,
                            height: " calc(100% - 198px)",
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
    import { fade } from "svelte/transition";
    import MiniPlayer from "./mainscreencomponents/MiniPlayer.svelte";

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
        </div>
    {:else}
        <div in:fade class="contentAnimator">
            <MiniPlayer openCommand={ChangeWinState} />
        </div>
    {/if}
</main>

<style>
    @import "./lollo_appstyles.css";

    :global(body) {
        background: transparent;
    }

    .login {
        position: fixed;

        left: 0px;
        bottom: 0px;

        border: solid rgba(255, 255, 255, 0.3) 1px;
        background: rgba(255, 255, 255, 0.1);
        border-radius: 15px;
    }

    main {
        background: rgba(0, 0, 0, 0.95);
        border: solid rgba(255, 255, 255, 0.3) 1px;
        border-radius: 46px;

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
