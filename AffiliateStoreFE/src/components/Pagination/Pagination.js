import React, { useEffect } from "react";
import "./Pagination.scss";
import { useState } from "react";
import { useSelector, useDispatch } from "react-redux";
import { setPaginations } from "../../store/paginationSilce";

const Pagination = ({ totalCount }) => {
  const dispatch = useDispatch();
  const [pagination, setPagination] = useState({
    Offet: 1,
    Limit: 10,
  });

  const pages = Array.from(
    { length: totalCount },
    (_, index) => index + 1
  );

  useEffect(() => {
    console.log(pagination);
    dispatch(setPaginations(pagination));
  }, []);

  useEffect(() => {
    dispatch(setPaginations(pagination));
  }, [pagination]);

  const handleLimitChange = (event) => {
    const selectedValue = event.target.value;
    setPagination({
      ...pagination,
      Limit: selectedValue,
    });
  };

  const handleOffsetChange = (page) => {
    setPagination({
      ...pagination,
      Offset: page,
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
