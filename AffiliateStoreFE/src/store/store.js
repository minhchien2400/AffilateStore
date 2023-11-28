import { configureStore } from "@reduxjs/toolkit";
import productReducer from "./productSlice";
import categoryReducer from "./categorySlice";
import modalReducer from "./modalSlice";
import cartReducer from "./cartSlice";
import searchReducer from "./searchSlice";
import filterReducer from "./filterSlice";

const store = configureStore({
    reducer: {
        product: productReducer,
        category: categoryReducer,
        modal: modalReducer,
        cart: cartReducer,
        search: searchReducer,
        filter: filterReducer,
    }
});

export default store;