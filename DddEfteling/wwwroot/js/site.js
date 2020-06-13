﻿
var map = new ol.Map({
    target: 'map',
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        })
    ],
    view: new ol.View({
        center: ol.proj.fromLonLat([5.0499, 51.6499]),
        zoom: 16
    })
});

$(document).ready(function () {

    var rideIconStyle = new ol.style.Style({
        image: new ol.style.Icon({
            anchor: [0.5, 44],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            src: 'data/ride-icon.png'
        })
    });

    var fairyTaleIconStyle = new ol.style.Style({
        image: new ol.style.Icon({
            anchor: [0.5, 44],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            src: 'data/tale-icon.png'
        })
    });

    var standIconStyle = new ol.style.Style({
        image: new ol.style.Icon({
            anchor: [0.5, 44],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            src: 'data/stand-icon.png'
        })
    });

    iconSources = [{
        "url": "/api/rides",
        "iconStyle": rideIconStyle
    },
        {
            "url": "/api/fairy-tales",
            "iconStyle": fairyTaleIconStyle
        },
        {
            "url": "/api/stands",
            "iconStyle": standIconStyle
        }]

    $.each(iconSources, function (i, iconSource) {
        $.get(iconSource.url, function (data) {

            var features = [];

            $.each(data, function (k, ride) {
                var iconFeature = new ol.Feature({
                    geometry: new ol.geom.Point(
                            ol.proj.fromLonLat([ride.coordinates.longitude, ride.coordinates.latitude])
                    ),
                    name: ride.name
                });

                iconFeature.setStyle(iconSource.iconStyle);
                features.push(iconFeature);
            });

            var vectorSource = new ol.source.Vector({
                features: features
            });

            var vectorLayer = new ol.layer.Vector({
                source: vectorSource
            });

            map.addLayer(vectorLayer);
        });
    });

});