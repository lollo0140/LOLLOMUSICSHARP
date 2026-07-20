import { goto } from "$app/navigation";

export async function NavigateTo(route, params, refresh = false) {
    let stringParams = "";

    if (Array.isArray(params) && params.length > 0) {

        stringParams += "?" + params.join("&");

    }

    // @ts-ignore

    if (route.includes("/artists") || refresh) {
        await goto("/load");
        await goto(route + stringParams, { replaceState: true });
    } else {
        await goto(route + stringParams);
    }



}

export async function NavigateToArtist(browseId) {
    NavigateTo("/artists", [`browseid=${browseId}`])
}

export async function NavigateToAlbum(browseId) {
    NavigateTo("/album", [`browseid=${browseId}`])
}
