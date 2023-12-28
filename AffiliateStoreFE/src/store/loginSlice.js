import { createSlice } from "@reduxjs/toolkit";
import { fetchDataBody } from "../utils/fetchData";
import { BASE_URL } from "../utils/apiURL";

const loginSlice = createSlice({
  name: "login",
  initialState: {
    IsLoggedIn: false,
    DataSignIn: {},
    DataSignUp: {},
    DataForgetPassword: {},
    DataChangePassword: {},
  },
  reducers: {
    setDataLogin(state, action) {
      switch (action.payload.type) {
        case "SET_LOGIN_STATUS":
          state.IsLoggedIn = action.payload.value;
        case "SET_DATA_SIGNIN":
          state.DataSignIn = action.payload.value;
        case "SET_DATA_SIGNUP":
          state.DataSignUp = action.payload.value;
        case "SET_FORGET_PASSWORD":
          state.DataForgetPassword = action.payload.value;
        case "SET_CHANGE_PASSWORD":
          state.DataChangePassword = action.payload.value;
      }
    },
  },
});

export const { setDataLogin } = loginSlice.actions;
export default loginSlice.reducer;

export const setInitDataLogin = (type, value) => {
  return async function setUserInfoThunk(dispatch) {
    dispatch(setDataLogin({type: type, value: value}));
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
  return async function fetchRefreshTokenThunk(dispatch, token) {
    //dispatch(setLoggedIn(true));
  };
};
