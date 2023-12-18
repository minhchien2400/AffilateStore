import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { fetchDataBody } from "../utils/fetchData";

const storeInLocalStorage = (data) => {
  localStorage.setItem("cart", JSON.stringify(data));
};

const cartSlice = createSlice({
  name: "cart",
  initialState: {
    productsAdded: {},
    totalAdded: 0,
    productsPurchased: {},
    totalPurchased: 0,
  },
  reducers: {
    setAddToCart(state, action) {
      state.data.push(action.payload);
      const response = fetchDataBody(
        `${BASE_URL}addtocart`,
        {
          ProductId: action.payload.ProductId,
          AccessToken: localStorage.getItem("jwtToken"),
        },
        "POST"
      );
    },
    setRemoveFromCart(state, action) {
      const tempCart = state.data.filter((item) => item.id !== action.payload);
      state.data = tempCart;
      storeInLocalStorage(state.data);
    },
    setClearCart(state) {
      state.data = [];
    },
  },
});

export const { setAddToCart, setRemoveFromCart, setClearCart } =
  cartSlice.actions;
export default cartSlice.reducer;

export const fetchCartAction = (product, dataSend, method) => {
  return async function fetchCartActionThunk(dispatch) {
    dispatch(setAddToCart(product));
    try {
      const data = await fetchDataBody(
        `${BASE_URL}addtocart`,
        dataSend,
        method
      );
      if (data.hasError) {
      }
    } catch (error) {}
  };
};
