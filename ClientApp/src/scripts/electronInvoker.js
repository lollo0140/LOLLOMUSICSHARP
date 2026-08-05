export async function EInvoke(funcName, ...params) {
    return await window.electron.ipcRenderer.lolloInvoke(funcName, ...params);
}

export async function EInvokeJSON(funcName, ...params) {
    const response = await EInvoke(funcName, ...params);
    return JSON.parse(response);
}

export function EOn(funcName, listener) {
    return window.electron.ipcRenderer.on(funcName, (event, ...args) => listener(event, ...args));
}

export function ESend(funcName, ...params) {
    window.electron.ipcRenderer.send(funcName, ...params);
}
