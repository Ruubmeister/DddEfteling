import React from 'react';
import { connect } from "react-redux";
import './App.css';
import "./LiveMap.css";
import {getRides} from './redux/ride-selectors';

import Map from 'ol/Map';
import Style from 'ol/style/Style';
import Icon from 'ol/style/Icon';
import Tile from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import View from 'ol/View';
import {fromLonLat} from 'ol/proj';
import Feature from 'ol/Feature';
import Point from 'ol/geom/Point';
import VectorSource from 'ol/source/Vector';
import VectorLayer from 'ol/layer/Vector';

var rideIconStyle = new Style({
  image: new Icon({
      anchor: [0.5, 44],
      anchorXUnits: 'fraction',
      anchorYUnits: 'pixels',
      src: 'data/ride-icon.png'
  })
});

var fairyTaleIconStyle = new Style({
  image: new Icon({
      anchor: [0.5, 44],
      anchorXUnits: 'fraction',
      anchorYUnits: 'pixels',
      src: 'data/tale-icon.png'
  })
});

var standIconStyle = new Style({
  image: new Icon({
      anchor: [0.5, 44],
      anchorXUnits: 'fraction',
      anchorYUnits: 'pixels',
      src: 'data/stand-icon.png'
  })
});

class LiveMap extends React.Component {

    eftelingMap = null;

  constructor(props) {
    super(props);
    this.rides = props.rides;
    this.map = React.createRef();
  }

  componentDidMount() {
    this.eftelingMap = new Map({
      target: this.map.current,
      layers: [
          new Tile({
              source: new OSM()
          })
      ],
      view: new View({
          center: fromLonLat([5.0499, 51.6499]),
          zoom: 16
      })
    });
  }

  getRideLayer(){
    var features = [];
    console.log(this.rides);

    this.rides.forEach(ride => {
        var iconFeature = new Feature({
            geometry: new Point(
                    fromLonLat([ride.coordinates.longitude, ride.coordinates.latitude])
            ),
            name: ride.name
        });

        iconFeature.setStyle(rideIconStyle);
        features.push(iconFeature);
    });

    var vectorSource = new VectorSource({
        features: features
    });

    var vectorLayer = new VectorLayer({
        source: vectorSource
    });

    return vectorLayer;

  }
  
  
    render() {
        if(this.eftelingMap != null){
            this.eftelingMap.addLayer(this.getRideLayer());

        }

      return <div ref={this.map} className="ol-map"> </div>;
    }
  }

  const mapStateToProps = state => {
    const rides = getRides(state);
    return { rides };
  };

  export default connect(mapStateToProps)(LiveMap);