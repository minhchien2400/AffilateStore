import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { STATUS } from "../utils/status";
import { fetchDataBody } from "../utils/fetchData";

const productSlice = createSlice({
    name: "product",
    initialState: {
        data: [],
        status: STATUS.IDLE,
    },

    reducers: {
        setProducts(state, action){
            state.data = action.payload;
        },
        setStatus(state, action){
            state.status = action.payload;
        },
        searchProducts(state, action){
            state.status = action.payload;
        }
    },
});

export const {setProducts, setStatus, searchProducts} = productSlice.actions;
export default productSlice.reducer;

export const fetchProducts = (dataSend, method) => {
    return async function fetchProductThunk(dispatch){
        dispatch(setStatus(STATUS.LOADING));
        // try{
        //     const response = await fetch(`${BASE_URL}getallproducts`);
        //     const data = await response.json();
        //     dispatch(setProducts(data));
        //     dispatch(setStatus(STATUS.IDLE));
        // } catch(error){
        //     dispatch(setStatus(STATUS.ERROR));
        // }

        try{
            const data = await fetchDataBody(`${BASE_URL}getallproducts`, dataSend, method);
            console.log(data);
            if(data.hasError)
            {
                dispatch(setStatus(STATUS.ERROR));
            }
            dispatch(setProducts(data.result));
            dispatch(setStatus(STATUS.IDLE));
        } catch(error){
            dispatch(setStatus(STATUS.ERROR));
        }
    }
}


