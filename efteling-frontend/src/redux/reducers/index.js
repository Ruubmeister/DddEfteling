import { combineReducers } from "redux";
import visitors from "./visitors";
import rides from "./rides";

export default combineReducers({ rides, visitors });
