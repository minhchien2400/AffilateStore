import "./Filter.scss";
import { useState, useEffect } from "react";
import { useDispatch } from "react-redux";
import { setOrderFilter } from "../../store/filterSlice";

const Filter = ({data}) => {
  const dispatch = useDispatch();

  const [filter, setFilter] = useState({ Price: "all", Stars: "all" });

  const [currentPriceIndex, setCurrentPriceIndex] = useState(0);
  const [currentStarsIndex, setCurrentStarsIndex] = useState(0);

  const orderPriceValues = ["all", "price-up", "price-down", "top-sale"];
  const orderStarsValues = ["all", "over-3-stars", "over-4-stars"];

  const orderPriceDisplay = ["All", "Increasing", "Descending", "Flash sale"];
  const orderStarsDisplay = ["All", "Over 3 stars", "Over 4 stars"];

  useEffect(() => {
    dispatch(setOrderFilter(filter));
  }, [filter]);

  const handleSetOrder = (type, orderValue, index) => {
    if (type === 0) {
      dispatch(setOrderFilter({
        Price: orderValue,
        Stars: data.filter.keys[1]
      }))
      setCurrentPriceIndex(index);
    } else if (type === 1) {
      dispatch(setOrderFilter({
        Price: data.filter.keys[0],
        Stars: orderValue
      }))
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
              {index === 1 ? <i class="fa-solid fa-arrow-up"/> : (index === 2 ? <i class="fa-solid fa-arrow-down"/> : "")}          </button>
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
