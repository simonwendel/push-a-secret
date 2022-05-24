export { store, retrieve }

const storage = window.localStorage;

const getId = () => (Math.round(Date.now())).toString(36);

const store = encrypted => {
    const id = getId();
    storage.setItem(id, JSON.stringify(encrypted));
    return {
        id: id
    };
};

const retrieve = id => {
    const stored = JSON.parse(storage.getItem(id));
    return {
        algorithm: stored.algorithm,
        iv: stored.iv,
        ciphertext: stored.ciphertext
    };
};
