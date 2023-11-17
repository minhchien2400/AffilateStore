import "./Filter.scss";
import { useState, useEffect } from "react";

const Filter = () => {
  const [filter, setFilter] = useState("");
  const [order, setOrder] = useState("top-sale");
  const [overStars, setOverStars] = useState(false);

  useEffect(() => {
    setColorButtom();
    console.log("abc");
  }, []);

  const handleOrder = (number) => {
    if (order === "") {
      if (number === 1) {
        setOrder("top-sale");
      } else if (number === 2) {
        setOrder("price-up");
      }
    } else if (order === "top-sale" && number === 1) {
      setOrder("");
    } else if (order === "top-sale" && number === 2) {
      setOrder("price-up");
    } else if (order === "price-up" && number === 2) {
      setOrder("price-down");
    } else if (order === "price-down" && number === 2) {
      setOrder("price-up");
    } else if (
      (order === "price-up" || order === "price-down") &&
      number === 1
    ) {
      setOrder("top-sale");
    }
  };

  const handleOverStars = (overStars) => {
    overStars === true ? setOverStars(false) : setOverStars(true);
  };

  const setColorButtom = () => {
    let className = "";
    if (order === "top-sale") {
      className = "filter-top-sale";
    } else if (order === "price-up") {
      className = "filter-price-icon-up";
    } else if (order === "price-down") {
      className = "filter-price-icon-down";
    }
    const elements = document.getElementsByClassName(className);
    console.log(elements);
    elements[0].style.color = "green";
  };

  return (
    <div className="container">
      <div className="flex filter">
        <button className="filter-top-sale" onClick={() => handleOrder(1)}>
          Top Sale
        </button>
        <div className="filter-price flex" onClick={() => handleOrder(2)}>
          <div>Gia</div>
          <div className="filter-price-icon">
            <div className="filter-price-icon-up">^</div>
            <div className="filter-price-icon-down">v</div>
          </div>
        </div>
        <button
          className="filter-over-stars"
          onClick={() => handleOverStars(overStars)}
        >
          Over 4 Stars
        </button>
      </div>
    </div>
  );
};

export default Filter;
