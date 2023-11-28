import React from "react";
import "./Pagination.scss";
import { useDispatch } from "react-redux";
import { setOrderFilter } from "../../store/filterSlice";
import { useSelector } from "react-redux";

const Pagination = ({ data }) => {
  const dispatch = useDispatch();
  const pages = Array.from(
    { length: data.totalCount },
    (_, index) => index + 1
  );


  const handleLimitChange = (event) => {
    const selectedValue = event.target.value;

    dispatch(
      setOrderFilter({
        Offset: 1,
        Limit: selectedValue,
        SearchText: data.filter.searchText,
        Keys: data.filter.keys,
      })
    );
  };

  const handleOffsetChange = (page) => {
    dispatch(
      setOrderFilter({
        Offset: page,
        Limit: data.filter.limit,
        SearchText: data.filter.searchText,
        Keys: data.filter.keys,
      })
    );
  };

  return (
    <div className="container">
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
