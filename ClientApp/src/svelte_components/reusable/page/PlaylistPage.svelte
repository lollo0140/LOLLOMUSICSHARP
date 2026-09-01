<script>
    import { page } from "$app/state";
    import { onMount } from "svelte";
    import { fly } from "svelte/transition";
    import { openPageContextMenu } from "../../../routes/ContextMenu.svelte";
    import { render } from "svelte/server";
    import LoadingAnimation from "../LoadingAnimation.svelte";
    import SongListRenderer from "../SongListRenderer.svelte";
    import PageHeader from "../PageHeader.svelte";

    let { children, content } = $props();

    let pageScroll = $state(0);
    let mainElement = $state(null);

    let headerHeight = $derived(
        document.getElementById("header-content").clientHeight,
    );

    function handleScroll(e) {
        pageScroll = e.currentTarget.scrollTop - headerHeight;
    }
</script>

{#if content != undefined}
    <main bind:this={mainElement} onscroll={handleScroll} in:fly={{ y: -50 }}>
        <div id="header-content">
            <PageHeader
                bgImmage={content.data.thumbnails[
                    content.data.thumbnails.length - 1
                ]}
                label={"PLAYLIST"}
            >
                {@render children()}
            </PageHeader>
        </div>

        <div class="PL-elements">
            <SongListRenderer
                scroll={pageScroll}
                content={content.items}
                playlistId={content.id}
                from={(content.data.title ?? "").toUpperCase()}
            />
        </div>

        <div class="spacer"></div>
    </main>
{:else}
    <div style="height: min-content;">
        <LoadingAnimation />
    </div>
{/if}

<style>
    .PL-elements {
        margin-top: 5px;
    }

    main {
        height: 100%;
        overflow-y: scroll;
        scrollbar-width: none !important;
        overflow-x: hidden;
    }
</style>
