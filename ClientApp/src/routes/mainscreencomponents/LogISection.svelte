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
    class="{open
        ? 'loginDivOpen'
        : 'loginDivClosed'} lollo-appstyle-DivContainer"
    onmouseenter={() => {
        open = true;
    }}
    onmouseleave={() => {
        open = false;
    }}
>
    <img src={$accountData.imgUrl} alt="" />

    {#if open}
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
    {/if}
</div>

<style>
    .loginDivClosed {
        display: flex;

        position: absolute;
        z-index: 2;

        right: 8px;
        top: 8px;

        width: auto;
        height: 40px;

        border-radius: 17px;

        backdrop-filter: blur(10px) brightness(0.6);
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
        border-radius: 10px;
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
