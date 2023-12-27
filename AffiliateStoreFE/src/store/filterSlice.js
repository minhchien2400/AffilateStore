import { createSlice } from "@reduxjs/toolkit";
import {
  SET_PRODUCTS_FILTER,
  SET_CATEGORY_FILTER,
  CART_ADDED_FILTER,
  CART_PURCHASED_FILTER,
} from "../utils/const";


const filterSlice = createSlice({
  name: "filter",
  initialState: {
    ProductsFilter: {
      Offset: 1,
      Limit: 10,
      SearchText: "",
      Keys: ["all", "all"],
    },
    CategoryFilter: {
      Offset: 1,
      Limit: 5,
      SearchText: "",
      Keys: ["all"],
    },
    CartAddedFilter: {
      Offset: 1,
      Limit: 2,
      SearchText: "",
      Keys: ["time-up"],
    },
    CartPurchasedFilter: {
      Offset: 1,
      Limit: 10,
      SearchText: "",
      Keys: ["time-up"],
    },
  },
  reducers: {
    setFilters(state, action) {
      switch (action.payload.type) {
        case SET_PRODUCTS_FILTER:
          state.ProductsFilter = action.payload.value;
          return;
        case SET_CATEGORY_FILTER:
          state.CategoryFilter = action.payload.value;
          return;
        case CART_ADDED_FILTER:
          state.CartAddedFilter = action.payload.value;
          return;
        case CART_PURCHASED_FILTER:
          state.CartPurchasedFilter = action.payload.value;
          return;
      }
    },
  },
});

export const { setFilters } = filterSlice.actions;
export default filterSlice.reducer;

export const setFilterAction = (type, filter) => {
  return async function setFilterThunk(dispatch) {
    dispatch(setFilters({type: type, value: filter}));
  };
};
