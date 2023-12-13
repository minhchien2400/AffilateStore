import { createSlice } from "@reduxjs/toolkit";
import { fetchDataBody } from "../utils/fetchData";
import { BASE_URL } from "../utils/apiURL";

const loginSlice = createSlice({
  name: "login",
  initialState: {
    IsLoggedIn: false,
  },
  reducers: {
    setLoggedIn(state, action) {
      state.IsLoggedIn = action.payload;
    },
  },
});

export const { setLoggedIn } = loginSlice.actions;
export default loginSlice.reducer;

export const setLoggedInStatus = (loginStatus) => {
  console.log("dispath: loginStatus", loginStatus);
  return async function setUserInfoThunk(dispatch) {
    dispatch(setLoggedIn(loginStatus))
  };
};

export const fetchRefreshToken = async () => {
  const token = await fetchDataBody(
    `${BASE_URL}refresh`,
    {
      AccessToken: localStorage.getItem("jwtToken"),
      RefreshToken: localStorage.getItem("refreshToken"),
    },
    "POST"
  );
  localStorage.setItem("jwtToken", token.jwtToken);
  localStorage.setItem("refreshToken", token.refreshToken);
  return async function fetchRefreshTokenThunk(dispatch, token){
    dispatch(setLoggedIn(true));
  }
};
