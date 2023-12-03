


const SignIn = () => {
  handleSubmit = (event) => {
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
        <FormInput
          name="email"
          type="email"
          handleChange={this.handleChange}
          value={this.state.email}
          label="email"
          required
        />
        <FormInput
          name="password"
          type="password"
          value={this.state.password}
          handleChange={this.handleChange}
          label="password"
          required
        />
        <CustomButton type="submit"> Sign in </CustomButton>
      </form>
    </div>
  );
};
