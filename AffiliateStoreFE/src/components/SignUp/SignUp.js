import React, { Component } from "react";
import "./SignIn.scss";

const SignUp = () => {
  const handleChange = (e) => {
    const { value, name } = e.target;
    this.setState({ [name]: value });
  };

  const handleSubmit = async (event) => {
    this.props.loginUser(this.state, this.props.history);

    event.preventDefault();
  };
  return (
    <div className="sign-in">
      <h2>I already have an account</h2>
      <span>Sign in with your email and password</span>

      <div className="input-container flex">
        <input
          name="username"
          type="text"
          value={""}
          handleChange={handleChange}
          label="User name"
          required
        />
        <input
          name="email"
          type="email"
          value={""}
          handleChange={handleChange}
          label="Email"
          required
        />
        <input
          name="password"
          type="password"
          value={""}
          handleChange={handleChange}
          label="Password"
          required
        />
        <input
          name="password"
          type="password"
          value={""}
          handleChange={handleChange}
          label="Password"
          required
        />
      </div>
      <div className="buttons">
        <button className="submit" type="submit">
          Create account
        </button>
        {/* <button isGoogleSignIn={true}>
            {""}
            SIGN IN With Google{""}
          </button> */}
      </div>
    </div>
  );
};
export default SignUp;
