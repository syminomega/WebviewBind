// webview-bridge.js
// Exposes InvokeHostMethod() for calling C# methods marked with [WebviewExport] from JavaScript.
// Load business scripts only after webview-bridge.js is loaded.

(function () {
    'use strict';

    // Pending callback map: callId => { resolve, reject }
    const _pending = new Map();

    // Listen for response messages from the C# host
    window.chrome.webview.addEventListener('message', function (event) {
        let data;
        try {
            data = JSON.parse(event.data);
        } catch (e) {
            return; // Ignore non-JSON messages
        }

        if (!data || !data.callId) return;

        const entry = _pending.get(data.callId);
        if (!entry) return;

        _pending.delete(data.callId);

        if (data.error) {
            entry.reject(new Error(data.error));
        } else {
            entry.resolve(data.result);
        }
    });

    /**
    * Calls a C# method marked with [WebviewExport].
     *
    * @param {string}  methodName  C# method name (same as the [WebviewExport] method)
    * @param {...any}  args        Arguments mapped one-to-one with the C# method parameters
    * @returns {Promise<any>}      Resolves to the C# method return value; rejects if the method throws
     *
     * @example
     *   const greeting = await InvokeHostMethod('SayHello', 'World');
     */
    window.InvokeHostMethod = function (methodName, ...args) {
        return new Promise(function (resolve, reject) {
            // Generate a unique call ID: timestamp + random suffix
            const callId = Date.now().toString(36) + Math.random().toString(36).slice(2);
            _pending.set(callId, { resolve, reject });
            window.chrome.webview.postMessage(
                JSON.stringify({ callId, method: methodName, args })
            );
        });
    };
}());
