import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { fetchDataBody } from "../utils/fetchData";

const storeInLocalStorage = (data) => {
  localStorage.setItem("cart", JSON.stringify(data));
};

const cartSlice = createSlice({
  name: "cart",
  initialState: {
    ProductsAdded: [],
    TotalAdded: 0,
    ProductsPurchased: [],
    TotalPurchased: 0,
  },
  reducers: {
    setProductsAdded(state, action) {
      state.ProductsAdded = action.payload;
      const cartData = localStorage.getItem('cart');
      var dataObject = JSON.parse(cartData);
      dataObject.ProductsAdded = action.payload;
      state.TotalAdded = dataObject.ProductsAdded.length;
      storeInLocalStorage(dataObject);
    },
    
    setProductsPurchased(state, action) {
      state.ProductsPurchased = action.payload;
      const cartData = localStorage.getItem('cart');
      var dataObject = JSON.parse(cartData);
      dataObject.ProductsPurchased = action.payload;
      state.TotalPurchased = dataObject.ProductsPurchased.length;
      storeInLocalStorage(dataObject);
    },


    setAddToCart(state, action) {
      state.ProductsAdded.push(action.payload);
    },
    setRemoveFromCart(state, action) {
      const tempCart = state.ProductsAdded.filter((item) => item.id !== action.payload);
      state.ProductsAdded = tempCart;
      const cartData = localStorage.getItem('cart');
      var dataObject = JSON.parse(cartData);
      dataObject.ProductsAdded = tempCart;
      state.TotalAdded = dataObject.ProductsAdded.length;
      storeInLocalStorage(dataObject);
    },
    setClearCart(state) {
      state.ProductsAdded = [];
      localStorage.removeItem('cart');
    },
  },
});

export const { setProductsAdded, setProductsPurchased, setAddToCart, setRemoveFromCart, setClearCart } =
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
