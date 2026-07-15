<script>
    import { SPageSearch } from "../search/+page.svelte";
    import { NavigateTo } from "../../scripts/navigationScript.js";
    import { fly } from "svelte/transition";

    let searchKey = $state();

    let selectionIndex = $state(0);
    let selecting = $state(false);

    let sugg = $state([]);
</script>

<div class="bar">
    <input
        bind:value={searchKey}
        onblur={() => {
            setTimeout(() => {
                searchKey = "";
                selectionIndex = 0;
                selecting = false;
                sugg = [];
            }, 150);
        }}
        placeholder="search something"
        type="text"
        oninput={async () => {
            sugg = [];

            sugg = JSON.parse(
                await window.electron.ipcRenderer.lolloInvoke(
                    "getSearchSugg",
                    searchKey,
                ),
            );
        }}
        onkeydown={(event) => {
            //ArrowUp
            //ArrowDown

            if (event.key === "ArrowUp") {
                selecting = true;
                event.preventDefault();
                if (selectionIndex < 1) {
                    selectionIndex = sugg.length - 1;
                } else {
                    selectionIndex--;
                }

                console.log(selectionIndex);
                console.log(sugg.length);
            }
            if (event.key === "ArrowDown") {
                selecting = true;
                event.preventDefault();
                if (selectionIndex > sugg.length - 2) {
                    selectionIndex = 0;
                } else {
                    selectionIndex++;
                }

                console.log(selectionIndex);
                console.log(sugg.length);
            }

            console.log();

            if (event.key === "Enter") {
                if (selecting) {
                    NavigateTo("/search");
                    SPageSearch(
                        sugg[selectionIndex].query +
                            sugg[selectionIndex].completition,
                    );
                } else {
                    NavigateTo("/search");
                    SPageSearch(searchKey);
                }

                selecting = false;
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

<div class="suggestions-container" style="{sugg.length > 0 ? "opacity:1; pointer-events: all;" : "opacity:0; pointer-events: none;"}">
    {#each sugg as S, i}
        <button
            class="sug-button {selectionIndex === i ? 'sugg-active' : ''}"
            onclick={() => {
                NavigateTo("/search");
                searchKey = S.query + S.completition;
                SPageSearch(S.query + S.completition);
            }}
            in:fly={{ y: -5, delay: 50 * i }}
        >
            <p style="font-weight: 900;">{S.query}</p>
            <p style="opacity: 0.7;">{S.completition}</p>
        </button>
    {/each}
</div>

<style>
    .sugg-active {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 20px;


    }

    .suggestions-container {
        position: absolute;
        z-index: 99;

        left: 97px;
        top: 47px;

        width: 302px;
        min-height: 200px;

        border: 1px solid rgba(255, 255, 255, 0.1);
        background: rgba(255, 255, 255, 0.05);
        border-radius: 20px;

        backdrop-filter: blur(40px) brightness(0.6);

        opacity: 1;
    }

    .sug-button {
        left: 0px;

        color: white;
        opacity: 1;

        display: flex;
        flex-direction: row;

        justify-content: start;
        align-items: center;
        width: 100%;

        padding-left: 15px;

        font-size: 17px;

        flex-wrap: nowrap;
    }

    .sug-button:hover {
        text-decoration: underline;
    }

    .bar {
        border: 1px solid rgba(255, 255, 255, 0.1);
        background: rgba(255, 255, 255, 0.05);

        height: 40px;
        width: 300px;

        border-radius: 30px;

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

        height: 20px;
        width: 20px;

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

        opacity: 0.5;
    }

    button:hover {
        opacity: 1;
    }
</style>
