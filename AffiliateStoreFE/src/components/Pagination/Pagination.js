import React from "react";
import "./Pagination.scss";
import { useDispatch } from "react-redux";
import { setPaginations } from "../../store/paginationSilce";

const Pagination = ({ data }) => {
  const dispatch = useDispatch();
  const pages = Array.from(
    { length: data.totalCount },
    (_, index) => index + 1
  );

  const handleLimitChange = (event) => {
    const selectedValue = event.target.value;
    dispatch(
      setPaginations({
        Offset: 1,
        Limit: selectedValue,
      })
    );
  };

  const handleOffsetChange = (page) => {
    dispatch(
      setPaginations({
        Offset: page,
        Limit: data.filter.limit,
      })
    );
  };

  return (
    <div className="container">
      {console.log("re-render Pagination")}
      <ul className="flex pagination">
        {pages.map((page) => (
          <li
            key={page}
            className={page === data.filter.offset ? "selected" : ""}
            onClick={() => handleOffsetChange(page)}
          >
            {page}
          </li>
        ))}
      </ul>
      <select
        className="limit-items"
        name="selectedNumber"
        value={data.filter.limit}
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
