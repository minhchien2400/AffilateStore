import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { STATUS } from "../utils/status";
import { fetchData, fetchDataBody } from "../utils/fetchData";

const productSlice = createSlice({
  name: "product",
  initialState: {
    data: {},
    status: STATUS.IDLE,
    productData: [],
    productDataStatus: STATUS.IDLE,
    topSale: {},
    topSaleStatus: STATUS.IDLE,
  },

  reducers: {
    setProducts(state, action) {
      state.data = action.payload;
    },
    setStatus(state, action) {
      state.status = action.payload;
    },
    // get product
    setProduct(state, action) {
      state.productData = action.payload;
    },
    setProductStatus(state, action) {
      state.productDataStatus = action.payload;
    },
    // top sale in home page
    setTopSale(state, action) {
      state.topSale = action.payload;
    },
    setTopSaleStatus(state, action) {
      state.topSaleStatus = action.payload;
    },
  },
});

export const {
  setProducts,
  setStatus,
  setProduct,
  setProductStatus,
  setTopSale,
  setTopSaleStatus,
} = productSlice.actions;
export default productSlice.reducer;

export const fetchProducts = (dataSend, method) => {
  return async function fetchProductThunk(dispatch) {
    dispatch(setStatus(STATUS.LOADING));

    try {
      const data = await fetchDataBody(
        `${BASE_URL}getproducts`,
        dataSend,
        method
      );
      if (data.hasError) {
        dispatch(setStatus(STATUS.ERROR));
      }
      dispatch(setProducts(data));
      dispatch(setStatus(STATUS.IDLE));
    } catch (error) {
      dispatch(setStatus(STATUS.ERROR));
    }
  };
};

export const fetchTopSale = (dataSend, method) => {
  return async function fetchProductThunk(dispatch) {
    dispatch(setTopSaleStatus(STATUS.LOADING));

    try {
      const data = await fetchDataBody(
        `${BASE_URL}getproducts`,
        dataSend,
        method
      );
      if (data.hasError) {
        dispatch(setTopSaleStatus(STATUS.ERROR));
      }
      dispatch(setTopSale(data));
      dispatch(setTopSaleStatus(STATUS.IDLE));
    } catch (error) {
      dispatch(setTopSaleStatus(STATUS.ERROR));
    }
  };
};

export const fetchProduct = (id) => {
  return async function fetchProductThunk(dispatch) {
    dispatch(setProductStatus(STATUS.LOADING));

    try {
      console.log("accc", id);
      const data = await fetchData(`${BASE_URL}getproductbyid/${id}`);
      console.log("data fetchProduct: ", data);
      if (data.hasError) {
        dispatch(setProduct(STATUS.ERROR));
      }
      dispatch(setProduct(data));
      dispatch(setProductStatus(STATUS.IDLE));
    } catch (error) {
      dispatch(setProductStatus(STATUS.ERROR));
    }
  };
};
