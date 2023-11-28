import { createSlice } from "@reduxjs/toolkit";

const filterSlice = createSlice({
  name: "filter",
  initialState: {
    data: {
      Offset: 1,
      Limit: 10,
      SearchText: "",
      Keys: ["all", "all"],
    },
    categoryData: {
      Offset: 1,
      Limit: 5,
      SearchText: "",
      Keys: ["all"],
    },
  },
  reducers: {
    setFilters(state, action) {
      state.data = action.payload;
    },
    setCategoryFilters(state, action) {
        state.categoryData = action.payload;
      },
  },
});

export const { setFilters, setCategoryFilters } = filterSlice.actions;
export default filterSlice.reducer;

export const setOrderFilter = (filter) => {
  return async function setFilterThunk(dispatch) {
    dispatch(setFilters(filter));
  };
};

export const setCategoryOrderFilter = (filter) => {
    return async function setFilterThunk(dispatch) {
      dispatch(setCategoryFilters(filter));
    };
  };
