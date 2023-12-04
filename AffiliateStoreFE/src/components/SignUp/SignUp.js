import React, { Component, useState } from "react";
import "./SignUp.scss";
import { ValidationEmail, ValidationPassword } from "../../utils/validation";

const SignUp = () => {

const [inputSignUp, setInputSignUp] = useState({
  Email: '',
  PassWord: '',
  RePassWord: ''
});

const [mesageFailed, setMesageFailed] = useState({
  emailFailed: '',
  passwordFailed: '',
  rePasswordFailed: ''
});

  const handleSignUp = () => {
    if(ValidationEmail(inputSignUp.Email)) return () => {setMesageFailed({
      ...mesageFailed,
      emailFailed: 'Email phai abc xyz'
    })}
    if(ValidationPassword(inputSignUp.PassWord)) return () => {setMesageFailed({
      ...mesageFailed,
      passwordFailed: 'Password phai abc xyz'
    })}
    if(inputSignUp.PassWord !== inputSignUp.RePassWord) return () => {setMesageFailed({
      ...mesageFailed,
      rePasswordFailed: 'RePassword phai abc xyz'
    })}
    
  }
  return (
    <div className="sign-in">
      <h2>I already have an account</h2>
      <span>Sign in with your email and password</span>

      <div className="input-container flex">
        <input
          name="email"
          type="email"
          value={""}
          onChange={(e) => setInputSignUp({
            ...inputSignUp,
            Email: e.target.value
          })}
          label="Email"
          placeholder="Nhap Email"
          required
        />
        <input
          name="password"
          type="password"
          value={""}
          onChange={(e) => setInputSignUp({
            ...inputSignUp,
            PassWord: e.target.value
          })}
          label="Password"
          placeholder="Nhap Password"
          required
        />
        <input
          name="password"
          type="password"
          value={""}
          onChange={(e) => setInputSignUp({
            ...inputSignUp,
            RePassWord: e.target.value
          })}
          label="Password"
          placeholder="Nhap RePassword"
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
