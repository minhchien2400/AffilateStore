import { useDispatch } from "react-redux";
import { setFilterAction } from "../../store/filterSlice";
import { SET_CATEGORY_FILTER as categoryFilter } from "../../utils/const";
import "./CategoryFilter.scss";

const CategoryFilter = ({ filter }) => {
  const dispatch = useDispatch();

  const orderCategoryValues = ["all", "a-z", "z-a"];

  const orderCategoryDisplay = ["All", "A-Z", "Z-A"];

  const handleSetOrder = (orderValue) => {
    dispatch(
      setFilterAction(categoryFilter, {
        Offset: filter.offset,
        Limit: filter.limit,
        SearchText: filter.searchText,
        Keys: [orderValue],
      })
    );
  };

  return (
    <div className="container">
      <div className="filter flex">
        Filter:
        {orderCategoryValues.map((orderCategoryValue, index) => (
          <button
            key={index}
            className={
              (filter && filter.keys && (filter.keys[0] === orderCategoryValue))
                ? "filter-time-selected"
                : ""
            }
            onClick={() => handleSetOrder(orderCategoryValue)}
          >
            {orderCategoryDisplay[index]}
          </button>
        ))}
      </div>
    </div>
  );
};

export default CategoryFilter;
