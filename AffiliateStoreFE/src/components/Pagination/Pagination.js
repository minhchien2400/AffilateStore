import React, { useEffect } from "react";
import "./Pagination.scss";
import { useState } from "react";

const Pagination = ({props}) => {
  const pages = Array.from(
    { length: props.TotalCount },
    (_, index) => index + 1
  );
  const [pagination, setPagination] = useState({
    Limit: props.filter.Limit,
    Offset: props.filter.Offset
  })

  useEffect(() => {
    
  }, [pagination])

  const handleLimitChange = (event) => {
    const selectedValue = event.target.value;
    setPagination({
      ...pagination,
      Limit: selectedValue
    });
  };

  const handleOffsetChange = (page) => {
    setPagination({
      ...pagination,
      Offset: page
    });
  };

  return (
    <div className="container">
      <ul className="flex pagination">
        {pages.map((page) => (
          <li key={page} className="" onClick={() => handleOffsetChange(page)}>
            {page}
          </li>
        ))}
      </ul>
      <select
        className="limit-items"
        name="selectedNumber"
        value={pagination.Limit}
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
