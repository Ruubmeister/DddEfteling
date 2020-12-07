import React from "react";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import { faMusic } from '@fortawesome/free-solid-svg-icons';
import Button from "react-bootstrap/Button";


class Music extends React.Component {
    state = {
      play: false
    }
    audio = new Audio(this.props.url)
  
    componentDidMount() {
      this.audio.addEventListener('ended', () => this.setState({ play: false }));
    }
  
    componentWillUnmount() {
      this.audio.removeEventListener('ended', () => this.setState({ play: false }));  
    }
  
    togglePlay = () => {
      this.setState({ play: !this.state.play }, () => {
        this.state.play ? this.audio.play() : this.audio.pause();
      });
    }
  
    render() {
      return (
        <div className="music-player">
          <Button variant="outline-primary" size="sm" onClick={this.togglePlay}><FontAwesomeIcon icon={faMusic} /> {this.state.play ? 'Pauzeren' : 'Afspelen'}</Button>
        </div>
      );
    }
  }
  
  export default Music;