import React, { useEffect, useState } from "react";
import "./CartPage.scss";
import { useSelector, useDispatch } from "react-redux";
import { Link } from "react-router-dom";
import { formatPrice } from "../../utils/helpers";
import { fetchCartProducts, fetchCartAction } from "../../store/cartSlice";
import Pagination from "../../components/Pagination/Pagination";
import {
  CART_ADDED_FILTER,
  CART_PURCHASED_FILTER,
  CartStatus,
} from "../../utils/const";
import { ActionTypeCart } from "../../utils/const";
import { setPageState } from "../../store/pageSlice";
import { setFilterAction } from "../../store/filterSlice";
import CartFilter from "../../components/Filter/CartFilter";
import { useNavigate } from "react-router-dom";

const CartPage = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { ProductsAdded: productsAdded } = useSelector((state) => state.cart);
  const { ProductsPurchased: productsPurchased } = useSelector(
    (state) => state.cart
  );

  //   const { IsLoggedIn: isLoggedIn } = useSelector((state) => state.login);
  const { CartAddedFilter: addedFilter } = useSelector((state) => state.filter);
  const { CartPurchasedFilter: purchasedFilter } = useSelector(
    (state) => state.filter
  );

  const [isCart, setIsCart] = useState(true);
  const products = isCart ? productsAdded : productsPurchased;
  const filter = isCart ? addedFilter : purchasedFilter;

  const [searchCart, setSearchCart] = useState("");

  const dataSend = {
    AccessToken: localStorage.getItem("jwtToken"),
    CartStatus: isCart ? CartStatus.Added : CartStatus.Purchased,
    Offset: filter.Offset,
    Limit: filter.Limit,
    SearchText: filter.SearchText,
    Keys: filter.Keys,
  };

  useEffect(() => {
    dispatch(
      setPageState({
        IsCartPage: true,
      })
    );

    dispatch(fetchCartProducts(dataSend, "POST"));
  }, [filter, isCart]);

  const handleRemoveFromCart = async (productId) => {
    await dispatch(
      fetchCartAction(
        {
          ProductId: productId,
          AccessToken: localStorage.getItem("jwtToken"),
          ActionType: ActionTypeCart.Remove,
          IsCart: isCart,
        },
        "POST"
      )
    );
    await dispatch(fetchCartProducts(dataSend, "POST"));
  };

  const handleMarkPurchased = async (productId) => {
    await dispatch(
      fetchCartAction(
        {
          ProductId: productId,
          AccessToken: localStorage.getItem("jwtToken"),
          ActionType: ActionTypeCart.Purchase,
          IsCart: isCart,
        },
        "POST"
      )
    );
    await dispatch(fetchCartProducts(dataSend, "POST"));
  };

  const handleClickBtn = (cart) => {
    if (isCart !== cart) {
      setIsCart(cart, () => {});
    }
  };

  const emptyCartMsg = <h4 className="text-red fw-6">No items found!!!</h4>;

  return (
    <div className="cart-page">
      <div className="container">
        <div className="breadcrumb">
          <div className="breadcrumb-items flex">
            <button className="breadcrumb-item">
              <Link to="/">
                <i className="fas fa-home"></i>
                <span className="breadcrumb-separator">
                  <i className="fas fa-chevron-right"></i>
                </span>
              </Link>
            </button>
            {">"}
            <button
              className="breadcrumb-item"
              style={{
                backgroundColor: isCart ? "#0aea67" : "#dbd1d1",
              }}
              onClick={() => handleClickBtn(true)}
            >
              Added
            </button>
            {">"}
            <button
              className="breadcrumb-item"
              style={{
                backgroundColor: !isCart ? "#0aea67" : "#dbd1d1",
              }}
              onClick={() => handleClickBtn(false)}
            >
              Purchased
            </button>
          </div>
        </div>
      </div>
      <div className="bg-ghost-white py-5">
        <div className="container">
          <div className="section-title bg-ghost-white">
            <h3 className="text-uppercase fw-7 text-regal-blue ls-1">
              My Cart
            </h3>
            <CartFilter filter={filter} isCart={isCart} />
          </div>
          <div className="search-product">
            <input className="search-product-input" />
          </div>
          {products.products && products.products.length === 0 ? (
            emptyCartMsg
          ) : (
            <div className="cart-content grid">
              <div className="cart-left">
                <div className="cart-items grid">
                  {products.products &&
                    products.products.map((cartProduct) => (
                      <div
                        className="cart-item grid"
                        key={cartProduct.productId}
                      >
                        <div className="cart-item-img flex flex-column bg-white">
                          <img
                            src={cartProduct.images}
                            alt={cartProduct.title}
                          />
                          <button
                            type="button"
                            className="btn-square rmv-from-cart-btn"
                            onClick={() =>
                              handleRemoveFromCart(cartProduct.productId)
                            }
                          >
                            <span className="btn-square-icon">
                              <i className="fas fa-trash"></i>
                            </span>
                          </button>
                        </div>

                        <div className="cart-item-info">
                          <h6 className="fs-16 fw-5 text-light-blue">
                            {cartProduct.title}
                          </h6>
                          <div className="flex flex-between">
                            <div className="text-pine-green fw-4 fs-15 price">
                              Cost : {formatPrice(cartProduct.cost)}
                            </div>
                            <div className="sub-total fw-6 fs-18 text-regal-blue">
                              <span>Price: </span>
                              <span className="">
                                {formatPrice(cartProduct.price)}
                              </span>
                            </div>
                          </div>
                        </div>
                        <button
                          type="button"
                          className="btn-danger"
                          onClick={() =>
                            handleMarkPurchased(cartProduct.productId)
                          }
                        >
                          <span className="fs-16">Mark purchase</span>
                        </button>
                      </div>
                    ))}
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
      {products.products && products.products.length > 0 && (
        <Pagination
          type={isCart ? CART_ADDED_FILTER : CART_PURCHASED_FILTER}
          filter={products.filter}
          totalCount={products.totalCount}
          limitValues={[5, 10, 15]}
        />
      )}
    </div>
  );
};

export default CartPage;
