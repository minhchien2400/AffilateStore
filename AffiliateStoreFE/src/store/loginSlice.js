import { createSlice } from "@reduxjs/toolkit";

const loginSlice = createSlice({
  name: "login",
  initialState: {
    signInData: {
      Email: '',
      Password: '',
    },
    signUpData: {
        Email: '',
        Password: '',
        RePassword: ''
    },
    forgotPassword: {
        Email: '',
        OldPassword: '',
        NewPassword: '',
        ReNewPassword: ''
    },
    resetPassword: {
      Email: '',
      Code: ''
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
  console.log("setOrderFilter", filter);
  return async function setFilterThunk(dispatch) {
    await dispatch(setFilters(filter));
  };
};

export const setCategoryOrderFilter = (filter) => {
  return async function setFilterThunk(dispatch) {
    dispatch(setCategoryFilters(filter));
  };
};
