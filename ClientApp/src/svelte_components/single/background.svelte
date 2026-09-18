<script module>

    let backgroundStyle = $state("filter: blur(40px) brightness(0.3);")
    export function SetFilters(opacity, blur) {
        backgroundStyle = `filter: blur(${blur}px) brightness(${opacity});`
    }

</script>

<script>
    import { index, queue } from "../../routes/audioPlayer/playerStore";
    import { GetImmageUrl } from "../../scripts/immages";

    let current = $derived($queue[$index]);
</script>

<div class="bg-wrapper" style="{backgroundStyle}">
    <div class="bg-animation" style="">
        {#if current != undefined}
            <img class="bg-img" src={GetImmageUrl(current.thumbnails[1], 300)} alt="" />
            <img class="bg-img" src={GetImmageUrl(current.thumbnails[1], 300)} alt="" />
        {/if}
    </div>
</div>

<style>
    .bg-animation {

        display: flex;
        flex-direction: row;

        position: absolute;

        left: 50%;
        top: 50%;

        transform: translate(-50%, -50%) scale(3);

        width: 1410px;
        height: 700px;

        transition: all 500ms;
    }

    @keyframes rotating {
        from {
            transform: rotateZ(0deg);
        }
        to {
            transform: rotateZ(365deg);
        }
    }

    .bg-wrapper {
        position: absolute;

        z-index: -1;

        width: 100%;
        height: 100%;

        overflow: hidden;

        border-radius: 39px;

        opacity: 1;


    }

    .bg-img {
        width: 700px;
        height: 700px;

        object-fit: cover;

        background: transparent;

        animation: infinite linear rotating 140s;

        border: none;
    }
</style>
