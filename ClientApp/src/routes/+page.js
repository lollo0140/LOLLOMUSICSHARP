import { SetPageButtons } from "./mainscreencomponents/UpperBar.svelte";

export function SetHomeButton() {
    SetPageButtons([
        {
            text: "reload",
            onclick: async () => {
                content = undefined;
                let newContent =
                    await window.electron.ipcRenderer.lolloInvoke(
                        "getHome",
                    );
                newContent = JSON.parse(newContent);
                content = newContent.Result.sections;
            },
        },
    ]);

}
