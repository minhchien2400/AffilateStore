import { createSlice } from "@reduxjs/toolkit";

const filterSlice = createSlice({
    name: "filter",
    initialState: {
        data: {},
    },
    reducers: {
        setFilters(state, action){
            state.data = action.payload;
            console.log(action.payload);
        },
    }
});

export const  {setFilters}  = filterSlice.actions;
export default filterSlice.reducer;

export const setOrderFilter = (filter) => {
    return async function setFilterThunk(dispatch){
        dispatch(setFilters(filter));
    }
}
