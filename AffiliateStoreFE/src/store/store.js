import { configureStore } from "@reduxjs/toolkit";
import productReducer from "./productSlice";
import categoryReducer from "./categorySlice";
import modalReducer from "./modalSlice";
import cartReducer from "./cartSlice";
import filterReducer from "./filterSlice";
import pageReducer from "./pageSlice";
import loginReducer from "./loginSlice"
const store = configureStore({
    reducer: {
        product: productReducer,
        category: categoryReducer,
        modal: modalReducer,
        cart: cartReducer,
        filter: filterReducer,
        page : pageReducer,
        login: loginReducer
    }
});

export default store;