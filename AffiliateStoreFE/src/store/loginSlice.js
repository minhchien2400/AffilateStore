import { createSlice } from "@reduxjs/toolkit";

const jwtToken = localStorage.getItem('jwtToken');
const CheckLoggedIn = (jwtToken) => {
  return jwtToken !== null;
}
const loginSlice = createSlice({
  name: "login",
  initialState: {
    IsLoggedIn: CheckLoggedIn(),
  },
  reducers: {
    setLoggedIn(state, action) {
      state.data = action.payload;
    },
  },
});

export const { setLoggedIn } = loginSlice.actions;
export default loginSlice.reducer;

export const setLoggedInStatus = (loginStatus) => {
  return async function setUserInfoThunk(dispatch) {
    dispatch(setLoggedIn(loginStatus))
  };
};
