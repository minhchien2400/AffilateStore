import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { fetchDataBody } from "../utils/fetchData";
import {
  SET_ADD_PRODUCTS,
  SET_PURCHASED_PRODUCTS,
  ADD_TO_CART,
  SET_TOTAL_ADDED,
  SET_MARK_PURCHASED,
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
      console.log("reducer: setCartProducts", action);
      switch (action.payload.type) {
        case SET_ADD_PRODUCTS:
          state.ProductsAdded = action.payload.value.products;
          state.TotalAdded = action.payload.value.totalProducts;
          return;
        case SET_PURCHASED_PRODUCTS:
          state.ProductsPurchased = action.payload.value.products;
          state.TotalPurchased = action.payload.value.totalProducts;
          return;
        case ADD_TO_CART:
          state.ProductsAdded.push(action.payload.value);
          state.TotalAdded = state.TotalAdded + 1;
          return;
        case SET_TOTAL_ADDED:
          state.TotalAdded = action.payload.value;
          return;
        case SET_MARK_PURCHASED:
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
        `${BASE_URL}getcartproducts`,
        dataSend,
        method
      );
      if (data.hasError) {
      }
      console.log("fetchCartProducts", data);
      dispatch(setCartProducts({ type: SET_ADD_PRODUCTS, value: data }));
    } catch (error) {}
  };
};

export const fetchTotalAdded = () => {
  return async function fetchTotalAddedThunk(dispatch) {
    try {
      const data = await fetchDataBody(
        `${BASE_URL}gettotalcart`,
        localStorage.getItem("jwtToken"),
        "POST"
      );
      console.log("fetchTotalAdded", data);
      dispatch(setCartProducts({ type: SET_TOTAL_ADDED, value: data }));
    } catch (error) {}
  };
};

export const fetchCartAction = (dataSend, method) => {
  return async function fetchCartActionThunk(dispatch) {
    try {
      const data = await fetchDataBody(
        `${BASE_URL}cartaction`,
        dataSend,
        method
      );
      if (!data.isSuccess) {
        return data.message;
      } else {
        switch (dataSend.ActionType) {
          case ActionTypeCart.Add:
            dispatch(
              setCartProducts({ type: ADD_TO_CART, value: data.result })
            );
          case ActionTypeCart.Purchase:
            dispatch(
              setCartProducts({ type: SET_MARK_PURCHASED, value: data.result })
            );
          case ActionTypeCart.Remove:
            dispatch(
              setCartProducts({ type: SET_REMOVE_ADDED, value: data.result })
            );
        }
      }
    } catch (error) {}
  };
};
