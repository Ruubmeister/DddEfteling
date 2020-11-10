import { SET_RIDES } from "../actionTypes";

const initialState = {
  allIds: [],
  byIds: {}
};

export default function(state = initialState, action) {
  switch (action.type) {
      case SET_RIDES:
        const { content } = action.payload;
        return {
          ...state,
          //allIds: [...state.allIds, content.map],
          byIds: {
            ...state.byIds,
            [id]: {
              content
            }
          }
        };
    default:
      return state;
  }
}
