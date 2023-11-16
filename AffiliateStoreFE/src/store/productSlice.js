import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { STATUS } from "../utils/status";

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

export const fetchProducts = () => {
    return async function fetchProductThunk(dispatch){
        dispatch(setStatus(STATUS.LOADING));
        try{
            const response = await fetch(`${BASE_URL}getallproducts`);
            const data = await response.json();
            dispatch(setProducts(data));
            dispatch(setStatus(STATUS.IDLE));
        } catch(error){
            dispatch(setStatus(STATUS.ERROR));
        }
    }
}

export const fetchSearchProducts = async (searchData) => {
    //dispatch(setStatus(STATUS.LOADING));
    let searchUrl = `${BASE_URL}searchproducts`;
    await fetch(searchUrl, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        // Thêm body cho yêu cầu
        body: JSON.stringify(searchData),
    })
        .then(response => response.json())
        .then(data => {
            // Xử lý dữ liệu trả về từ server
        //    dispatch(searchProducts(data));
      //  dispatch(setStatus(STATUS.IDLE));
        })
        .catch(error => {
            // Xử lý lỗi nếu có
         //   dispatch(setStatus(STATUS.ERROR))
        });
    // try{
    //     const response = await fetch();
    //     const data = await response.json();
    //     dispatch(searchProducts(data));
    //     dispatch(setStatus(STATUS.IDLE));
    // }
    // catch(error){
    //     dispatch(setStatus(STATUS.ERROR))
    // }
}
