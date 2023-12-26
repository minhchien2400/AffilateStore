import { useDispatch } from "react-redux";
import { setFilterAction } from "../../store/filterSlice";
import { CART_ADDED_FILTER as addedFilter } from "../../utils/const";
import { CART_PURCHASED_FILTER as purchasedFilter } from "../../utils/const";
import "./CartFilter.scss";

const CartFilter = ({ filter, isCart }) => {
  const dispatch = useDispatch();

  const orderCartValues = ["time-up", "time-down", "a-z", "z-a", "top-sale"];

  const orderCartDisplay = ["", "", "a-z", "z-a", "Flash sale"];

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
                filter && filter.Keys[0] === orderCartValue
                  ? "filter-time-selected"
                  : ""
              }
              onClick={() => handleSetOrder(orderCartValue)}
            >
              {orderCartDisplay[index]}
              {index === 0 ? (
                <i class="fa-solid fa-arrow-up" />
              ) : index === 1 ? (
                <i class="fa-solid fa-arrow-down" />
              ) : (
                ""
              )}
              {orderCartValue === "top-sale" && (
                <i class="fa-solid fa-bolt-lightning" />
              )}{" "}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default CartFilter;
