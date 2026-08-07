<script>
    import { goto } from "$app/navigation";
</script>

<button
    onclick={() => {
        goto("/downloaded");
    }}
>
    <div class="Bcontent">
        <img
            src="/assets/defpng/def_playlist_icon.png"
            onerror={() => {
                imgError();
            }}
            draggable="false"
            alt=""
        />

        <div
            class="title-strip"
            style="display: flex; flex-direction: row; align-items: center;"
        >
            <p class="title" style="font-size:18px;">DOWNLOADED</p>
        </div>

        <div class="subtitle-div">
            <p class="subtitle">PLAYLIST</p>
        </div>
    </div>
</button>

<style>
    .Bcontent {
        background: transparent;

        height: 250px;

        display: flex;
        flex-direction: column;
        gap: 3px;

        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    }

    button img {
        height: 160px;
        border-radius: 10px;

        pointer-events: none;
    }

    button {
        position: relative;

        color: white;
        background: none;
        border: none;
        margin: 5px;

        cursor: pointer;
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    }

    button:hover {
        transform: translateY(-4px);
    }

    .title {
        text-align: start;

        margin: 0px;

        font-weight: 900;

        min-height: 25px;
        width: 155px;

        pointer-events: none;
    }

    .dot-divider {
        margin: 0px;

        margin-left: 5px !important;
        margin-right: 5px !important;
    }

    .subtitle-div {
        display: flex;
        align-items: center;

        flex-wrap: wrap;

        font-weight: 700;
        font-size: 15px;

        text-align: start;

        height: 17px;
        width: 155px;

        margin: 0px;

        opacity: 0.5;
    }

    .subtitle-div p {
        text-align: start;
        width: fit-content;
        margin: 0px;
    }

    .subtitle-div button {
        margin: 0px;

        padding: 0px;
        font-weight: 700;
        font-size: 15px;

        text-wrap: nowrap;
    }

    .subtitle-div button:hover {
        transform: translateY(0px);

        text-decoration: underline;
        font-weight: 900;
    }
</style>
