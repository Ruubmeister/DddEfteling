import React from 'react';
import { connect } from "react-redux";
import './App.css';
import "./LiveMap.css";
import {getRidesList} from './redux/ride-selectors';

import Map from 'ol/Map';
import Style from 'ol/style/Style';
import Icon from 'ol/style/Icon';
import Tile from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import View from 'ol/View';
import {fromLonLat} from 'ol/proj';

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
  constructor(props) {
    super(props);
    this.rides = props.rides;
    this.map = React.createRef();
  }

  componentDidMount() {
    var map = new Map({
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
  
  
    render() {
      return <div ref={this.map} className="ol-map"> </div>;
    }
  }

  const mapStateToProps = state => {
    const rides = getRidesList(state);
    return { rides };
  };

  export default connect(mapStateToProps)(LiveMap);