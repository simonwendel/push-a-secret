export { store, retrieve, remove, check }

const storage = window.localStorage;

const getId = () => (Math.round(Date.now())).toString(36);

const check = id => storage.getItem(id) !== null;

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

const remove = id => {
    storage.removeItem(id);
};
