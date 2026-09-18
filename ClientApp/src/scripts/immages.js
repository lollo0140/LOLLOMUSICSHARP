export function GetImmageUrl(url, size, cache = false) {
    if (!url) return "";

    const saveString = `?save=${cache}`;

    // Track Image
    if (url.includes("https://yt3.googleusercontent.com/")) {
        const parsed = url.replace("https://yt3.googleusercontent.com/", "");
        return `http://localhost:8001/api/img/${size}/${encodeURIComponent(parsed)}${saveString}`;
    }

    // Video Image
    if (url.includes("https://i.ytimg.com/vi/")) {
        const parsed = url.replace("https://i.ytimg.com/vi/", "");
        return `http://localhost:8001/api/videoimg/${size}/${encodeURIComponent(parsed)}${saveString}`;
    }

    return url;
}
