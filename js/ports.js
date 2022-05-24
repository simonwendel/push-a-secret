export { wireCrypto, wireStorage }

import { retrieve, store } from './storage.js';
import { generate, encrypt } from '/crypto.js'

const wireCrypto = app => {
    app.ports.requestKey.subscribe(async () => {
        const exported = await generate();
        app.ports.receiveKey.send({
            algorithm: exported.alg,
            key: exported.k
        });
    });

    app.ports.requestEncryption.subscribe(async request => {
        const encrypted = await encrypt(request);
        app.ports.receiveEncryption.send({
            iv: encrypted.iv,
            ciphertext: encrypted.ciphertext
        });
    });
}

const wireStorage = app => {
    app.ports.requestStorage.subscribe(request => {
        const stored = store(request);
        app.ports.receiveStorage.send({
            id: stored.id
        });
    });

    app.ports.requestLookup.subscribe(request => {
        const stored = retrieve(request.id);
        app.ports.receiveLookup.send({
            iv: stored.iv,
            ciphertext: stored.ciphertext
        });
    });
}