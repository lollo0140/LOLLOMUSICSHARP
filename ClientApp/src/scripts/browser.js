export async function GetArtistPage(id) {

    if (navigator.onLine) {
        console.log("online result ---------- " + id);
        return JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke(
                "getPageData",
                "artist",
                id,
            ),
        );
    }

    console.log("offline result ---------- " + id);
    return GetSavedPage(id, "artist");

}

export async function GetAlbumPage(id) {

    if (navigator.onLine) {
        console.log("online result ---------- " + id);
        return JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke(
                "getPageData",
                "album",
                id,
            ),
        );
    }

    console.log("offline result ---------- " + id);
    return GetSavedPage(id, "album");

}

export async function GetPlaylistPage(id) {

    if (navigator.onLine) {
        console.log("online result ---------- " + id);
        return JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke(
                "getPageData",
                "playlist",
                id,
            ),
        );
    }

    console.log("offline result ---------- " + id);
    return GetSavedPage(id, "playlist");

}

export async function GetSavedPage(id, pageType) {
    let content = JSON.parse(
        await window.electron.ipcRenderer.lolloInvoke(
            "GetFromDB",
            id,
            pageType
        )
    )

    if (content === "none") {
        return false;
    }

    return content;

}
