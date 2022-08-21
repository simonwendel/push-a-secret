// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

export { generate, encrypt, decrypt }

import { strToUTF8Arr, UTF8ArrToStr, base64EncArr, base64DecToArr } from '/mdn.b64.js'

const subtle = window.crypto.subtle;
const settings = {
    key: {
        'A128GCM': {
            name: 'AES-GCM',
            length: 128
        }
    },
    defaultAlgorithm: 'A128GCM',
    format: 'jwk',
    uses: ['encrypt', 'decrypt'],
    extractable: true
};

const getInitializationVector =
    () => window.crypto.getRandomValues(new Uint8Array(12));

const constructJwk = key => Object.create({
    alg: key.algorithm,
    ext: settings.extractable,
    k: key.key,
    key_ops: settings.uses,
    kty: 'oct'
});

const generate = async () => {
    const key = await subtle.generateKey(
        settings.key[settings.defaultAlgorithm],
        settings.extractable,
        settings.uses);

    return await subtle.exportKey(settings.format, key);
};

const encrypt = async request => {
    const jwk = constructJwk(request.key);
    const key = await subtle.importKey(
        settings.format,
        jwk,
        settings.key[settings.defaultAlgorithm].name,
        settings.extractable,
        settings.uses);

    const clearBuffer = strToUTF8Arr(request.cleartext);

    const ivBuffer = getInitializationVector();
    const cipherBuffer = await subtle.encrypt({
        name: settings.key[settings.defaultAlgorithm].name,
        iv: ivBuffer
        },
        key,
        clearBuffer);

    const iv = base64EncArr(ivBuffer);
    const ciphertext = base64EncArr(new Uint8Array(cipherBuffer));

    return {
        iv: iv,
        ciphertext: ciphertext
    };
};

const decrypt = async request => {
    const keySettings = settings.key[request.key.algorithm];
    if (keySettings == undefined) {
        throw 'Can\'t look up settings for algorithm: ' + request.key.algorithm;
    }

    const jwk = constructJwk(request.key);
    const key = await subtle.importKey(
        settings.format,
        jwk,
        keySettings.name,
        settings.extractable,
        settings.uses);

    const ivBuffer = base64DecToArr(request.iv);
    const cipherBuffer = base64DecToArr(request.ciphertext);

    const clearBuffer = await subtle.decrypt({ name: keySettings.name, iv: ivBuffer }, key, cipherBuffer);
    const cleartext = UTF8ArrToStr(new Uint8Array(clearBuffer));
    return {
        cleartext: cleartext
    }
};
