import { reloadSidebarList } from "../routes/mainscreencomponents/navigation_bar.svelte";

export async function SetArtistSubscribe(id, state) {

    window.electron.ipcRenderer.lolloInvoke("subscribeArtist", id, state);

}


export async function SetPlaylistSave(id, state) {

    window.electron.ipcRenderer.lolloInvoke("setSaveAlbum", id, state);

    setTimeout(() => {
        reloadSidebarList()
    }, 5000);
}
