import React, { Component, useState } from "react";
import { ValidationEmail } from "../../utils/validation";
import "./SignIn.scss";
import { Link } from "react-router-dom";

const SignIn = () => {
  const [emailInput, setEmailInput] = useState("");
  const [passwordInput, setPasswordInput] = useState("");
  const [validEmail, setValidEmail] = useState(true);

  const handleSignIn = async () => {
    setValidEmail(true);
    if (!ValidationEmail(emailInput));
    {
      setValidEmail(false);
      return;
    }
  };
  return (
    <div className="sign-in">
      <h2>I already have an account</h2>
      <span>Sign in with your email and password</span>

      <div className="input-container flex">
        <input
          name="email"
          type="email"
          value={emailInput}
          onChange={(e) => setEmailInput(e.target.value)}
          label="Email"
          required
        />
        {!validEmail && "Email not valid !"}
        <input
          name="password"
          type="password"
          value={passwordInput}
          onChange={(e) => setPasswordInput(e.target.value)}
          label="Password"
          required
        />
      </div>
      <div className="buttons">
        <button className="submit" type="submit" onClick={() => handleSignIn()}>
          SIGN IN
        </button>
        <Link to="/signup">
          <button>
            {""}
            SIGN UP{""}
          </button>
        </Link>
      </div>
    </div>
  );
};
export default SignIn;
