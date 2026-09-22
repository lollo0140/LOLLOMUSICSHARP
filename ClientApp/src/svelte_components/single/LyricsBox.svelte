<script module>
    let currentTimeValue = $state(0);

    export function SetLyricsCurrentTime(sec) {
        currentTimeValue = Math.floor(sec);
    }
</script>

<script>
    import { SetCurrentTimelineSec } from "../../routes/audioPlayer/audioPlayer.svelte";

    import { index, queue } from "../../routes/audioPlayer/playerStore";
    import { EInvokeJSON } from "../../scripts/electronInvoker";

    let show = $state(false);
    let hovering = $state(false);

    let currentSong = $derived($queue?.[$index]);
    let lyr = $state(undefined);

    // Riferimento al tag <main> per gestire lo scroll interno
    let mainElement = $state(null);

    // Caricamento testo
    $effect(async () => {
        const tit = currentSong?.title ?? "";
        const art = currentSong?.artists?.[0]?.artistName ?? "";
        const album = currentSong?.album?.titleName ?? "";

        if (!tit) return;

        lyr = undefined;
        show = false;

        lyr = await EInvokeJSON(
            "getLyrics",
            tit.toLowerCase(),
            art.toLowerCase(),
            album !== tit ? album.toLowerCase() : "",
        );

        console.log(lyr);

        if (lyr.syncedLyrics.length > 0) {
            show = true;
        }
    });

    // Auto-scroll isolato sul contenitore <main>
    $effect(() => {
        if (!mainElement || !lyr?.syncedLyrics) return;

        // Leggiamo la variabile reattiva
        const currentTime = currentTimeValue;

        // Troviamo tutti gli elementi <p> all'interno del main
        const paragraphs = mainElement.querySelectorAll("p.active");

        if (paragraphs.length > 0) {
            // L'ultimo paragrafo attivo è quello del verso corrente
            const lastActiveP = paragraphs[paragraphs.length - 1];
            const activeButton = lastActiveP.closest("button");

            if (activeButton) {
                // Calcoliamo la posizione del pulsante RISPETTO AL MAIN (senza toccare la pagina)
                const targetScrollTop =
                    activeButton.offsetTop -
                    mainElement.clientHeight / 2 +
                    activeButton.clientHeight / 2;

                // Applichiamo lo scroll SOLO ed ESCLUSIVAMENTE al nodo <main>
                if (!hovering) {
                    mainElement.scrollTo({
                        top: Math.max(0, targetScrollTop),
                        behavior: "smooth",
                    });
                }
            }
        }
    });
</script>

<!-- Utilizziamo bind:this per catturare il nodo DOM del main -->

<p class="label" style="display: {show ? 'block' : 'none'};">LYRICS</p>
<main
    onmouseenter={(hovering = true)}
    onmouseleave={(hovering = false)}
    class={show ? "open" : ""}
    bind:this={mainElement}
>
    {#if lyr?.syncedLyrics && lyr.syncedLyrics.length > 0}
        {#each lyr.syncedLyrics as l}
            <button
                onclick={() => {
                    SetCurrentTimelineSec(l.seconds);
                }}
            >
                <p class:active={l.seconds <= currentTimeValue}>
                    {l.content}
                </p>
            </button>
        {/each}
    {/if}
</main>

<style>
    .label {
        margin-top: 15px;
        margin-bottom: 0px;
        font-weight: 900;
        opacity: 0.7;
    }

    .open {
        position: relative; /* Importante per calcolare offsetTop correttamente */
        border: 1px solid rgba(255, 255, 255, 0.1);
        background: rgba(255, 255, 255, 0.05);
        transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275) !important;
        height: 400px;
        border-radius: 15px;
        overflow-y: auto;
        scrollbar-width: none;

        display: flex;
        flex-direction: column;
        gap: 10px;
        padding: 10px;
    }

    button {
        text-align: left;
        background: none;
        border: none;
        min-height: 80px;
        width: 100%;
        cursor: pointer;
    }

    main::-webkit-scrollbar {
        display: none;
    }

    p {
        opacity: 0.2;
        transition:
            opacity 0.2s ease,
            color 0.2s ease;
        margin: 4px 0;
        color: white;
        font-size: 18px;
    }

    p.active {
        opacity: 1;
        color: #fff;
        font-weight: 800;
    }
</style>
