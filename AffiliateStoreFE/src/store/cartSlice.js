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
  CartStatus,
} from "../utils/const";

const cartSlice = createSlice({
  name: "cart",
  initialState: {
    ProductsAdded: {},
    ProductsPurchased: {},
  },
  reducers: {
    setCartProducts(state, action) {
      switch (action.payload.type) {
        case SET_ADD_PRODUCTS:
          state.ProductsAdded = action.payload.value;
          state.ProductsAdded = action.payload.value;
          console.log("SET_ADD_PRODUCTS", action.payload.value);
          return;
        case SET_PURCHASED_PRODUCTS:
          state.ProductsPurchased = action.payload.value;
          console.log("SET_PURCHASED_PRODUCTS", action.payload.value);
          return;
        case ADD_TO_CART:
          state.ProductsAdded.products.push(action.payload.value);
          state.ProductsAdded.totalProducts =
            state.ProductsAdded.totalProducts + 1;
          return;
        case SET_TOTAL_ADDED:
          state.ProductsAdded.totalProducts = action.payload.value;
          return;
        case SET_MARK_PURCHASED:
          state.ProductsPurchased.products.push(action.payload.value);
          state.ProductsAdded.products = state.ProductsAdded.products.filter(
            (item) => item.productId !== action.payload.value.productId
          );
          state.ProductsAdded.totalProducts =
            state.ProductsAdded.totalProducts - 1;
          state.TotalPurchased.totalProducts =
            state.ProductsPurchased.totalProducts + 1;
          return;
        case SET_REMOVE_ADDED:
          state.ProductsAdded.products = state.ProductsAdded.products.filter(
            (item) => item.productId !== action.payload.value.productId
          );
          state.ProductsAdded.totalProducts =
            state.ProductsAdded.totalProducts - 1;
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
    const isAdded = dataSend.CartStatus === CartStatus.Added;
    try {
      const data = await fetchDataBody(
        `${BASE_URL}getcartproducts`,
        dataSend,
        method
      );
      if (data.hasError) {
      }
      console.log("fetchCartProducts", data);
      dispatch(
        setCartProducts({
          type: isAdded ? SET_ADD_PRODUCTS : SET_PURCHASED_PRODUCTS,
          value: data,
        })
      );
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
