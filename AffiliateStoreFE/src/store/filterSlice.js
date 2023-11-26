import { createSlice } from "@reduxjs/toolkit";

const filterSlice = createSlice({
    name: "filter",
    initialState: {
        data: [],
    },
    reducers: {
        setFilter(state, action){
            state.data = action.payload;
        },
    }
});

export const  setFilter  = filterSlice.actions;
export default filterSlice.reducer;

export const setOrderFilter = (filter) => {
    return async function setFilterThunk(dispatch){
        dispatch(setFilter(filter));
    }
}