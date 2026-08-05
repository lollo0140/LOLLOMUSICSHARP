<script>
    import { fly } from "svelte/transition";
    import SettingSection from "./SettingSection.svelte";
    import OptionLabel from "./optionLabel.svelte";

    import { settings } from "../../stores/settingsStore";

    let settingChanged = $state(false);

    $effect(() => {
        console.log($settings);
    });
</script>

<p class="page-title">SETTINGS</p>

<SettingSection title={"LOCAL STORAGE"}>
    <OptionLabel label={"DOWNLOAD PATH"}>
        <p>{$settings.localData.downloadPath}</p>
        <button
            onclick={async () => {
                const newPath =
                    await window.electron.ipcRenderer.lolloInvoke(
                        "openDirPicker",
                    );
                settingChanged = true;
                $settings.localData.downloadPath = newPath;
            }}>CHOSE DIRECTORY</button
        >
    </OptionLabel>
</SettingSection>


{#if settingChanged}
    <button transition:fly={{y:10}} onclick={ () => {
        window.electron.ipcRenderer.lolloInvoke("saveSettings", JSON.stringify($settings));
        settingChanged = false;
    }} class="save-button"> APPLY SETTINGS </button>
{/if}

<style>
    .save-button {
        position: absolute;

        background: white;
        color: black;



        bottom: 35px;
        right: 35px;
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
