const pendingCalls = new Map();
let isListening = false;

function getWebviewApi() {
    const webview = globalThis?.chrome?.webview;
    if (!webview || typeof webview.postMessage !== 'function' || typeof webview.addEventListener !== 'function') {
        throw new Error('WebView2 host bridge is unavailable. Ensure the app is running inside WebView2.');
    }

    return webview;
}

function normalizeArgs(args) {
    return Array.isArray(args) ? args : [];
}

function onMessage(event) {
    let payload = event?.data;

    if (typeof payload === 'string') {
        try {
            payload = JSON.parse(payload);
        } catch {
            return;
        }
    }

    if (!payload || !payload.callId) {
        return;
    }

    const pendingCall = pendingCalls.get(payload.callId);
    if (!pendingCall) {
        return;
    }

    pendingCalls.delete(payload.callId);

    if (payload.error) {
        pendingCall.reject(new Error(payload.error));
        return;
    }

    pendingCall.resolve(payload.result);
}

function ensureListener() {
    if (isListening) {
        return;
    }

    const webview = getWebviewApi();
    webview.addEventListener('message', onMessage);
    isListening = true;
}

function generateCallId() {
    return `${Date.now().toString(36)}-${Math.random().toString(36).slice(2)}`;
}

function invokeWithInternalBridge(methodName, args) {
    if (!methodName || typeof methodName !== 'string') {
        throw new Error('A non-empty host method name is required.');
    }

    ensureListener();

    return new Promise((resolve, reject) => {
        const callId = generateCallId();
        pendingCalls.set(callId, { resolve, reject });

        getWebviewApi().postMessage(JSON.stringify({
            callId,
            method: methodName,
            args: normalizeArgs(args)
        }));
    });
}

export function isAvailable() {
    const webview = globalThis?.chrome?.webview;
    return Boolean(webview && typeof webview.postMessage === 'function' && typeof webview.addEventListener === 'function');
}

export function installGlobalBridge() {
    if (typeof globalThis.InvokeHostMethod === 'function') {
        return;
    }

    globalThis.InvokeHostMethod = function (methodName, ...args) {
        return invokeWithInternalBridge(methodName, args);
    };
}

export function invokeHostMethod(methodName, args) {
    return invokeWithInternalBridge(methodName, normalizeArgs(args));
}