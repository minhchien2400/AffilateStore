import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { STATUS } from "../utils/status";
import { fetchDataBody } from "../utils/fetchData";

const categorySlice = createSlice({
  name: "category",
  initialState: {
    data: {},
    status: STATUS.IDLE,
    productsCategory: {},
    productsCategoryStatus: STATUS.IDLE,
  },

  reducers: {
    setCategories(state, action) {
      state.data = action.payload;
    },
    setStatus(state, action) {
      state.status = action.payload;
    },
    setCategoryProducts(state, action) {
      state.productsCategory = action.payload;
    },
    setCategoryProductsStatus(state, action) {
      state.productsCategoryStatus = action.payload;
    }
  },
});

export const {
  setCategories,
  setStatus,
  setCategoryProducts,
  setCategoryProductsStatus
} = categorySlice.actions;
export default categorySlice.reducer;

export const fetchCategories = (dataSend, method) => {
  return async function fetchCategoryThunk(dispatch) {
    dispatch(setStatus(STATUS.LOADING));
    try {
      const data = await fetchDataBody(
        `${BASE_URL}getcategory`,
        dataSend,
        method
      );
      dispatch(setCategories(data));
      dispatch(setStatus(STATUS.IDLE));
    } catch (error) {
      dispatch(setStatus(STATUS.ERROR));
    }
  };
};

export const fetchProductsByCategoryId = (dataSend, method) => {
  return async function fetchCategoryProductThunk(dispatch) {
    
    dispatch(setCategoryProductsStatus(STATUS.LOADING));

    try {
      const data = await fetchDataBody(
        `${BASE_URL}category/${dataSend.CategoryId}`,
        dataSend,
        method
      );
      await dispatch(setCategoryProducts(data));
      dispatch(setCategoryProductsStatus(STATUS.IDLE));
    } catch (error) {
      dispatch(setCategoryProductsStatus(STATUS.ERROR));
    }
  };
};

// export const fetchProductsByCategory = (categoryName, dataType) => {
//   return async function fetchCategoryProductThunk(dispatch) {
//     if (dataType === "all") dispatch(setCategoriesStatusAll(STATUS.LOADING));

//     try {
//       const response = await fetch(
//         `${BASE_URL}getproductsbycategoryname?categoryName=${categoryName}`
//       );
//       const data = await response.json();
//         dispatch(setCategoriesProductAll(data.result));
//         dispatch(setCategoriesStatusAll(STATUS.IDLE));
//     } catch (error) {
//       dispatch(setCategoriesStatusAll(STATUS.ERROR));
//     }
//   };
// };
