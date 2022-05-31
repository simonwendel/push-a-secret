export { wireCrypto, wireStorage }

import { check, retrieve, store, remove } from './storage.js';
import { generate, encrypt, decrypt } from '/crypto.js'

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

    app.ports.requestDecryption.subscribe(async request => {
        const decrypted = await decrypt(request);
        app.ports.receiveDecryption.send({
            cleartext: decrypted.cleartext
        });
    });
}

const wireStorage = app => {
    app.ports.requestPeek.subscribe(request => {
        const exists = check(request.id);
        app.ports.receivePeek.send({
            exists: exists
        });
    });

    app.ports.requestCreate.subscribe(request => {
        const stored = store(request);
        app.ports.receiveCreate.send({
            id: stored.id
        });
    });

    app.ports.requestRead.subscribe(request => {
        const stored = retrieve(request.id);
        app.ports.receiveRead.send({
            algorithm: stored.algorithm,
            iv: stored.iv,
            ciphertext: stored.ciphertext
        });
    });
    
    app.ports.requestDelete.subscribe(request => {
        remove(request.id);
        app.ports.receiveDelete.send({
            success: true
        });
    });
}
