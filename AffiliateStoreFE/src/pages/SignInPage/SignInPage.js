import { useState } from "react";
import { Button } from "react-bootstrap/lib/InputGroup";



const SignIn = () => {
  const [dataLogin, setDataLogin] = useState({
    UserNameOrEmail: "",
    password: "",
  })
  handleSubmit = () => {
    event.preventDefault();

    this.setState({ email: "", password: "" });
  };

  handleChange = (event) => {
    const { value, name } = event.target;

    this.setState({ [name]: value });
  };

  return (
    <div className="sign-in">
      <h2>I already have an account</h2>
      <span>Sign in with your email and password</span>

      <form onSubmit={this.handleSubmit}>
        <input
          name="email"
          type="email"
          onChange={(e) => setDataLogin({
            ...dataLogin,
            UserNameOrEmail: e.target.value
          })}
          value={this.state.email}
          label="email"
          required
        />
        <input
          name="password"
          type="password"
          value={this.state.password}
          onChange={(e) => setDataLogin({
            ...dataLogin,
            password: e.target.value
          })}
          label="password"
          required
        />
        <Button type="submit" onSubmit = {() => handleSubmit()}> Sign in </Button>
      </form>
    </div>
  );
};
