<script>
    import { accountData, setDefault } from "../../stores/settingsStore.js";

    let open = $state(false);

    async function LogIn() {
        await window.electron.ipcRenderer.lolloInvoke("openLog");
    }

    async function LogOff() {
        await window.electron.ipcRenderer.lolloInvoke("LogOff");
    }
</script>

<!-- svelte-ignore a11y_no_static_element_interactions -->
<div
    class={open ? "loginDivOpen" : "loginDivClosed"}
    onmouseenter={() => {
        open = true;
    }}
    onmouseleave={() => {
        open = false;
    }}
>
    {#if open}
        <img src={$accountData.imgUrl} alt="" />

        <div class="text">
            <p style="font-weight: 900;">{$accountData.name}</p>
            <p style="opacity: 0.7; font-weight: 700;">
                {$accountData.username}
            </p>
        </div>

        {#if $accountData.logged}
            <button
                onclick={() => {
                    LogOff();
                    setDefault();
                }}>LOG OFF</button
            >
        {:else}
            <button
                onclick={() => {
                    LogIn();
                }}>LOG IN</button
            >
        {/if}
    {:else}
        <p class="acc-name">{$accountData.name ?? "guest"}</p>
        <img src={$accountData.imgUrl} alt="" />
    {/if}
</div>

<style>

    .acc-name {
        color: white;
        font-size: 14px;

        font-weight: 800;
        opacity: 0.7;

        margin-top: 12px;
        margin-left: 10px;


    }

    .loginDivClosed {
        display: flex;

        position: absolute;
        z-index: 2;

        right: 8px;
        top: 8px;

        width: auto;
        height: 40px;

        border-radius: 30px;

        backdrop-filter: blur(10px) brightness(0.6);

        border: 1px solid rgba(255, 255, 255, 0.1);
        background: rgba(255, 255, 255, 0.05);

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .loginDivOpen {
        display: flex;

        position: absolute;
        z-index: 2;

        right: 10px;
        top: 10px;

        width: auto;
        height: 70px;

        border-radius: 45px;

        backdrop-filter: blur(10px) brightness(0.6);

        border: 1px solid rgba(255, 255, 255, 0.1);
        background: rgba(255, 255, 255, 0.05);

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }

    .text {
        margin-top: 10px;

        display: flex;
        flex-direction: column;
        gap: 0px;
    }

    .text p {
        color: white;
        margin: 2px;
    }

    .loginDivClosed img {
        margin: 6px;
        border-radius: 30px;
    }

    .loginDivOpen img {
        margin: 9px;
        border-radius: 49px;
    }

    button {
        margin: 9px;
        border-radius: 49px;

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
