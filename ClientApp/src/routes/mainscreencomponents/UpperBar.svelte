<script module>
    // @ts-ignore
    let customData = $state([]);
    // @ts-ignore
    export async function SetPageButtons(data) {
        customData = [];

        setTimeout(() => {
            customData = data;
        }, 300);
    }

    SetPageButtons([]);

</script>

<script>
    import { afterNavigate } from "$app/navigation";
    import { onMount } from "svelte";
    import SearchBar from "./SearchBar.svelte";
    import { fly } from "svelte/transition";

    let canGoForward = $state();
    let canGoBack = $state();

    onMount(() => {
        canGoForward = navigation.canGoForward;
        canGoBack = navigation.canGoBack;

        afterNavigate(() => {
            canGoForward = navigation.canGoForward;
            canGoBack = navigation.canGoBack;
        });
    });
</script>

<div class="upBar">
    <div style="margin-right: 8px; display: flex;">
        <button
            class={!canGoBack ? "unpressable" : ""}
            onclick={() => {
                navigation.back();
            }}
            style="border-top-right-radius: 0px; border-bottom-right-radius: 0px; border-right: none;"
        >
            <img
                style="transform: scaleX(-1);"
                src="/assets/arrow.png"
                alt=""
            />
        </button>

        <button
            class={!canGoForward ? "unpressable" : ""}
            onclick={() => {
                navigation.forward();
            }}
            style="border-top-left-radius: 0px; border-bottom-left-radius: 0px; border-left: 0px;"
        >
            <img src="/assets/arrow.png" alt="" />
        </button>
    </div>

    <SearchBar />

    {#each customData as button, i}
        <button
            in:fly={{ x: -50, delay: 100 * (i + 1) }}
            style="margin-left: 8px; width: fit-content; padding-left: 8px; padding-right: 8px;"
            onclick={() => {
                button.onclick();
            }}
        >
            {button.text}
        </button>
    {/each}
</div>

<style>
    .upBar {
        display: flex;
    }

    .unpressable {
        opacity: 0.5;
        pointer-events: none;
    }

    button {
        border: 1px solid rgba(255, 255, 255, 0.3);
        background: rgba(255, 255, 255, 0.1);
        height: 42px;
        width: 42px;

        border-radius: 17px;

        padding: 0px;
        margin-right: 0px;

        backdrop-filter: blur(10px) brightness(0.6);
        cursor: pointer;

        color: rgba(255, 255, 255, 0.7);
        font-size: 15px;
        font-weight: 900;
    }

    button:hover {
        background: rgba(255, 255, 255, 0.2);
    }
    button:hover img {
        opacity: 1;
    }

    button img {
        width: 25px;

        margin-left: 1px;
        margin-top: 3px;

        opacity: 0.7;
    }
</style>
