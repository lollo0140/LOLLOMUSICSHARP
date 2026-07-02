const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electron', {
    ipcRenderer: {
        send: (channel, data) => ipcRenderer.send(channel, data),
        on: (channel, func) => ipcRenderer.on(channel, (event, ...args) => func(...args)),
        removeListener: (channel, func) => ipcRenderer.removeListener(channel, func),
        removeAllListeners: (channel) => ipcRenderer.removeAllListeners(channel),
        lolloInvoke: (channel, ...args) => {
            return new Promise((resolve) => {
                const requestId = Math.random().toString(36).substring(2, 9);
                const responseChannel = `${channel}-reply-${requestId}`;

                ipcRenderer.once(responseChannel, (event, result) => {
                    resolve(result[0]);
                });
                ipcRenderer.send(channel, { requestId, payload: args });
            });
        }
    }
});
