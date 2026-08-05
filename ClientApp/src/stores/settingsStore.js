import { writable } from "svelte/store";

export let settings = writable({});

export async function GetSettings() {
    settings.set(
        JSON.parse(await window.electron.ipcRenderer.lolloInvoke("getSettings"))
    );
}

const nonLoggedData = {
    imgUrl: "",
    name: "Guest",
    username: "",
    logged: false
};

export let accountData = writable(nonLoggedData);

export async function setDefault() {
    accountData.set(nonLoggedData);
}

export function isLogged() {
    return accountData.logged;
}
