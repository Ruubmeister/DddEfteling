import React from 'react';
import { connect } from "react-redux";
import './App.css';
import "./LiveMap.css";
import {getRides} from './redux/ride-selectors';
import {getFairyTales} from './redux/fairy-tale-selectors';
import {getStands} from './redux/stand-selectors';
import {getVisitors} from './redux/visitor-selectors';

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

    visitorsLayer = null;
    ridesLayer = null;
    fairyTalesLayer = null;
    standsLayer = null;

  constructor(props) {
    super(props);
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

  getLocationLayer(data, iconStyle){
    var features = [];

    data.forEach(item => {
        var iconFeature = new Feature({
            geometry: new Point(
                    fromLonLat([item.coordinates.longitude, item.coordinates.latitude])
            ),
            name: item.name
        });

        iconFeature.setStyle(iconStyle);
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

  getVisitorLayer(){

    var features = []

    this.props.visitors.forEach(visitor => {

      var iconFeature = new Feature({
          geometry: new Point(
              fromLonLat([visitor.currentLocation.longitude, visitor.currentLocation.latitude])
          ),
          name: visitor.Guid
      });

      features.push(iconFeature);
    });

    var vectorSource = new VectorSource({
        features: features
    });

    var visitorVectorLayer = new VectorLayer({
        source: vectorSource
    });

    return visitorVectorLayer;
  }

  getRideLayer(){
    return this.getLocationLayer(this.props.rides, rideIconStyle);
  }

  getFairyTaleLayer(){
    return this.getLocationLayer(this.props.fairyTales, fairyTaleIconStyle);
  }

  getStandLayer(){
    return this.getLocationLayer(this.props.stands, standIconStyle);
  }

  removeLayers(){
    if(this.visitorsLayer != null){
      this.eftelingMap.removeLayer(this.visitorsLayer);
    }
    if(this.ridesLayer != null){
      this.eftelingMap.removeLayer(this.ridesLayer);
    }
    if(this.fairyTalesLayer != null){
      this.eftelingMap.removeLayer(this.fairyTalesLayer);
    }
    if(this.standsLayer != null){
      this.eftelingMap.removeLayer(this.standsLayer);
    }
  }

  addLayers(){

    this.eftelingMap.addLayer(this.ridesLayer);
    this.eftelingMap.addLayer(this.fairyTalesLayer);
    this.eftelingMap.addLayer(this.standsLayer);
    this.eftelingMap.addLayer(this.visitorsLayer);
  }
  
  
    render() {
        if(this.eftelingMap != null){
          this.removeLayers();
          this.visitorsLayer = this.getVisitorLayer();
          this.ridesLayer = this.getRideLayer();
          this.fairyTalesLayer = this.getFairyTaleLayer();
          this.standsLayer = this.getStandLayer();
          this.addLayers();
        }

      return <div ref={this.map} className="ol-map"> </div>;
    }
  }

  const mapStateToProps = state => {
    const rides = getRides(state);
    const fairyTales = getFairyTales(state);
    const stands = getStands(state);
    const visitors = getVisitors(state);
    return { rides: rides, fairyTales: fairyTales, stands: stands, visitors: visitors };
  };

  export default connect(mapStateToProps)(LiveMap);