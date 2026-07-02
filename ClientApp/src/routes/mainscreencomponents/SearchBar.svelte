<script>
    import { SPageSearch } from "../search/+page.svelte";
    import { NavigateTo } from "../../scripts/navigationScript.js";

    let searchKey = $state("");
</script>

<div class="bar">
    <input
        bind:value={searchKey}
        placeholder="search something"
        type="text"
        onkeydown={(event) => {
            if (event.key === "Enter") {
                NavigateTo("/search");
                SPageSearch(searchKey);
            }
        }}
    />
    <button
        onclick={() => {
            NavigateTo("/search");
            SPageSearch(searchKey);
        }}
    >
        <img src="/assets/navbar/search.png" alt="" />
    </button>
</div>

<style>
    .bar {
        border: 1px solid rgba(255, 255, 255, 0.3);
        background: rgba(255, 255, 255, 0.1);

        height: 40px;
        width: 300px;

        border-radius: 17px;

        backdrop-filter: blur(10px) brightness(0.6);

        display: flex;
    }

    input {
        height: 29px;

        position: relative;

        left: 5px;
        top: 5px;
        bottom: 5px;
        right: 30px;

        background: transparent;
        border: none;

        outline: none;
        color: white;
        font-weight: 900;

        font-size: 17px;
    }

    img {
        position: relative;

        margin: 5px;

        height: 30px;
        width: 30px;

        background: transparent;
    }

    button {
        position: relative;

        left: 20px;

        width: 40px;
        height: 40px;
        padding: 0px;

        border: none;
        background: transparent;

        cursor: pointer;

        opacity: 0.7;
    }

    button:hover {
        opacity: 1;
    }
</style>
