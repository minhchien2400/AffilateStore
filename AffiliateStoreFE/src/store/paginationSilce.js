import { createSlice } from "@reduxjs/toolkit";

const paginationSlice = createSlice({
    name: "pagination",
    initialState: {
        data: {},
    },
    reducers: {
        setPagination(state, action){
            state.data = action.payload;
        },
    }
});

export const  {setPagination}  = paginationSlice.actions;
export default paginationSlice.reducer;

export const setPaginations = (values) => {
    return async function setPaginationsThunk(dispatch){
        dispatch(setPagination(values));
    }
}