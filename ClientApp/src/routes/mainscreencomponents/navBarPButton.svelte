<script>
    import { onMount } from "svelte";
    import { NavigateTo } from "../../scripts/navigationScript";
    import { GetDefPng } from "../../scripts/defPngManager";

    let { content } = $props();

    let imgSrc = $state();

    onMount(() => {
        console.log(content);
        imgSrc = content.thumbnails[0];
    });
</script>

<button
    onclick={() => {
        NavigateTo("/playlist", [`browseid=${content.browseId}`], true);
    }}
>
    <img src={imgSrc} onerror={() => {imgSrc = "/assets/defpng/def_playlist_icon.png"}} alt="" />
</button>

<style>
    button {
        width: 55px;
        height: 50px;

        border: none;
        background: transparent;

        cursor: pointer;

        opacity: 0.7;
    }

    button:hover {
        opacity: 1;
    }

    img {
        object-fit: cover;

        width: 40px;
        height: 40px;

        border: solid 1px rgba(255, 255, 255, 0.3);
        border-radius: 10px;
    }
</style>
