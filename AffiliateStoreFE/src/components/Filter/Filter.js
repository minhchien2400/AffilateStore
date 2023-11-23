import "./Filter.scss";
import { useState, useEffect } from "react";

const Filter = ({sendFilterItems}) => {
  const [filter, setFilter] = useState({ Price: "all", Stars: "all" });
  const [orderPrice, setOrderPrice] = useState("all");
  const [orderStars, setOrderStars] = useState("all");

  const [currentPriceIndex, setCurrentPriceIndex] = useState(0);
  const [currentStarsIndex, setCurrentStarsIndex] = useState(0);

  const orderPriceValues = ["all", "price-up", "price-down"];
  const orderStarsValues = ["all", "over-3-stars", "over-4-stars"];

  const orderPriceDisplay = ["All", "Increasing", "Descending"];
  const orderStarsDisplay = ["All", "Over 3 stars", "Over 4 stars"];

  useEffect(() => {
    setFilter({
      Price: orderPrice,
      Stars: orderStars,
    });
    sendFilterItems(filter);
  }, [orderPrice, orderStars]);

  const handleSetOrder = (type, orderValue, index) => {
    if (type === 0) {
      setOrderPrice(orderValue);
      setCurrentPriceIndex(index);
    } else if (type === 1) {
      setOrderStars(orderValue);
      setCurrentStarsIndex(index);
    }
  };

  return (
    <div className="container">
      <div className="filter flex">
        <div className="filter-price flex">
          Price:
          {orderPriceValues.map((orderPriceValue, index) => (
            <button
              key={index}
              className={
                index === currentPriceIndex ? "filter-price-selected" : ""
              }
              onClick={() => handleSetOrder(0, orderPriceValue, index)}
            >
              {orderPriceDisplay[index]}
              {index !== 0 ? (
                index === 1 ? (
                  <i class="fa-solid fa-arrow-up"></i>
                ) : (
                  <i class="fa-solid fa-arrow-down"></i>
                )
              ) : (
                ""
              )}
            </button>
          ))}
        </div>
        <div className="filter-stars flex">
          Stars:
          {orderStarsValues.map((orderStarsValue, index) => (
            <button
              key={index}
              className={
                index === currentStarsIndex ? "filter-stars-selected" : ""
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
