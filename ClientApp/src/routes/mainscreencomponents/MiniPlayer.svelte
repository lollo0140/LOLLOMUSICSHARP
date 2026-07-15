<script>

    let { openCommand } = $props();

</script>


<div class="mini-player">

    <div></div>

    <button class="open-button" onclick={() => openCommand()}>
        <img src="/assets/resize.png" alt="">
    </button>

</div>

<style>

    .mini-player {
        position: absolute;

        display: flex;
        flex-direction: row;

        width: 100%;
        height: 100%;

        margin: 0px;
        padding: 0px;

        justify-content: space-between;
    }

    .open-button {
        margin: 3px;
        height: calc(100% - 6px);
        width: 45px;

        border-radius: 40px;

        background: none;
        border: none;
        display: flex;

        align-items: center;

        justify-content: center;

        cursor: pointer;

        opacity: 0.5;
    }

    .open-button:hover {
        opacity: 0.8;
    }



</style>
