import { Elm } from './src/Main.elm';
import { setup } from './js/ports.js'

const app = Elm.Main.init({
    flags: {
        base_url: window.location.protocol + '//' + window.location.host
    }
});

setup(app);