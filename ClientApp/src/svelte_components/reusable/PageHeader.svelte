<script>
    let { children, bgImmage = "", label = "none" } = $props();

    let imgTag = $state();

</script>

<div class="PL-header">
    <img bind:this={imgTag} class="PL-H-Img" src={bgImmage} alt="" onerror={ () => {
        imgTag.style.opacity = 0;
    }}/>

    <div class="content-render">
        <p class="C-label">{label}</p>
        <p class="Custom-content">
            {@render children()}
        </p>
    </div>
</div>

<style>
    .Custom-content {
        display: flex;
        flex-direction: column;

        align-items: start;

        justify-content: space-between;
    }

    .C-label {
        color: white;

        opacity: 0.8;

        font-size: 24px;
        font-weight: 800;

    }

    .content-render {
        margin-left: 20px;

        top: 20px;
        left: 20px;

        width: 100%;
        height: 100%;
    }

    .PL-H-Img {
        position: absolute;
        right: 0px;
        top: 0px;

        height: 100%;
        width: 70%;
        border-radius: 20px;

        border: none;
        outline: none;

        object-fit: cover;
        pointer-events: none;
        -webkit-mask-image: linear-gradient(to right, transparent, black);
    }

    .PL-header {
        position: relative;

        margin: 0px;
        top: 0px;

        border-radius: 20px;

        width: 100%;
        height: fit-content;
        min-height: 430px;

        display: flex;
    }
</style>
