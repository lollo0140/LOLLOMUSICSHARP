<script>
    import { onMount } from "svelte";
    import LoadingAnimation from "../svelte_components/reusable/LoadingAnimation.svelte";
    import HomeSection from "./HomeSection.svelte";
    import { fly } from "svelte/transition";
    import { SetPageButtons } from "./mainscreencomponents/UpperBar.svelte";

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

        content = await window.electron.ipcRenderer.lolloInvoke("getHome");
        content = JSON.parse(content);
        content = content.Result.sections;
    });
</script>

<p class="page-title">HOME</p>

<div class="home-content">
    {#if content == undefined}
        <LoadingAnimation />
    {/if}

    {#each content as section, i}
        <div in:fly={{ y: -20, delay: 100 * i}} style="width: 100%;">
            <HomeSection content={section} />
        </div>
    {/each}
</div>

<style>
    .home-content {
        margin: 0px;
        height: fit-content;
        width: 100%;

        display: flex;
        flex-direction: column;


        align-items: start;
        gap: 10px;
    }
</style>
