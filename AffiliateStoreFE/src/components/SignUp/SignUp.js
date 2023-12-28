import React, { useState } from "react";
import "./SignUp.scss";
import { fetchDataBody } from "../../utils/fetchData";
import { BASE_URL } from "../../utils/apiURL";
import { useDispatch } from "react-redux";
import { setInitDataLogin } from "../../store/loginSlice";
import { Link } from "react-router-dom";
import Select from "react-select";
import { countries, genders, listYearsSelect } from "../../utils/const";

const SignUp = () => {
  const dispatch = useDispatch();

  // Tạo mảng các năm từ startYear đến endYear
  const startYear = 2020;
  const endYear = 2120;
  const [selectedBirthYear, setSelectedBirthYear] = useState(null);
  const years = listYearsSelect(startYear, endYear);
  const handleChangeBirthYear = (selectedOption) => {
    setSelectedBirthYear(selectedOption);
  };

  // select country
  const [selectedCountry, setSelectedCountry] = useState(null);
  const handleChangeCountry = (selectedOption) => {
    setSelectedCountry(selectedOption);
  };

  //select gender
  const [selectedGender, setSelectedGender] = useState(null);
  const handleChangeGender = (selectedOption) => {
    setSelectedGender(selectedOption);
  };

  const [dataSignUp, setDataSignUp] = useState({
    Username: "",
    BirthYear: 0,
    Gender: 0,
    Email: "",
    Country: "",
    Password: "",
    RePassword: "",
  });
  const handleSubmit = async (e) => {
    console.log("click sigin");
    // bo default get data tu url
    e.preventDefault();

    // fetch api login
    const data = await fetchDataBody(`${BASE_URL}signin`, dataSignUp, "POST");
    console.log("datalogin", dataSignUp);

    // luu jwt token va refresh token vao localstorage
    if (data.token) {
      const expires = new Date();
      expires.setDate(expires.getDate() + 7); // Số ngày cookie tồn tại
      console.log("data.token", data.token);
      dispatch(setInitDataLogin(true));
      localStorage.setItem("jwtToken", data.token);
      localStorage.setItem("refreshToken", data.refreshToken);
      document.cookie = `myCookie=${encodeURIComponent(
        data.token
      )}; expires=${expires.toUTCString()}; path=/`;
      const cartData = fetchDataBody(
        `${BASE_URL}getcartproducts/${data.token}`
      );
      localStorage.setItem("cart", cartData);
    }
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
            value={dataSignUp.Username}
            onChange={(event) =>
              setDataSignUp({ ...dataSignUp, Username: event.target.value })
            }
            type="text"
          />
          <h5>Year of birth</h5>
          <Select
            value={dataSignUp.BirthYear}
            onChange={(event) =>
              setDataSignUp({ ...dataSignUp, BirthYear: event.target.value })
            }
            options={years}
            placeholder="Chon nam sinh"
          />
          <h5>Gender</h5>
          <Select
            value={dataSignUp.Gender}
            onChange={(event) =>
              setDataSignUp({ ...dataSignUp, Gender: event.target.value })
            }
            options={genders}
            placeholder="Chon gioi tinh"
          />
          <h5>Country</h5>
          <Select
            value={dataSignUp.Country}
            onChange={(event) =>
              setDataSignUp({ ...dataSignUp, Country: event.target.value })
            }
            options={countries}
            placeholder="Chọn quốc gia"
          />
          <h5>E-Mail</h5>
          <input
            value={dataSignUp.Email}
            onChange={(event) =>
              setDataSignUp({ ...dataSignUp, Email: event.target.value })
            }
            type="email"
          />
          <h5>Password</h5>
          <input
            value={dataSignUp.Password}
            onChange={(event) =>
              setDataSignUp({ ...dataSignUp, Password: event.target.value })
            }
            type="password"
          />
          <h5>Re-Password</h5>
          <input
            value={dataSignUp.RePassword}
            onChange={(event) =>
              setDataSignUp({ ...dataSignUp, RePassword: event.target.value })
            }
            type="password"
          />
          <button
            onClick={handleSubmit}
            type="submit"
            className="signin_btn"
          >
            Sign-Up
          </button>
        </form>
        <p>Bạn đã có tài khoản rồi ?</p>
        <button onClick={''} className="register_btn">
          Sign-In
        </button>
      </div>
    </div>
  );
};
export default SignUp;
