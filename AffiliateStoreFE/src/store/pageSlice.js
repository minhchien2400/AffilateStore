import { createSlice } from "@reduxjs/toolkit";

const pageSlice = createSlice({
  name: "page",
  initialState: {
    data: {
      IsHomePage: true,
      IsCategoryPage: false,
      IsSingleProductPage: false,
    },
  },
  reducers: {
    setPage(state, action) {
      state.data = action.payload;
    },
  },
});

export const { setPage } = pageSlice.actions;
export default pageSlice.reducer;

export const setPageState = (page) => {
  return async function setFilterThunk(dispatch) {
    dispatch(setPage(page));
  };
};

export const fetchSearchProducts = (dataSend, method, page) => {
  return async function fetchProductThunk(dispatch) {
    dispatch(setStatus(STATUS.LOADING));
    try {
      const data = await fetchDataBody(
        `${BASE_URL}getproducts`,
        dataSend,
        method
      );
      console.log(data);
      if (data.hasError) {
        dispatch(setStatus(STATUS.ERROR));
      }
      dispatch(setDataSearch(data.result));
      dispatch(setStatus(STATUS.IDLE));
    } catch (error) {
      dispatch(setStatus(STATUS.ERROR));
    }
  };
};
