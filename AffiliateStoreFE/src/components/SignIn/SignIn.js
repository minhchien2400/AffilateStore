import React, { Component, useState } from "react";
import "./SignIn.scss";
import { fetchDataBody } from "../../utils/fetchData";
import { BASE_URL } from "../../utils/apiURL";
import { useSelector, useDispatch } from "react-redux";
import { setLoggedInStatus } from "../../store/loginSlice";
import { setAddToCart } from "../../store/cartSlice";
import { useParams, Link } from "react-router-dom";

const SignIn = () => {
  const dispatch = useDispatch();

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
      const cartData = fetchDataBody(`${BASE_URL}getcartproducts/${data.token}`)
      localStorage.setItem('cart', cartData);
      //dispatch(setProductsAdded(cartData.productsAdded));
      //dispatch(setProductsPurchased(cartData.productsPurchased))
    }

    setDataLogin({
      UsernameOrEmail: "",
      Password: "",
    });
  };

  return (

    <div className="login">
       <Link to='/'>
       <img className="login_logo" src="http://media.corporate-ir.net/media_files/IROL/17/176060/Oct18/Amazon%20logo.PNG" alt=""/>

       </Link>
       <div className="login_container">
           <h1>Sign In</h1>
           <form >
               <h5>E-Mail</h5>
               <input 
            //    value={email} 
            //    onChange={event=>setemail(event.target.value)} 
               type="email"/>
               <h5>Password</h5>
               <input 
            //    value={password} 
            //    onChange={event=>setpassword(event.target.value)} 
               type="password"/>
                <button 
                //onClick={loginb} 
                type="submit" className="signin_btn">Sign-In</button>
           </form>
           <p>By continuing, you agree to Amazon's Conditions of Use and Privacy Notice.</p>
            <button
             //onClick={register}
              className='register_btn'>Create your Amazon account</button>
       </div>
    </div>
)
};
export default SignIn;
