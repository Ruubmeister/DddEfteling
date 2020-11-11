
export const getRidesState = store => store.rides;

export const getRidesList = store =>
getRidesState(store) ? getRidesState(store).allIds : [];

export const getRideById = (store, id) =>
getRidesState(store) ? { ...getRidesState(store).byIds[id], id } : {};
