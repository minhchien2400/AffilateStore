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

  const { data: dataFilter } = useSelector((state) => state.filter);


  const handleLimitChange = (event) => {
    const selectedValue = event.target.value;

    console.log(dataFilter);
    dispatch(
      setOrderFilter({
        ...dataFilter,
        Offset: 1,
        Limit: selectedValue,
        //Keys: data.filter.keys,
        // Price: data.filter.keys[0],
        // Limit: data.filter.keys[1],
      })
    );
  };

  const handleOffsetChange = (page) => {
    console.log(dataFilter);
    dispatch(
      setOrderFilter({
        ...dataFilter,
        Offset: page,
        // Limit: data.filter.limit,
        // Keys: data.filter.keys,
        // Price: data.filter.keys[0],
        // Limit: data.filter.keys[1],
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
