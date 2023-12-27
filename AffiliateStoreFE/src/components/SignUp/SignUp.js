import React, { useState } from "react";
import "./SignUp.scss";
import { fetchDataBody } from "../../utils/fetchData";
import { BASE_URL } from "../../utils/apiURL";
import { useDispatch } from "react-redux";
import { setLoggedInStatus } from "../../store/loginSlice";
import { Link } from "react-router-dom";
import Select from 'react-select';

const SignUp = () => {
  const dispatch = useDispatch();

  const startYear = 2020;
    const endYear = 2120;
    const [showScroll, setShowScroll] = useState(false);

    // Tạo mảng các năm từ startYear đến endYear
    const years = Array.from({ length: endYear - startYear + 1 }, (_, index) => startYear + index);


  const [dataLogin, setDataLogin] = useState({
    UsernameOrEmail: "",
    Password: "",
  });
  const handleSubmit = async (e) => {
    console.log("click sigin");
    // bo default get data tu url
    e.preventDefault();

    // fetch api login
    const data = await fetchDataBody(`${BASE_URL}signin`, dataLogin, "POST");
    console.log("datalogin", dataLogin);

    // luu jwt token va refresh token vao localstorage
    if (data.token) {
      const expires = new Date();
      expires.setDate(expires.getDate() + 7); // Số ngày cookie tồn tại
      console.log("data.token", data.token);
      dispatch(setLoggedInStatus(true));
      localStorage.setItem("jwtToken", data.token);
      localStorage.setItem("refreshToken", data.refreshToken);
      document.cookie = `myCookie=${encodeURIComponent(
        data.token
      )}; expires=${expires.toUTCString()}; path=/`;
      const cartData = fetchDataBody(
        `${BASE_URL}getcartproducts/${data.token}`
      );
      localStorage.setItem("cart", cartData);
      //dispatch(setProductsAdded(cartData.productsAdded));
      //dispatch(setProductsPurchased(cartData.productsPurchased))
    }

    setDataLogin({
      UsernameOrEmail: "",
      Password: "",
    });
  };

  const handleSelectClick = () => {
    setShowScroll(true);
  };

  const handleSelectBlur = () => {
    setShowScroll(false);
  };

  const [selectedCountry, setSelectedCountry] = useState(null);

  const countries = [
    { value: 'VN', label: 'Vietnam' },
    { value: 'US', label: 'United States' },
    { value: 'GB', label: 'United Kingdom' },
    // Thêm các quốc gia khác tương tự
  ];


  const handleChange = (selectedOption) => {
    setSelectedCountry(selectedOption);
  };
  return (
    <div className="login">
      <Link to="/">
        <img
          className="login_logo"
          src="http://media.corporate-ir.net/media_files/IROL/17/176060/Oct18/Amazon%20logo.PNG"
          alt=""
        />
      </Link>
      <div className="login_container">
        <h1>Sign In</h1>
        <form>
          <h5>User name</h5>
          <input
            //    value={email}
            //    onChange={event=>setemail(event.target.value)}
            type="text"
          />

          <h5>Year of birth</h5>
          <select
        id="year"
        name="year"
        size={showScroll ? '10' : '1'}
        onClick={handleSelectClick}
        onBlur={handleSelectBlur}
      >
        {years.map((year) => (
          <option key={year} value={year}>
            {year}
          </option>
        ))}
      </select>

          <h5>Gender</h5>
          <input
            //    value={email}
            //    onChange={event=>setemail(event.target.value)}
            type="text"
          />

          <h5>Country</h5>
          <Select
        value={selectedCountry}
        onChange={handleChange}
        options={countries}
        placeholder="Chọn quốc gia"
      />

          <h5>E-Mail</h5>
          <input
            //    value={email}
            //    onChange={event=>setemail(event.target.value)}
            type="email"
          />
          <h5>Password</h5>
          <input
            //    value={password}
            //    onChange={event=>setpassword(event.target.value)}
            type="password"
          />
          <h5>Re-Password</h5>
          <input
            //    value={password}
            //    onChange={event=>setpassword(event.target.value)}
            type="password"
          />
          <button
            //onClick={loginb}
            type="submit"
            className="signin_btn"
          >
            Sign-Up
          </button>
        </form>
        <p>
          By continuing, you agree to Amazon's Conditions of Use and Privacy
          Notice.
        </p>
        <button
          //onClick={register}
          className="register_btn"
        >
          Create your Amazon account
        </button>
      </div>
    </div>
  );
};
export default SignUp;
