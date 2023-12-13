import React from "react";
import { ratingStars } from "./images";
import { fetchDataBody } from "./fetchData";
import { BASE_URL } from "./apiURL";
import { setLoggedInStatus } from "../store/loginSlice";
import { useDispatch } from "react-redux";


export const formatPrice = (price) => {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
  }).format(price / 100);
};

export const formatStars = (stars) => {
  const starsInt = Math.floor(stars / 10);
  const starIcons = [];
  if (stars % 2 === 0) {
    for (let i = 0; i < starsInt; i++) {
      starIcons.push(ratingStars[0]);
    }
    for (let i = starsInt; i < 5; i++) {
      starIcons.push(ratingStars[2]);
    }
  } else {
    for (let i = 0; i < starsInt; i++) {
      starIcons.push(ratingStars[0]);
    }
    starIcons.push(ratingStars[1]);
    for (let i = starsInt + 1; i < 5; i++) {
      starIcons.push(ratingStars[2]);
    }
  }
  return starIcons;
};

const NAME_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
const EMAIL_CLAIM =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/email";
const GENDER_CLAIM =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/gender";
const COUNTRY_CLAIM =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/country";
const ROLE_CLAIM =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

export const DecodedJwtTokenData = (jwtToken) => {
  try {
    const decodedPayload = atob(jwtToken.split(".")[1]);
    console.log("decodedPaload", decodedPayload);
    const decodedToken = JSON.parse(decodedPayload);
    return {
      UserName: decodedToken[NAME_CLAIM],
      Email: decodedToken[EMAIL_CLAIM],
      Gender: decodedToken[GENDER_CLAIM],
      Country: decodedToken[COUNTRY_CLAIM],
      Roles: decodedToken[ROLE_CLAIM],
    };
  } catch (error) {}
};


