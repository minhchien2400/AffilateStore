import "./Filter.scss";
import { useState, useEffect } from "react";
import { useDispatch } from "react-redux";
import { setOrderFilter } from "../../store/filterSlice";

const Filter = ({ data }) => {
  const dispatch = useDispatch();

  const orderPriceValues = ["all", "price-up", "price-down", "top-sale"];
  const orderStarsValues = ["all", "over-3-stars", "over-4-stars"];

  const orderPriceDisplay = ["All", "Increasing", "Descending", "Flash sale"];
  const orderStarsDisplay = ["All", "Over 3 stars", "Over 4 stars"];

  const handleSetOrder = (type, orderValue) => {
    if (type === 0) {
      dispatch(
        setOrderFilter({
          Offset: data.filter.offset,
          Limit: data.filter.limit,
          SearchText: data.filter.searchText,
          Keys: [orderValue, data.filter.keys[1]],
        })
      );
    } else if (type === 1) {
      dispatch(
        setOrderFilter({
          Offset: data.filter.offset,
          Limit: data.filter.limit,
          SearchText: data.filter.searchText,
          Keys: [data.filter.keys[0], orderValue],
        })
      );
    }
  };

  return (
    <div className="container">
      {console.log(data.filter.keys)}
      <div className="filter flex">
        <div className="filter-price flex">
          Price:
          {orderPriceValues.map((orderPriceValue, index) => (
            <button
              key={index}
              className={
                 orderPriceValue === data.filter.keys[0] ? "filter-price-selected" : ""
              }
              onClick={() => handleSetOrder(0, orderPriceValue)}
            >
              {orderPriceDisplay[index]}
              {index === 1 ? (
                <i class="fa-solid fa-arrow-up" />
              ) : index === 2 ? (
                <i class="fa-solid fa-arrow-down" />
              ) : (
                ""
              )}{" "}
            </button>
          ))}
        </div>
        <div className="filter-stars flex">
          Stars:
          {orderStarsValues.map((orderStarsValue, index) => (
            <button
              key={index}
              className={
                 orderStarsValue === data.filter.keys[1] ? "filter-stars-selected" : ""
              }
              onClick={() => handleSetOrder(1, orderStarsValue, index)}
            >
              {orderStarsDisplay[index]}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default Filter;
