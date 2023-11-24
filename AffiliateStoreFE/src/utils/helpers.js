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

// export const setupGlobalSetting = () => {
//   // Kiểm tra xem đã lưu global setting hay chưa
//   const storedSetting = localStorage.getItem('globalSetting');

//   if (!storedSetting) {
//     // Nếu chưa có global setting, lấy kích thước màn hình và cài đặt
//     const screenSize = {
//       width: window.innerWidth,
//       height: window.innerHeight
//     };

//     // Thực hiện cài đặt global setting dựa trên kích thước màn hình
//     const globalSetting = {
//       // ...thêm các thuộc tính global setting khác dựa trên kích thước màn hình
//       screenSize,
//       // Ví dụ: có thể cài đặt theme, font size, etc.
//     };

//     // Lưu global setting vào localStorage
//     localStorage.setItem('globalSetting', JSON.stringify(globalSetting));
//   }
// }

// // Gọi hàm khi trang web được tải
