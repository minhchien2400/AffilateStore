import React from "react";
import { ratingStars } from "./images";

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
