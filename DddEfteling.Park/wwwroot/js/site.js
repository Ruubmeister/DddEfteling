
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


visitorVectorLayer = null;

var rideStatsBar = $(".ride-statistics")
var parkStatsBar = $(".park-statistics")

$(document).ready(function () {

    $(".map-col").height($("body").height() - $("footer").height());
    map.updateSize();

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
        "url": "http://localhost:3997/api/v1/rides",
        "iconStyle": rideIconStyle
    },
        {
            "url": "http://localhost:3999/api/v1/fairy-tales",
            "iconStyle": fairyTaleIconStyle
        },
        {
            "url": "http://localhost:3996/api/v1/stands",
            "iconStyle": standIconStyle
        }]

    setInterval(function () {
        $.get("http://localhost:3995/api/v1/visitors", function (data) {
            var features = [];

            $.each(data, function (k, visitor) {
                var iconFeature = new ol.Feature({
                    geometry: new ol.geom.Point(
                        ol.proj.fromLonLat([visitor.currentLocation.longitude, visitor.currentLocation.latitude])
                    ),
                    name: visitor.Guid
                });

                features.push(iconFeature);
            });

            var vectorSource = new ol.source.Vector({
                features: features
            });
            console.log(visitorVectorLayer);
            if (visitorVectorLayer != null) {
                map.removeLayer(visitorVectorLayer);
            }

            visitorVectorLayer = new ol.layer.Vector({
                source: vectorSource
            });

            map.addLayer(visitorVectorLayer);
        });
    }, 400);
            
    
   

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

            map.on('click', function (evt) {
                var feature = map.forEachFeatureAtPixel(evt.pixel,
                    function (feature) {
                        return feature;
                    });
                if (feature) {

                    rideStatsBar.addClass("visible").removeClass("invisible");
                    parkStatsBar.addClass("invisible").removeClass("visible");

                    var coordinates = feature.getGeometry().getCoordinates();
                    popup.setPosition(coordinates);
                    $(element).popover({
                        placement: 'top',
                        html: true,
                        content: feature.get('name')
                    });
                    $(element).popover('show');
                } else {
                    rideStatsBar.addClass("invisible").removeClass("visible");
                    parkStatsBar.addClass("visible").removeClass("invisible");
                }
            });
        });
    });

});