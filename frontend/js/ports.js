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
    app.ports.requestCheck.subscribe(request => {
        const exists = check(request.id);
        app.ports.receiveCheck.send({
            exists: exists
        });
    });

    app.ports.requestStorage.subscribe(request => {
        const stored = store(request);
        app.ports.receiveStorage.send({
            id: stored.id
        });
    });

    app.ports.requestLookup.subscribe(request => {
        const stored = retrieve(request.id);
        app.ports.receiveLookup.send({
            algorithm: stored.algorithm,
            iv: stored.iv,
            ciphertext: stored.ciphertext
        });
    });
    
    app.ports.requestDeletion.subscribe(request => {
        remove(request.id);
        app.ports.receiveDeletion.send({
            success: true
        });
    });
}
