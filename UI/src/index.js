import Vue from "vue";
import {
    createNanoEvents
} from "nanoevents";
import UIkit from 'uikit';
import Icons from 'uikit/dist/js/uikit-icons';
import App from "./App.vue";

import "./index.scss";

const emitter = createNanoEvents();
window.emitter = emitter;

UIkit.use(Icons);

new Vue({
    render: h => h(App)
}).$mount("#app");

window.emitter.on('tick', () => {
    console.log('test');
});