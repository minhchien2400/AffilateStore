import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { STATUS } from "../utils/status";
import { fetchDataBody } from "../utils/fetchData";

const searchSlice = createSlice({
    name: "search",
    initialState: {
        data: [],
        status: STATUS.IDLE,
    },

    reducers: {
        setDataSearch(state, action){
            state.data = action.payload;
        },
        setStatus(state, action){
            state.status = action.payload;
        },
    },
});

export const {setDataSearch, setStatus} = searchSlice.actions;
export default searchSlice.reducer;

export const fetchSearchProducts = (dataSend, method) => {
    return async function fetchProductThunk(dispatch){
        dispatch(setStatus(STATUS.LOADING));
        try{
            const data = await fetchDataBody(`${BASE_URL}getproducts`, dataSend, method);
            console.log(data);
            if(data.hasError)
            {
                dispatch(setStatus(STATUS.ERROR));
            }
            dispatch(setDataSearch(data.result));
            dispatch(setStatus(STATUS.IDLE));
        } catch(error){
            dispatch(setStatus(STATUS.ERROR));
        }
    }
}
