import { writable } from "svelte/store";


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
