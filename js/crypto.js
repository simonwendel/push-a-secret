export { generate, encrypt }

import {strToUTF8Arr, base64EncArr} from '/mdn.b64.js'

const crypto = window.crypto.subtle;

const settings = {
    key: {
        name: 'AES-GCM',
        length: 128,
        persisted: 'A128GCM'
    },
    format: 'jwk',
    uses: ['encrypt', 'decrypt'],
    extractable: true
};

const getInitializationVector = () => window.crypto.getRandomValues(new Uint8Array(12))

const constructJwk = key => Object.create({
    alg: key.algorithm,
    ext: settings.extractable,
    k: key.key,
    key_ops: settings.uses,
    kty: 'oct'
});

const generate = async () => {
    const key = await crypto.generateKey(
        settings.key,
        settings.extractable,
        settings.uses);

    return await crypto.exportKey(settings.format, key);
}

const encrypt = async request => {
    const jwk = constructJwk(request.key);
    const key = await crypto.importKey(
        settings.format,
        jwk,
        settings.key.name,
        settings.extractable,
        settings.uses);

    const clearBuffer = strToUTF8Arr(request.cleartext);

    const ivBuffer = getInitializationVector();
    const cipherBuffer = await crypto.encrypt({ name: settings.key.name, iv: ivBuffer }, key, clearBuffer);

    const iv = base64EncArr(ivBuffer);
    const ciphertext = base64EncArr(new Uint8Array(cipherBuffer));

    return {
        iv: iv,
        ciphertext: ciphertext
    };
}
