<script module>
    import { fly } from "svelte/transition";
    import { GetAlbumPage, GetSavedPage } from "../scripts/browser";
    import { text } from "@sveltejs/kit";
    import { SetPlaylistSave } from "../scripts/savedElements";
    import { NavigateToArtist } from "../scripts/navigationScript";
    import { AddToPlaylist } from "./AddToPlaylistMenu.svelte";

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
        }
    });

    let title = $derived.by(() => {
        if (DATA?.title) {
            return DATA.title.toUpperCase();
        }
    });

    let subtitle = $derived.by(() => {
        if (DATA?.type === "playlist") {
            return "PLAYLIST";
        } else if (DATA?.type === "album") {
            if (DATA?.artists) {
                return DATA.artists[0].artistName.toUpperCase();
            } else {
                return "ALBUM";
            }
        }
    });

    export async function openContextMenu(e, data) {
        buttons = [];

        if (lastCliked) {
            lastCliked.style.opacity = 1;
        }

        e.srcElement.style.opacity = 0.2;
        lastCliked = e.srcElement;

        DATA = data;

        X = e.screenX;
        Y = e.screenY;
        visible = true;

        //loading content

        switch (data.type) {
            case "album":
                SetUpAlbumButtons(data);
                break;
            case "playlist":
                break;
            case "artist":
                break;
        }
    }

    export function closeContextMenu(e) {
        if (lastCliked) {
            lastCliked.style.opacity = 1;
        }
        if (e.target != contextMenu) {
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
                },
            });
        } else {
            buttons.push({
                text: "ADD TO LIBRARY",
                click: () => {
                    SetPlaylistSave(content.data.saveParam, true);
                },
            });
        }

        if (content?.data?.artist) {
            buttons.push({
                text: "SEE ARTIST",
                click: () => {
                    NavigateToArtist(content.data.artist.browseId);
                },
            });
        }

        if (content?.data?.shareLink) {
            buttons.push({
                text: "COPY LINK",
                click: () => {
                    navigator.clipboard.writeText(content.data.shareLink);
                },
            });
        }

        if (content?.items) {
            buttons.push({
                text: "ADD TO PLAYLIST",
                click: () => {AddToPlaylist(ids)},
            });
        }
    }
</script>

{#if visible}
    <div
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
