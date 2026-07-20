<script module>
    import { fade } from "svelte/transition";
    import { CreateNewPlaylist, EditLibraryPlaylist } from "../scripts/savedElements";

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
        };
    }

</script>

{#if menuVisible}
    <main transition:fade={{ duration: 300 }}>
        <div class="edit-playlist">
            <input bind:value={name} type="text" placeholder="Playlist name" />
            <input
                bind:value={desc}
                type="text"
                placeholder="Playlist description"
            />

            <select bind:value={privacySelector} name="privacy">
                <option value="PUBLIC">Public</option>
                <option value="UNLISTED">Unlisted</option>
                <option value="PRIVATE">Private</option>
            </select>

            <button
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
    .edit-playlist {
        padding: 15px;

        background: black;
        border: rgba(255, 255, 255, 0.3) 1px solid;
        border-radius: 20px;

        position: absolute;

        width: fit-content;
        height: fit-content;

        left: 50%;
        top: 50%;
        transform: translate(-50%, -50%);

        color: white;
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
