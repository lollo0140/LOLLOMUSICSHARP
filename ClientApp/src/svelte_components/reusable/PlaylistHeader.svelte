<script>
    import { GetDefPng } from "../../scripts/defPngManager";
    import { NavigateTo } from "../../scripts/navigationScript";
    import SongListRenderer from "./SongListRenderer.svelte";

    let { content, type } = $props();

    let ImmageSrc = $derived.by(() => {
        if (type === "album") {
            return content.data.thumbnails[content.data.thumbnails.length - 1];
        } else if (type === "playlist") {
            return content.data.thumbnail;
        }
    });

    let DefPng = $derived.by(async () => {
        return await GetDefPng(type === "album");
    });
</script>

{#if content != undefined}
    <div class="data">
        <img
            src={ImmageSrc}
            onerror={() => {
                ImmageSrc = DefPng;
            }}
            alt=""
        />
        <div class="texts">
            <p class="type">{type.toUpperCase()}</p>

            <p class="title">{content.data.title}</p>

            {#if content?.data?.artist?.artistId && content?.data?.artist?.artistName}
                <!-- svelte-ignore node_invalid_placement_ssr -->
                <button
                    class="link"
                    onclick={(e) => {
                        e.stopPropagation();
                        NavigateTo("/artists", [
                            `browseid=${content.data.artist.artistId}`,
                        ]);
                    }}
                >
                    {content.data.artist.artistName}</button
                >
            {/if}

            {#if content?.data?.description}
                <p class="description">{content.data.description}</p>
            {/if}
        </div>
    </div>

    <div class="content">
        <SongListRenderer
            content={content.items}
            renderPhoto={type === "album" ? false : true}
        />
    </div>
{/if}

<style>
    .link {
        background: none;
        border: none;
        width: auto;
        height: auto;

        color: rgba(255, 255, 255, 0.7);
        font-weight: 800;

        font-size: 35px;

        backdrop-filter: none;

        cursor: pointer;
    }

    .link:hover {
        transform: translateX(0px);
        box-shadow: none;

        text-decoration: underline;
    }

    .content {
        margin-top: 5px;
    }

    .description {
        font-size: 25px;
        font-weight: 800;
        opacity: 0.7;
    }

    .texts {
        position: absolute;

        left: 20px;
        top: 20px;
    }

    .texts p {
        margin: 0px;

        color: white;
    }

    .type {
        opacity: 0.4;
    }

    .title {
        font-size: 70px;
        font-weight: 900;

        margin: 0px;
    }

    a:link {
        color: #ffffff;

        font-size: 30px;
        font-weight: 800;

        opacity: 0.7;
    }

    /* 2. Stato visitato (link già cliccati in passato) */
    a:visited {
        color: #ffffff;
    }

    /* 3. Stato Hover (quando ci passi sopra con il mouse) */
    a:hover {
        color: #ffffffa7;
    }

    /* 4. Stato Attivo (mentre l'utente ci sta cliccando sopra) */
    a:active {
        color: #e74c3c;
    }

    .data {
        position: relative;

        border: 1px solid rgba(255, 255, 255, 0.3);
        background: rgba(255, 255, 255, 0.1);

        overflow: hidden;
        border-radius: 16px;

        height: 400px;
    }

    .data img {
        position: absolute;

        right: 0px;
        top: 50%;
        transform: translateY(-50%);

        width: 60%;

        object-fit: contain;

        -webkit-mask-image: linear-gradient(to left, black, transparent);
        mask-image: linear-gradient(to left, black, transparent);
    }
</style>
