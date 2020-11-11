import './App.css';
import LiveMap from "./LiveMap"
import React from 'react';
import axios from 'axios';
import { connect } from "react-redux";
import { setFairyTales } from "./redux/fairy-tale-actions";
import { setRides } from "./redux/ride-actions";
import { setStands } from "./redux/stand-actions";
import { setVisitors } from "./redux/visitor-actions";

class App extends React.Component{

  constructor(props) {
    super(props);
    this.getFairyTales();
    this.getRides();
    this.getStands();
    this.getVisitors();
  }

  getFairyTales(){
    axios.get('http://localhost:3999/api/v1/fairy-tales').then(response => 
    this.props.setFairyTales(response.data));
  }

  getRides(){
    axios.get('http://localhost:3997/api/v1/rides').then(response => 
    this.props.setRides(response.data));
  }

  getStands(){
    axios.get('http://localhost:3996/api/v1/stands').then(response => 
    this.props.setStands(response.data));
  }

  getVisitors(){
    axios.get('http://localhost:3995/api/v1/visitors').then(response => 
    this.props.setVisitors(response.data));
  }

  render() { 
    return <LiveMap/>;
  };
}

export default connect(null, {setRides, setFairyTales, setStands, setVisitors},)(App);
