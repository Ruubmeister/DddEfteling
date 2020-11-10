
export const getRidesState = store => store.rides;

export const geRidesList = store =>
getRidesState(store) ? getRidesState(store).allIds : [];

export const getRideById = (store, id) =>
getRidesState(store) ? { ...getRidesState(store).byIds[id], id } : {};

/**
 * example of a slightly more complex selector
 * select from store combining information from multiple reducers
 */
export const getRides = store =>
geRidesList(store).map(id => getRideById(store, id));
