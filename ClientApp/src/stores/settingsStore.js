import { writable } from "svelte/store";
import { EInvokeJSON } from "../scripts/electronInvoker";

export let settings = writable({});

export async function GetSettings() {
    settings.set(await EInvokeJSON("getSettings"));
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
