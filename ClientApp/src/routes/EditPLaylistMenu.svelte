<script module>
    import { fade } from "svelte/transition";
    import {
        CreateNewPlaylist,
        EditLibraryPlaylist,
    } from "../scripts/savedElements";
    import { ReloadLibrary } from "./library/+page.svelte";

    let sendButtonText = $state("");
    let onSendButton = $state(() => {});

    let defStatus = $state({
        name: "LOLLOMUSIC PLAYLIST",
        desc: "playlist created on LOLLOMUSIC",
        privacy: "UNLISTED",
    });

    let menuVisible = $state(false);

    let privacySelector = $state();
    let name = $state();
    let desc = $state();
    let PLid = $state();

    export async function EditPlaylist(data) {
        menuVisible = true;

        sendButtonText = "SAVE";
        onSendButton = () => {
            EditLibraryPlaylist(PLid, name, desc, privacySelector);
            if (window.location.pathname === "/library") {
                ReloadLibrary();
            }
            menuVisible = false;
        };

        console.log(data);

        name = data.data.title;
        desc = data.data.description;
        privacySelector = data.data.privacyStatus;
        PLid = data.data.playlistId;
    }

    export async function CreatePlaylist() {
        menuVisible = true;

        sendButtonText = "CREATE";

        name = defStatus.name;
        desc = defStatus.desc;
        privacySelector = defStatus.privacy;

        onSendButton = () => {
            CreateNewPlaylist(name, desc, privacySelector);
            menuVisible = false;
            if (window.location.pathname === "/library") {
                ReloadLibrary();
            }
        };
    }
</script>

{#if menuVisible}
    <main transition:fade={{ duration: 300 }}>
        <div class="edit-playlist">
            <p class="secTitle">EDIT PLAYLIST</p>
            <div class="field">
                <p>PLAYLIST NAME</p>
                <input bind:value={name} type="text" placeholder="name" />
            </div>

            <div class="field">
                <p>PLAYLIST DESCRIPTION</p>
                <input
                    bind:value={desc}
                    type="text"
                    placeholder="description"
                />
            </div>

            <div class="field">
                <p>PLAYLIST PRIVACY</p>
                <select bind:value={privacySelector} name="privacy">
                    <option value="PUBLIC">Public</option>
                    <option value="UNLISTED">Unlisted</option>
                    <option value="PRIVATE">Private</option>
                </select>
            </div>

            <button
                class="send"
                onclick={() => {
                    onSendButton();
                }}>{sendButtonText}</button
            >
        </div>

        <button
            class="close-button"
            onclick={() => {
                menuVisible = false;
            }}
        >
            .
        </button>
    </main>
{/if}

<style>

    .secTitle {
        font-size: 35px;
        font-weight: 900;
        margin: 0px;

        margin-top: 10px;

        margin-bottom: 30px;
    }

    .field {
        width: 100%;

        font-size: 20px;
        font-weight: 700;
    }

    .field p {
        margin-bottom: 5px;
    }

    .field input,
    select,
    .send {
        width: 100%;
        border: 1px solid rgba(255, 255, 255, 0.1);
        background: rgba(255, 255, 255, 0.05);
        color: white;
        padding: 10px;
        border-radius: 35px;
        text-wrap: wrap;
    }

    .send {
        transition: all 200ms;
        cursor: pointer;
    }

    .send:hover {
        transform: translateY(-5px);
    }

    .field input {
        width: calc(100% - 22px);
    }

    .edit-playlist {
        padding: 15px;



        background: black;
        border: rgba(255, 255, 255, 0.3) 1px solid;
        border-radius: 20px;

        position: absolute;

        width: 600px;
        height: fit-content;

        left: 50%;
        top: 50%;
        transform: translate(-50%, -50%);

        color: white;

        display: flex;
        flex-direction: column;
        gap: 25px;
    }

    main {
        position: absolute;
        z-index: 10;

        background: rgba(0, 0, 0, 0.758);

        backdrop-filter: blur(3px);

        border-radius: 39px;

        width: 100%;
        height: 100%;
    }

    .close-button {
        width: 100%;
        height: 100%;

        background: none;
        border: none;

        color: transparent;
    }
</style>
