<script>
    import { SetPageButtons } from "../mainscreencomponents/UpperBar.svelte";
    import PlaylistHeader from "../../svelte_components/reusable/PlaylistHeader.svelte";
    import LoadingAnimation from "../../svelte_components/reusable/LoadingAnimation.svelte";

    import { page } from "$app/state";
    import { onMount } from "svelte";
    import { fly } from "svelte/transition";

    let quary = $derived(page.url.searchParams.get("browseid"));
    let content = $state(undefined);

    export const snapshot = {
        capture: () => {
            return content;
        },
        restore: (saved) => {
            content = saved;
        },
    };

    onMount(async () => {
        SetPageButtons([]);
        console.log("loading playlist: ", quary);
        content = JSON.parse(
            await window.electron.ipcRenderer.lolloInvoke(
                "getPageData",
                "playlist",
                quary,
            ),
        );



        if (quary === "VLLM") {
            content.data.title = "Liked music";
        }

        console.log(content);
    });
</script>

{#if content != undefined}
    <div in:fly={{ y: -50 }}>
        <PlaylistHeader {content} type={"playlist"} />
    </div>
{:else}
    <div style="height: min-content;">
        <LoadingAnimation />
    </div>
{/if}
