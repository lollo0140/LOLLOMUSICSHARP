<script>
    let { label = "none", children } = $props();
</script>

<div class="optionButton">
    <p class="label">{label}:</p>

    <div class="content">{@render children()}</div>
</div>

<style>
    .label {
        color: white;
        white-space: nowrap;
        width: fit-content;
        font-weight: 800;
    }

    .optionButton {
        width: 100%;

        display: flex;
        flex-direction: row;

        gap: 10px;

        border-bottom: rgba(255, 255, 255, 0.3) 1px solid;
    }

    .content {
        display: flex;
        flex-direction: row;

        justify-content: space-between;
        align-items: center;

        width: 100%;
    }
</style>
