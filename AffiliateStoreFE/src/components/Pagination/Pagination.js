import React from "react";
import "./Pagination.scss";
import { useDispatch } from "react-redux";
import { setFilterAction } from "../../store/filterSlice";

const Pagination = ({ type, filter, totalCount, limitValues }) => {
  const dispatch = useDispatch();
  const pages = Array.from({ length: totalCount }, (_, index) => index + 1);

  const handleLimitChange = (event) => {
    const selectedValue = event.target.value;

    dispatch(
      setFilterAction(type, {
        Offset: 1,
        Limit: selectedValue,
        SearchText: filter.searchText,
        Keys: filter.keys,
      })
    );
  };

  const handleOffsetChange = (page) => {
    dispatch(
      setFilterAction(type, {
        Offset: page,
        Limit: filter.limit,
        SearchText: filter.searchText,
        Keys: filter.keys,
      })
    );
  };

  return (
    <div className="container">
      <ul className="flex pagination">
        {pages.map((page) => (
          <li
            key={page}
            className={page === filter.offset ? "selected" : ""}
            onClick={() => handleOffsetChange(page)}
          >
            {page}
          </li>
        ))}
      </ul>
      <select
        className="limit-items"
        name="selectedNumber"
        value={filter.limit}
        onChange={handleLimitChange}
      >
        {limitValues.map((limitValue) =><option key={limitValue} value={`${limitValue}`}>{limitValue}</option>)}
      </select>
    </div>
  );
};

export default Pagination;
