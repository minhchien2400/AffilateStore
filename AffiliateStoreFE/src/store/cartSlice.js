import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { fetchDataBody } from "../utils/fetchData";
import {
  SET_CART_PRODUCTS,
  SET_PRODUCTS_ADDED,
  SET_TOTAL_ADDED,
  SET_PRODUCTS_PURCHASED,
  SET_TOTAL_PURCHASED,
  SET_REMOVE_ADDED,
  ActionTypeCart,
} from "../utils/const";

const cartSlice = createSlice({
  name: "cart",
  initialState: {
    ProductsAdded: [],
    TotalAdded: 0,
    ProductsPurchased: [],
    TotalPurchased: 0,
  },
  reducers: {
    setCartProducts(state, action) {
      console.log("reducer: setCartProducts",action);
      switch (action.payload.type) {
        case SET_CART_PRODUCTS:
          console.log("case SET_CART_PRODUCTS",action);
          state.ProductsAdded = action.payload.value.productsAdded;
          state.TotalAdded = action.payload.value.totalAdded;
          state.ProductsPurchased = action.payload.value.productsPurchased;
          state.TotalPurchased = action.payload.value.totalPurchased;
          return;
        case SET_PRODUCTS_ADDED:
          state = {
            ...state,
            ProductsAdded: state.ProductsAdded.push(action.payload.value),
            TotalAdded: state.TotalAdded +1
          }
          //state.ProductsAdded.push(action.payload.value);
          //state.TotalAdded = state.TotalAdded + 1;
          console.log("state.ProductsAdded.length", state.TotalAdded + 1);
          return;
        case SET_TOTAL_ADDED:
          state.TotalAdded = action.payload.value;
          return
        case SET_PRODUCTS_PURCHASED:
          state.ProductsPurchased.push(action.payload.value);
          state.ProductsAdded = state.ProductsAdded.filter(
            (item) => item.productId !== action.payload.value.productId
          );
          state.TotalAdded = state.ProductsAdded.length;
          state.TotalPurchased = state.ProductsPurchased.length;
          return;
        case SET_TOTAL_PURCHASED:
          state.TotalPurchased = action.payload.value;
          return;
        case SET_REMOVE_ADDED:
          state.ProductsAdded = state.ProductsAdded.filter(
            (item) => item.productId !== action.payload.value.productId
          );
          state.TotalAdded = state.ProductsAdded.length;
        default:
          return;
      }
    },
  },
});

export const { setCartProducts } = cartSlice.actions;
export default cartSlice.reducer;

export const fetchCartProducts = (dataSend, method) => {
  return async function fetchCartActionThunk(dispatch) {
    try {
      const data = await fetchDataBody(
        `${BASE_URL}getcartproducs`,
        dataSend,
        method
      );
      if (data.hasError) {
      }
      console.log("fetchCartProducts", data);
      dispatch(setCartProducts({ type: SET_CART_PRODUCTS, value: data }));
    } catch (error) {}
  };
};

export const fetchTotalAdded = () => {
  return async function fetchTotalAddedThunk(dispatch) {
    try {
      const data = await fetchDataBody(`${BASE_URL}gettotalcart`, localStorage.getItem('jwtToken'), "POST");
      console.log("fetchTotalAdded", data);
      dispatch(setCartProducts({ type: SET_TOTAL_ADDED, value: data }));
    } catch (error) {}
  };
};

export const fetchCartAction = (product, dataSend, method) => {
  return async function fetchCartActionThunk(dispatch) {
    try {
      const data = await fetchDataBody(
        `${BASE_URL}cartaction`,
        dataSend,
        method
      );
      if (data.hasError) {
      }
      switch (dataSend.ActionType) {
        case ActionTypeCart.Add:
          dispatch(
            setCartProducts({ type: SET_PRODUCTS_ADDED, value: product })
          );
        case ActionTypeCart.Purchase:
          dispatch(
            setCartProducts({ type: SET_PRODUCTS_PURCHASED, value: product })
          );
        case ActionTypeCart.Remove:
          dispatch(
            setCartProducts({ type: SET_REMOVE_ADDED, value: product })
          );
      }
    } catch (error) {}
  };
};
