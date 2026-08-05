<script>
    import { render } from "svelte/server";


    let {children, title = "SECTION"} = $props();

</script>

<div class="setting-div">
    <p class="setting-label">{title}</p>

    <div class="setting-content">
        {@render children()}
    </div>


</div>

<style>
    .setting-div {

        padding-top: 20px;
        padding-bottom: 20px;

        border-radius: 10px;
        border-bottom: 1px solid rgba(255, 255, 255, 0.2);

    }

    .setting-content {
        display: flex;
        flex-direction: row;
    }

    .setting-label {
        color: white;
        font-size: 23px;

        font-weight: 800;

        opacity: 0.7;
    }
</style>
