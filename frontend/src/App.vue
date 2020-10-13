<template>
    <div id="app">
        <div class="row">
            <div class="col" style="height:700px;">
                <LiveMap/>
            </div>
        </div>
    </div>
</template>

<script>
    import LiveMap from './components/LiveMap.vue'
    export default {
        components: {
            LiveMap
        }
    };

    
</script>

<style>
    #map {
        height: 99%;
        width:99%;
        min-height:100%;
        display:block;
    }
    #app{
        height:100%;
        width:100%;
    }
</style>
