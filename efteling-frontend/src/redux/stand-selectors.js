
export const getStandsState = store => store.stands;

export const geStandsList = store =>
getStandsState(store) ? getStandsState(store).allIds : [];

export const getStandById = (store, id) =>
getStandsState(store) ? { ...getStandsState(store).byIds[id], id } : {};

