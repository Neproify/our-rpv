import Vue from "vue";
import UIkit from 'uikit';
import Icons from 'uikit/dist/js/uikit-icons';
import App from "./App.vue";

import "./index.scss";

UIkit.use(Icons);

new Vue({
    render: h => h(App)
}).$mount("#app");