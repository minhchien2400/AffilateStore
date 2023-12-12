import React, { Component, useState } from "react";
import "./SignIn.scss";
import { fetchDataBody } from "../../utils/fetchData";
import { BASE_URL } from "../../utils/apiURL";

const SignIn = () => {
  const [dataLogin, setDataLogin] = useState({
    UsernameOrEmail: "",
    Password: "",
  });
  const handleSubmit = async (e) => {

    // bo default get data tu url
    e.preventDefault();

    // fetch api login
    const data = await fetchDataBody(`${BASE_URL}signin`, dataLogin, "POST");

    // luu jwt token va refresh token vao localstorage
    localStorage.setItem("jwtToken", data.token);
    localStorage.setItem("refreshToken", data.refreshToken);
    setDataLogin({
      UsernameOrEmail: "",
      Password: "",
    });
  };

  return (
    <div className="sign-in">
      <h2>I already have an account</h2>
      <span>Sign in with your email and password</span>

      <form onSubmit={handleSubmit}>
        <input
          name="email"
          type="email"
          onChange={(e) =>
            setDataLogin({
              ...dataLogin,
              UsernameOrEmail: e.target.value,
            })
          }
          value={dataLogin.UsernameOrEmail}
        />
        <input
          name="password"
          type="password"
          value={dataLogin.Password}
          onChange={(e) =>
            setDataLogin({
              ...dataLogin,
              Password: e.target.value,
            })
          }
        />
        {/* <input
          name="remember"
          type="checkbox"
          value={true}
        /> */}
        <button type="submit"> Sign in </button>
      </form>
    </div>
  );
};
export default SignIn;
