import { createSlice } from "@reduxjs/toolkit";

const pageSlice = createSlice({
  name: "page",
  initialState: {
    data: {
      IsHomePage: true,
      IsCategoryPage: false,
      IsProductsPage: false,
      IsSingleProductPage: false,
      IsCartAddedPage: false,
      IsCartPurchasedPage: false
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
  return function setFilterThunk(dispatch, getState) {
    const updatedPage = {
      ...getState().page.data,
      ...page,
    };
    const updatedData = Object.keys(updatedPage).reduce((acc, key) => {
      acc[key] = key === Object.keys(page)[0] ? updatedPage[key] : false;
      return acc;
    }, {});
    console.log("updatedData la: ", updatedData);

    dispatch(setPage(updatedData));
  };
};