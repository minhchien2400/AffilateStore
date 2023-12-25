import { useDispatch } from "react-redux";
import { setFilterAction } from "../../store/filterSlice";
import { CART_ADDED_FILTER as addedFilter} from "../../utils/const";
import { CART_PURCHASED_FILTER as purchasedFilter} from "../../utils/const";
import "./CartFilter.scss"; 

const CartFilter = ({ filter, isCart }) => {
  const dispatch = useDispatch();

  const orderCartValues = ["all", "time-up", "time-down", "top-sale"];

  const orderCartDisplay = ["All", "Increasing", "Descending", "Flash sale"];

  const handleSetOrder = (orderValue) => {
    console.log("CLICK SET FILTER", orderValue);
    console.log(filter);
    
      dispatch(
        setFilterAction(isCart ? addedFilter : purchasedFilter, {
          Offset: filter.Offset,
          Limit: filter.Limit,
          SearchText: filter.SearchText,
          Keys: [orderValue],
        })
      );
  };

  return (
    <div className="container">
      <div className="filter flex">
        <div className="filter-time flex">
          Time:
          {orderCartValues.map((orderCartValue, index) => (
            <button
              key={index}
              className={
                 (filter && orderCartValue === filter.Keys[0]) ? "filter-time-selected" : ""
              }
              onClick={() => handleSetOrder(orderCartValue)}
            >
              {orderCartDisplay[index]}
              {index === 1 ? (
                <i class="fa-solid fa-arrow-up" />
              ) : index === 2 ? (
                <i class="fa-solid fa-arrow-down" />
              ) : (
                ""
              )}{" "}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default CartFilter;
