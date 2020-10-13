import Vue from 'vue'
import LiveMap from './components/LiveMap.vue'

Vue.config.productionTip = false

Vue.component("LiveMap", LiveMap)

new Vue({
    el: '#app',
    template: `<LiveMap />`,
    components: {
        LiveMap
    }
})