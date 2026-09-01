import { accountData, GetSettings } from "../stores/settingsStore";
import { EInvoke, EInvokeJSON, EOn } from "./electronInvoker";
import { closeContextMenu, forceCloseMenu } from "../routes/ContextMenu.svelte";
import { setLocalSongs, setUpLikedList } from "../stores/songDataBase";

export async function SetupLolloMusic() {

    EOn("reloadLogInfo", () => {
        handleLoginCheck();
    });

    const handleLoginCheck = async () => {
        console.log("handling log");

        try {


            const logged = await EInvoke("loginYT");
            console.log(logged);



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

                console.log("Loading Liked Songs");
                await setUpLikedList();
            }
        } catch (error) {
            console.error("Errore durante il controllo del login:", error);
        }
    };

    setLocalSongs();

    handleLoginCheck();


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
