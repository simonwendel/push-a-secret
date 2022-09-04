// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

import { generate, encrypt, decrypt } from './crypto.js'

export const setup = app => {
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
};
