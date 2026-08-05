import { accountData, GetSettings } from "../stores/settingsStore";
import { EInvoke, EInvokeJSON, EOn } from "./electronInvoker";
import { closeContextMenu, forceCloseMenu } from "../routes/ContextMenu.svelte";
import { setLocalSongs, setUpLikedList } from "../stores/songDataBase";

export async function SetupLolloMusic() {
    // 1. Funzione isolata per gestire il login e il caricamento dei dati
    const handleLoginCheck = async () => {
        try {
            const logged = await EInvoke("loginYT");

            if (logged) {
                const logInfo = await EInvokeJSON("getLogInfo");

                const logData = {
                    imgUrl: logInfo.imgUrl,
                    name: logInfo.name,
                    username: logInfo.username,
                    logged: true,
                };

                accountData.set(logData);
                console.log("Account caricato:", logData);

                await setUpLikedList();
            }
        } catch (error) {
            console.error("Errore durante il controllo del login:", error);
            await setLocalSongs();
        }
    };

    EOn("reloadLogInfo", () => {
        handleLoginCheck();
    });

    await handleLoginCheck();

    await GetSettings();

    setTimeout(() => {
        setLocalSongs();
    }, 500);

    document.addEventListener("click", (e) => {
        closeContextMenu(e);
    });

    document.addEventListener(
        "scroll",
        () => {
            forceCloseMenu();
        },
        true
    );
}
