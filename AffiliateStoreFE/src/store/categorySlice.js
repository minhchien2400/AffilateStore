import { createSlice } from "@reduxjs/toolkit";
import { BASE_URL } from "../utils/apiURL";
import { STATUS } from "../utils/status";
import { fetchDataBody } from "../utils/fetchData";

const categorySlice = createSlice({
  name: "category",
  initialState: {
    data: [],
    status: STATUS.IDLE,
    catProductAll: [],
    catProductAllStatus: STATUS.IDLE,
    catProductSingle: [],
    catProductSingleStatus: STATUS.IDLE,
  },

  reducers: {
    setCategories(state, action) {
      state.data = action.payload;
    },
    setStatus(state, action) {
      state.status = action.payload;
    },
    // setCategoriesProductAll(state, action) {
    //   state.catProductAll.push(action.payload);
    // },
    setCategoriesProductAll(state, action) {
      state.catProductAll = action.payload;
    },
    setCategoriesStatusAll(state, action) {
      state.catProductAllStatus = action.payload;
    },
    // setCategoriesProductSingle(state, action) {
    //   state.catProductSingle = action.payload;
    // },
    // setCategoriesStatusSingle(state, action) {
    //   state.catProductSingleStatus = action.payload;
    // },
  },
});

export const {
  setCategories,
  setStatus,
  setCategoriesProductAll,
  setCategoriesStatusAll,
  // setCategoriesProductSingle,
  // setCategoriesStatusSingle,
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
      dispatch(setCategories(data.result));
      dispatch(setStatus(STATUS.IDLE));
    } catch (error) {
      dispatch(setStatus(STATUS.ERROR));
    }

    //   try{
    //     const response = await fetch(`${BASE_URL}getcategory`);
    //     const data = await response.json();
    //     dispatch(setCategories(data.slice(0, 5)));
    //     dispatch(setStatus(STATUS.IDLE));
    // } catch(error){
    //     dispatch(setStatus(STATUS.ERROR));
    // }
  };
};

export const fetchProductsByCategory = (categoryName, dataType) => {
  return async function fetchCategoryProductThunk(dispatch) {
    if (dataType === "all") dispatch(setCategoriesStatusAll(STATUS.LOADING));

    try {
      const response = await fetch(
        `${BASE_URL}getproductsbycategoryname?categoryName=${categoryName}`
      );
      const data = await response.json();
        dispatch(setCategoriesProductAll(data.result));
        dispatch(setCategoriesStatusAll(STATUS.IDLE));
    } catch (error) {
      dispatch(setCategoriesStatusAll(STATUS.ERROR));
    }
  };
};

export const fetchProductsByCategoryId = (dataSend, method) => {
  return async function fetchCategoryProductThunk(dispatch) {
    console.log(dataSend);
    // if (dataType === "all") dispatch(setCategoriesStatusAll(STATUS.LOADING));
    // if (dataType === "single")
    //   dispatch(setCategoriesStatusSingle(STATUS.LOADING));

    // try {
    //   const response = await fetchDataBody(
    //     `${BASE_URL}category/${id}`, data, method
    //   );
    //   const data = await response.json();
    //   if (dataType === "all") {
    //     dispatch(setCategoriesProductAll(data.slice(0, 10)));
    //     dispatch(setCategoriesStatusAll(STATUS.IDLE));
    //   }
    //   if (dataType === "single") {
    //     dispatch(setCategoriesProductSingle(data.slice(0, 20)));
    //     dispatch(setCategoriesStatusSingle(STATUS.IDLE));
    //   }
    // } catch (error) {
    //   dispatch(setCategoriesStatusAll(STATUS.ERROR));
    // }

    dispatch(setCategoriesStatusAll(STATUS.LOADING));

    try {
      const data = await fetchDataBody(
        `${BASE_URL}category/${dataSend.CategoryId}`,
        dataSend,
        method
      );
      dispatch(setCategoriesProductAll(data.result));
      dispatch(setCategoriesStatusAll(STATUS.IDLE));
    } catch (error) {
      dispatch(setCategoriesStatusAll(STATUS.ERROR));
    }
  };
};
