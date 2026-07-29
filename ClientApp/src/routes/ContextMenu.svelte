<script module>
    import { fly } from "svelte/transition";
    import {
        GetAlbumPage,
        GetArtistPage,
        GetPlaylistPage,
        GetSavedPage,
    } from "../scripts/browser";
    import { text } from "@sveltejs/kit";
    import {
        DeletePlaylist,
        RemoveFromPlaylist,
        SetArtistSubscribe,
        SetPlaylistSave,
    } from "../scripts/savedElements";
    import {
        NavigateToAlbum,
        NavigateToArtist,
    } from "../scripts/navigationScript";
    import { AddToPlaylist } from "./AddToPlaylistMenu.svelte";
    import { EditPlaylist } from "./EditPLaylistMenu.svelte";
    import { AddToQueue } from "./audioPlayer/playerStore";

    let DATA = $state();

    let visible = $state(false);
    let buttons = $state([]);

    let contextMenu = $state();

    let lastCliked = undefined;

    let X = $state();
    let Y = $state();

    let headerImgSrc = $derived.by(() => {
        if (DATA?.thumbnails) {
            return DATA?.thumbnails[0];
        } else {
            return "";
        }
    });

    let title = $derived.by(() => {
        if (DATA?.title) {
            return DATA.title.toUpperCase();
        } else {
            return "";
        }
    });

    let subtitle = $derived.by(() => {
        if (DATA) {
            if (DATA?.type === "playlist") {
                return "PLAYLIST";
            } else if (DATA?.type === "album") {
                if (DATA?.artists) {
                    return DATA.artists[0].artistName.toUpperCase();
                } else {
                    return "ALBUM";
                }
            } else if (DATA.type === "video" || DATA.type === "track") {
                if (DATA?.artists?.[0]?.artistName != undefined) {
                    return DATA.artists[0].artistName;
                }
            }
        } else {
            return "";
        }
    });

    export async function openContextMenu(e, data, changeOpacity = true) {
        buttons = [];

        console.log(data);

        if (lastCliked) {
            lastCliked.style.opacity = 1;
        }

        const target = e.currentTarget || e.srcElement;
        if (changeOpacity && target) {
            target.style.opacity = 0.2;
            lastCliked = target;
        }

        DATA = data;

        let posX = e.clientX;
        let posY = e.clientY;

        const menuWidth = 200;
        const menuHeight = 350;

        const windowWidth = window.innerWidth;
        const windowHeight = window.innerHeight;

        if (posX + menuWidth > windowWidth) {
            posX = windowWidth - menuWidth - 10;
        }

        if (posY + menuHeight > windowHeight) {
            posY = windowHeight - menuHeight - 100;
        }

        X = Math.max(10, posX);
        Y = Math.max(10, posY);

        visible = true;

        //loading content

        switch (data.type) {
            case "album":
                SetUpAlbumButtons(data);
                break;
            case "playlist":
                SetUpPlaylistButtons(data);
                break;
            case "artist":
                SetUpArtistButtons(data);
                break;

            case "video":
                SetUpVideoButtons(data);
                break;
            case "track":
                SetUpVideoButtons(data);
                break;
        }
    }

    export async function openPageContextMenu(e, content, type) {
        buttons = [];

        title = "MORE OPTIONS";

        let posX = e.clientX;
        let posY = e.clientY;

        const menuWidth = 200;
        const menuHeight = 350;

        const windowWidth = window.innerWidth;
        const windowHeight = window.innerHeight;

        if (posX + menuWidth > windowWidth) {
            posX = windowWidth - menuWidth - 10;
        }

        if (posY + menuHeight > windowHeight) {
            posY = windowHeight - menuHeight - 100;
        }

        X = Math.max(10, posX);
        Y = Math.max(10, posY);

        visible = true;

        const ids = content.items.map((x) => x.id);
        console.log(ids);

        switch (type) {
            case "album":
                buttons = [];

                if (content?.items) {
                    buttons.push({
                        text: "ADD TO PLAYLIST",
                        click: () => {
                            AddToPlaylist(ids);
                            forceCloseMenu();
                        },
                    });
                }

                if (content?.items != undefined) {
                    buttons.push({
                        text: "ADD TO QUEUE",
                        click: () => {
                            AddToQueue(content.items);
                            forceCloseMenu();
                        },
                    });
                }
                break;

            case "playlist":
                buttons = [];

                if (content?.data?.canEdit || content?.data?.canDelete) {
                    if (content?.data?.canEdit) {
                        buttons.push({
                            text: "EDIT PLAYLIST",
                            click: () => {
                                EditPlaylist(content);
                                forceCloseMenu();
                            },
                        });
                    }
                    if (content?.data?.canDelete) {
                        buttons.push({
                            text: "DELETE PLAYLIST",
                            click: () => {
                                DeletePlaylist(content.data.playlistId);
                                forceCloseMenu();
                            },
                        });
                    }
                }

                if (content?.items) {
                    buttons.push({
                        text: "ADD TO PLAYLIST",
                        click: () => {
                            AddToPlaylist(ids);
                            forceCloseMenu();
                        },
                    });
                }

                if (content?.items != undefined) {
                    buttons.push({
                        text: "ADD TO QUEUE",
                        click: () => {
                            AddToQueue(content.items);
                            forceCloseMenu();
                        },
                    });
                }
                break;
        }
    }

    export function closeContextMenu(e) {
        if (lastCliked) {
            lastCliked.style.opacity = 1;
        }

        const isInsidePageMenu = e.target.closest(".page-menu");
        const isInsideContextMenu =
            e.target === contextMenu ||
            (contextMenu && contextMenu.contains(e.target));

        if (!isInsideContextMenu && !isInsidePageMenu) {
            visible = false;
        }
    }

    export function forceCloseMenu() {
        if (lastCliked) {
            lastCliked.style.opacity = 1;
        }
        visible = false;
    }

    //actions -------------------------------

    async function SetUpAlbumButtons(data) {
        let content = await GetAlbumPage(data.browseId);
        console.log(content);

        const ids = content.items.map((x) => x.id);
        console.log(ids);

        buttons = [];

        //save action

        if (content.data.saved) {
            buttons.push({
                text: "REMOVE FROM LIBRARY",
                click: () => {
                    SetPlaylistSave(content.data.saveParam, false);
                    forceCloseMenu();
                },
            });
        } else {
            buttons.push({
                text: "ADD TO LIBRARY",
                click: () => {
                    SetPlaylistSave(content.data.saveParam, true);
                    forceCloseMenu();
                },
            });
        }

        if (content?.data?.artist) {
            buttons.push({
                text: "SEE ARTIST",
                click: () => {
                    NavigateToArtist(content.data.artist.browseId);
                    forceCloseMenu();
                },
            });
        }

        if (content?.data?.shareLink) {
            buttons.push({
                text: "COPY LINK",
                click: () => {
                    navigator.clipboard.writeText(content.data.shareLink);
                    forceCloseMenu();
                },
            });
        }

        if (content?.items) {
            buttons.push({
                text: "ADD TO PLAYLIST",
                click: () => {
                    AddToPlaylist(ids);
                    forceCloseMenu();
                },
            });
        }

        if (content?.items != undefined) {
            buttons.push({
                text: "ADD TO QUEUE",
                click: () => {
                    AddToQueue(content.items);
                    forceCloseMenu();
                },
            });
        }
    }

    async function SetUpPlaylistButtons(data) {
        let content = await GetPlaylistPage(data.browseId);
        console.log(content);

        const ids = content.items.map((x) => x.id);
        console.log(ids);

        buttons = [];

        if (content?.data?.canEdit || content?.data?.canDelete) {
            if (content?.data?.canEdit) {
                buttons.push({
                    text: "EDIT PLAYLIST",
                    click: () => {
                        EditPlaylist(content);
                        forceCloseMenu();
                    },
                });
            }
            if (content?.data?.canDelete) {
                buttons.push({
                    text: "DELETE PLAYLIST",
                    click: () => {
                        DeletePlaylist(content.data.playlistId);
                        forceCloseMenu();
                    },
                });
            }
        } else {
            if (content?.data?.saved) {
                buttons.push({
                    text: "REMOVE FROM LIBRARY",
                    click: () => {
                        SetPlaylistSave(content.data.playlistId, false);
                        forceCloseMenu();
                    },
                });
            } else {
                buttons.push({
                    text: "ADD TO LIBRARY",
                    click: () => {
                        SetPlaylistSave(content.data.playlistId, false);
                        forceCloseMenu();
                    },
                });
            }
        }

        if (content?.data?.shareLink) {
            buttons.push({
                text: "COPY LINK",
                click: () => {
                    navigator.clipboard.writeText(content.data.shareLink);
                    forceCloseMenu();
                },
            });
        }

        if (content?.items) {
            buttons.push({
                text: "ADD TO PLAYLIST",
                click: () => {
                    AddToPlaylist(ids);
                    forceCloseMenu();
                },
            });
        }

        if (content?.items != undefined) {
            buttons.push({
                text: "ADD TO QUEUE",
                click: () => {
                    AddToQueue(content.items);
                    forceCloseMenu();
                },
            });
        }
    }

    async function SetUpArtistButtons(data) {
        let content = await GetArtistPage(data.browseId);

        if (content.header.subscribed) {
            buttons.push({
                text: "UNSUBSCRIBE",
                click: () => {
                    SetArtistSubscribe(data.browseId, false);
                    forceCloseMenu();
                },
            });
        } else {
            buttons.push({
                text: "SUBSCRIBE",
                click: () => {
                    SetArtistSubscribe(data.browseId, true);
                    forceCloseMenu();
                },
            });
        }
    }

    async function SetUpVideoButtons(data) {
        console.log(data);

        if (data.album.albumId != undefined) {
            buttons.push({
                text: "GO TO ALBUM",
                click: () => {
                    NavigateToAlbum(data.album.albumId);
                    forceCloseMenu();
                },
            });
        }

        if (data?.artists?.[0]?.artistId != undefined) {
            buttons.push({
                text: "GO TO ARTIST",
                click: () => {
                    NavigateToArtist(data?.artists[0]?.artistId);
                    forceCloseMenu();
                },
            });
        }

        if (data?.setVideoId != undefined && data?.playlistId != undefined) {
            buttons.push({
                text: "REMOVE FROM PLAYLIST",
                click: () => {
                    RemoveFromPlaylist(
                        data?.id,
                        data?.setVideoId,
                        data?.playlistId,
                    );
                    forceCloseMenu();
                },
            });
        }

        if (data?.id != undefined) {
            buttons.push({
                text: "ADD TO PLAYLIST",
                click: () => {
                    AddToPlaylist([data?.id]);
                    forceCloseMenu();
                },
            });
        }

        if (data?.id != undefined) {
            buttons.push({
                text: "ADD TO QUEUE",
                click: () => {
                    AddToQueue([data]);
                    forceCloseMenu();
                },
            });
        }
    }
</script>

{#if visible}
    <div
        class="main-cm"
        style="left: {X + 5}px; top: {Y + 5}px;"
        transition:fly={{ y: -15 }}
        bind:this={contextMenu}
    >
        <div class="CM-content">
            <div class="CM_header">
                <img src={headerImgSrc} alt="" />

                <p class="CM-H-titile">{title}</p>
                <p class="CM-H-subtitile">{subtitle}</p>
            </div>

            {#each buttons as B, i}
                <button
                    class="ACT-button"
                    in:fly={{ y: -5, delay: 100 * i }}
                    onclick={B.click}
                >
                    {B.text}</button
                >
            {/each}
        </div>
    </div>
{/if}

<style>
    .main-cm {
        z-index: 99;
    }

    .CM-content {
        display: flex;
        flex-direction: column;

        align-items: center;
        justify-content: center;

        margin: 0px;

        padding: 10px;
    }

    .ACT-button {
        width: 100%;
        height: 45px;

        border-radius: 15px;
        border: 1px rgba(255, 255, 255, 0.5) solid;
        background: black;

        font-size: 15px;
        font-weight: 800;
        color: rgba(255, 255, 255, 0.5);

        text-align: start;
        padding-left: 10px;
        padding-right: 10px;

        margin-top: 5px;

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;

        box-shadow:
            rgba(0, 0, 0, 0.3) 0px 19px 38px,
            rgba(0, 0, 0, 0.22) 0px 15px 12px;

        cursor: pointer;
    }

    .ACT-button:hover {
        transform: translateY(-2px);
        color: rgba(255, 255, 255, 1);
    }

    .CM-H-titile {
        font-weight: 800;
    }
    .CM-H-subtitile {
        opacity: 0.6;
        font-weight: 700;
    }
    .CM_header {
        position: relative;

        width: 100%;
        height: 60px;

        overflow: hidden;

        border-radius: 15px;
        border: 1px solid rgba(255, 255, 255, 0.5);
        background: black;

        display: flex;
        flex-direction: column;
        align-items: start;
        justify-content: center;

        box-shadow:
            rgba(0, 0, 0, 0.3) 0px 19px 38px,
            rgba(0, 0, 0, 0.22) 0px 15px 12px;
    }

    .CM_header p {
        color: white;
        margin: 0px;

        margin-left: 10px;

        width: 200px;
        overflow: hidden;
        text-overflow: ellipsis;
        text-wrap: nowrap;
        white-space: nowrap;
    }

    .CM_header img {
        width: 100%;
        height: 100%;

        filter: blur(15px);

        opacity: 0.5;

        position: absolute;
    }

    div {
        position: fixed;

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
    }
</style>
