import React from "react";
import "./Pagination.scss";
import { useState } from "react";

const Pagination = ({props}) => {
  const pages = Array.from(
    { length: props.TotalCount },
    (_, index) => index + 1
  );
  const [limitItems, setLimitItems] = useState(10);

  const handleLimitChange = (event) => {
    const selectedValue = event.target.value;
    setLimitItems(selectedValue);
  };

  return (
    <div className="container">
      <ul className="flex pagination">
        {pages.map((page) => (
          <li key={page} className="">
            {page}
          </li>
        ))}
      </ul>
      <select
        className="limit-items"
        name="selectedNumber"
        value={limitItems}
        onChange={handleLimitChange}
      >
        <option value="10">10</option>
        <option value="20">20</option>
        <option value="30">30</option>
      </select>
    </div>
  );
};

export default Pagination;
