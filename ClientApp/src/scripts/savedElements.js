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

export async function EditLibraryPlaylist(id, name, desc, privacy) {
    window.electron.ipcRenderer.lolloInvoke("EditPlaylistInfo", id, name, desc, privacy);
}
export async function CreateNewPlaylist(name, desc, privacy) {
    window.electron.ipcRenderer.lolloInvoke("CreatePlaylistInfo", name, desc, privacy);
}
export async function DeletePlaylist(id) {
    window.electron.ipcRenderer.lolloInvoke("DeletePlaylist", id);
}
