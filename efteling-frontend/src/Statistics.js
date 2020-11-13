import React from 'react';
import { connect } from "react-redux";
import './App.css';
import {getRides} from './redux/ride-selectors';
import {getFairyTales} from './redux/fairy-tale-selectors';
import {getStands} from './redux/stand-selectors';
import {getVisitors} from './redux/visitor-selectors';

import Container from "react-bootstrap/Container";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col"
import Card from "react-bootstrap/Card";

class Statistics extends React.Component {


  constructor(props) {
    super(props);
  }

  componentDidMount() {

  }
  
    render() {
        const rideStatistics = this.props.rides.map(ride => 
            <Col xs={6} md={4} lg={4} key={ride.guid}>

                <Card style={{ "margin": "20px"}}>
                    <Card.Body>
                        <Card.Title>{ride.name}</Card.Title>
                        <Card.Text>
                        Visitors in line: 
                        </Card.Text>
                    </Card.Body>
                </Card>
            </Col>
            )

      return <Container fluid>
          <Row>
              {rideStatistics}
          </Row>
      </Container>;
    }
  }

  const mapStateToProps = state => {
    const rides = getRides(state);
    const fairyTales = getFairyTales(state);
    const stands = getStands(state);
    const visitors = getVisitors(state);
    return { rides: rides, fairyTales: fairyTales, stands: stands, visitors: visitors };
  };

  export default connect(mapStateToProps)(Statistics);