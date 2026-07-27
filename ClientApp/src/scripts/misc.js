export function SecTotimeDisplay(seconds) {

    const ore = Math.floor(seconds / 3600);
    const minuti = Math.floor((seconds % 3600) / 60);
    const secondi = seconds % 60;

    const formatoOre = String(ore).padStart(2, "0");
    const formatoMinuti = String(minuti).padStart(2, "0");
    const formatoSecondi = String(secondi).padStart(2, "0");

    return `${formatoOre}:${formatoMinuti}:${formatoSecondi}`;

}