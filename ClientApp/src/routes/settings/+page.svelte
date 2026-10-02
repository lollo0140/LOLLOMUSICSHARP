<script>
    import { fly } from "svelte/transition";
    import SettingSection from "./SettingSection.svelte";
    import OptionLabel from "./optionLabel.svelte";

    import { settings } from "../../stores/settingsStore";
    import { EInvoke, ESend } from "../../scripts/electronInvoker";

    let activeStyle = $state($settings.appearence.winStyle); //float


    let requireRestart = $state(false);
    let settingChanged = $state(false);


    $effect(() => {
        console.log($settings);
    });
</script>

<main>
    <p class="page-title">SETTINGS</p>

    <SettingSection title={"APPEARENCE"}>
        <OptionLabel label={"WINDOW STYLE"} column={true}>
            <div class="win-style-selector">
                <button
                    onclick={() => {
                        activeStyle = "pill";
                        requireRestart = true;
                        settingChanged = true;
                    }}
                    class="demo-container"
                    style="opacity: {activeStyle === 'pill' ? '1' : '0.5'};"
                >
                    <img src="/assets/demos/win1demo.gif" alt="" />
                </button>

                <button
                    onclick={() => {
                        activeStyle = "float";
                        requireRestart = true;
                        settingChanged = true;
                    }}
                    class="demo-container"
                    style="opacity: {activeStyle === 'float' ? '1' : '0.5'};"
                >
                    <img src="/assets/demos/win2demo.gif" alt="" />
                </button>
            </div>
        </OptionLabel>
    </SettingSection>

    <SettingSection title={"LOCAL STORAGE"}>
        <OptionLabel label={"DOWNLOAD PATH"}>
            <p>{$settings?.localData?.downloadPath ?? "default"}</p>
            <button
                onclick={async () => {
                    const newPath = await EInvoke("openDirPicker");
                    settingChanged = true;
                    $settings.localData.downloadPath = newPath;
                }}>CHOSE DIRECTORY</button
            >
        </OptionLabel>
    </SettingSection>

    {#if settingChanged}
        <button
            transition:fly={{ y: 10 }}
            onclick={() => {

                $settings.appearence.winStyle = activeStyle;

                EInvoke("saveSettings", JSON.stringify($settings));

                if (requireRestart) {
                    ESend("setWinState", "exit");
                }

                settingChanged = false;
            }}
            class="save-button"
        >
            APPLY SETTINGS
        </button>
    {/if}

    <div class="spacer"></div>
</main>

<style>
    main {
        position: absolute;
        left: 0px;
        top: 0px;
        bottom: 0px;
        right: 0px;

        overflow-y: scroll;
        overflow-x: hidden;
    }

    .win-style-selector {
        width: 100%;

        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;

        margin-bottom: 10px;

        gap: 20px;
    }

    .demo-container {
        height: 300px;
        width: 600px;

        border: 2px rgba(255, 255, 255, 0.5) solid;
        border-radius: 25px;
        overflow: hidden;
        display: flex;
        align-items: center;
        justify-content: center;

        transition: all 300ms;
    }

    .demo-container img {
        width: 200%;
        height: 150%;
        object-fit: contain;
    }

    .save-button {
        position: absolute;

        background: white;
        color: black;

        right: 20px;

        margin-top: 20px;

        font-size: 20px;
    }

    .save-button:hover {
        background: black;
        color: white;
    }

    button {
        border: 1px rgba(255, 255, 255, 0.2) solid;
        background: rgba(255, 255, 255, 0.05);
        border-radius: 30px;

        padding: 7px;
        padding-left: 10px;
        padding-right: 10px;

        color: white;
        font-weight: 800;

        cursor: pointer;
    }

    button:hover {
        background: rgba(255, 255, 255, 0.1);
    }

    p {
        color: white;
    }
</style>
