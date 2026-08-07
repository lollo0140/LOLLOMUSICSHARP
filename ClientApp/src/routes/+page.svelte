<script>
    import { onMount } from "svelte";
    import LoadingAnimation from "../svelte_components/reusable/LoadingAnimation.svelte";
    import HomeSection from "./HomeSection.svelte";
    import { fly } from "svelte/transition";
    import { SetHomeButton } from "./+page";
    import { EInvokeJSON } from "../scripts/electronInvoker";

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
        SetHomeButton();

        let _ = await EInvokeJSON("getHome");
        content = _.Result.sections;
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
