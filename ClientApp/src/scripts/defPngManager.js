const defaultPngs = {
    album: "/assets/defpng/def_album_icon.png",
    artist: "/assets/defpng/def_artist_icon.png",
    track: "/assets/defpng/def_song_icon.png",
    playlist: "/assets/defpng/def_playlist_icon.png",
}


export async function GetDefPng(type) {

    switch (type) {

        case "album":
            return defaultPngs.album;
        case "artist":
            return defaultPngs.artist;
        case "channel":
            return defaultPngs.artist;
        case "track":
            return defaultPngs.track;
        case "video":
            return defaultPngs.track;
        case "playlist":
            return defaultPngs.playlist;
        default:
            return defaultPngs.track;
    }

}
