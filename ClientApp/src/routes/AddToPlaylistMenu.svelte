<script>
    import { onMount } from "svelte";

    onMount(() => {

    })
</script>

<main>

    <div class="add-to-playlist">

    </div>


</main>

<style>

    .add-to-playlist {
        background: rgba(255, 255, 255, 0.1);
        border: rgba(255, 255, 255, 0.3) 1px solid;
        border-radius: 20px;

        position: absolute;

        width: 450px;
        height: 510px;

        left: 50%;
        top: 50%;
        transform: translate(-50%, -50%);


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

</style>
