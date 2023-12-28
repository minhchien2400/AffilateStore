// cartSilce
export const SET_ADD_PRODUCTS = "SET_ADD_PRODUCTS";
export const SET_PURCHASED_PRODUCTS = "SET_PURCHASED_PRODUCTS";
export const ADD_TO_CART = "ADD_TO_CART";
export const SET_TOTAL_ADDED = "SET_TOTAL_ADDED";
export const SET_MARK_PURCHASED = "SET_MARK_PURCHASED";
export const SET_TOTAL_PURCHASED = "SET_TOTAL_PURCHASED";
export const SET_REMOVE_ADDED = "SET_REMOVE_ADDED";

export const ActionTypeCart = Object.freeze({
  Add: 0,
  Purchase: 1,
  Remove: 2,
});

export const CartStatus = Object.freeze({
  Added: 0,
  Purchased: 1,
  Removed: 2,
});

// filterSlice
export const SET_PRODUCTS_FILTER = "SET_PRODUCTS_FILTER";
export const SET_CATEGORY_FILTER = "SET_CATEGORY_FILTER";
export const CART_ADDED_FILTER = "CART_ADDED_FILTER";
export const CART_PURCHASED_FILTER = "CART_PURCHASED_FILTER";

// list countries
export const countries = [
  { value: 'VN', label: 'Vietnam' },
  { value: 'US', label: 'United States' },
  { value: 'GB', label: 'United Kingdom' },
  // Thêm các quốc gia khác tương tự
];

// list genders
const Gender = Object.freeze({
  Male: 0,
  Female: 1,
  Orther: 2,
});
export const genders = [
  { value: Gender.Male, label: 'Male' },
  { value: Gender.Female, label: 'Female' },
  { value: Gender.Orther, label: 'Orther' },
];

// option birth years
export const listYearsSelect = (startYear, endYear) => {
  let years = [];
  for (let nam = startYear; nam <= endYear; nam++) {
    years.push({ value: nam, label: nam.toString() });
  }
  return years;
};
  






