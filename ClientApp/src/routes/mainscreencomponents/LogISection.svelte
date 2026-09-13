<script>
    import { fade } from "svelte/transition";
    import { accountData, setDefault } from "../../stores/settingsStore.js";
    import { onMount } from "svelte";

    let open = $state(false);

    async function LogIn() {
        await window.electron.ipcRenderer.lolloInvoke("openLog");
    }

    async function LogOff() {
        await window.electron.ipcRenderer.lolloInvoke("LogOff");
    }

    onMount(() => {
        window.addEventListener("click", (e) => {
            if (!e.target.classList.contains("log")) {
                open = false;
            }
        });
    });
</script>

<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="{open ? 'loginDivOpen' : 'loginDivClosed'} log">
    {#if open}
        <img class="log" src={$accountData.imgUrl} alt="" />

        <div in:fade class="text log">
            <p style="font-weight: 900;" class="log">{$accountData.name}</p>
            <p style="opacity: 0.7; font-weight: 700;" class="log">
                {$accountData.username}
            </p>
        </div>

        {#if $accountData.logged}
            <button
                class="log"
                onclick={() => {
                    LogOff();
                    setDefault();
                }}>LOG OFF</button
            >
        {:else}
            <button
                class="log"
                onclick={() => {
                    LogIn();
                }}>LOG IN</button
            >
        {/if}
    {:else}
        <button
            class="open-button log"
            onclick={() => {
                open = true;
            }}
        >
            <img src={$accountData.imgUrl} alt="" />
            <p class="acc-name">{$accountData.name ?? "guest"}</p>
        </button>
    {/if}
</div>

<style>
    .open-button {
        margin: 0px;
        width: 100%;
        height: 35px;

        padding: 0px;

        align-items: center;
        justify-content: space-between;
        display: flex;
        flex-direction: row;



        background: none;
    }

    .open-button * {
        pointer-events: none;
    }

    .open-button img {
        height: calc(100% - 10px);
    }

    .acc-name {
        color: white;
        font-size: 14px;

        font-weight: 800;
        opacity: 0.7;

        margin-top: 12px;
        margin-left: 0px;
        margin-right: 8px;

        white-space: nowrap;
    }

    .loginDivClosed {

        overflow: hidden;

        display: flex;

        z-index: 2;

        width: 175px;
        height: 35px;

        border-radius: 30px;

        backdrop-filter: blur(10px);

        border: 1px solid rgba(255, 255, 255, 0.1);
        background: rgba(255, 255, 255, 0.05);

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .loginDivOpen {

        overflow: hidden;

        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: space-around;

        z-index: 2;

        width: 400px;
        height: 70px;

        border-radius: 45px;

        backdrop-filter: blur(10px);

        border: 1px solid rgba(255, 255, 255, 0.1);
        background: rgba(255, 255, 255, 0.05);

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .text {
        margin-top: 0px;

        display: flex;
        flex-direction: column;
        gap: 0px;

        white-space: nowrap;
    }

    .text p {
        color: white;
        margin: 2px;

        white-space: nowrap;
    }

    .loginDivClosed img {
        margin: 6px;
        border-radius: 30px;
    }

    .loginDivOpen img {
        margin: 0px;
        border-radius: 49px;

        height: 55px;
    }

    button {
        margin: 0px;
        border-radius: 49px;

        height: 50px;

        border: none;
        background: rgba(255, 255, 255, 0.1);

        color: white;
        font-weight: 900;

        cursor: pointer;

        padding-left: 15px;
        padding-right: 15px;
    }

    button:hover {
        background: rgba(255, 255, 255, 0.3);
    }
</style>
