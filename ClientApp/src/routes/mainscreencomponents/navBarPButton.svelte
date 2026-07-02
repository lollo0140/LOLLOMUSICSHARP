<script>
    import { onMount } from "svelte";
    import { NavigateTo } from "../../scripts/navigationScript";

    let { content } = $props();

    onMount(() => {
        console.log(content);
    });
</script>

<button
    onclick={() => {
        NavigateTo("/playlist", [`browseid=${content.browseId}`], true);
    }}
>
    <img src={content.thumbnails[0]} alt="" />
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
