import React, { useEffect } from "react";
import "./CartPage.scss";
import { useSelector, useDispatch } from "react-redux";
import { Link } from "react-router-dom";
import { formatPrice } from "../../utils/helpers";
import { fetchCartProducts, fetchCartAction } from "../../store/cartSlice";
import {
  SET_CART_PRODUCTS,
  SET_PRODUCTS_ADDED,
  SET_TOTAL_ADDED,
  SET_PRODUCTS_PURCHASED,
  SET_TOTAL_PURCHASED,
  SET_REMOVE_ADDED,
  ActionTypeCart,
} from "../../utils/const";

const CartPage = () => {
  const dispatch = useDispatch();
  const { ProductsAdded: productsAdded } = useSelector((state) => state.cart);
  //   const { IsLoggedIn: isLoggedIn } = useSelector((state) => state.login);

  console.log("ProductsAdded in CartPage", productsAdded);

  useEffect(() => {
    dispatch(fetchCartProducts(localStorage.getItem("jwtToken"), "POST"));
  }, []);

  // const handleAddToCart = (product) => {
  //   dispatch(
  //     fetchCartAction(product, {
  //       ProductId: product.productId,
  //       AccessToken: localStorage.getItem("jwtToken"),
  //       ActionType: ActionTypeCart.Add,
  //     })
  //   );
  // };

  const handleRemoveFromCart = (product) => {
    dispatch(
      fetchCartAction(product, {
        ProductId: product.productId,
        AccessToken: localStorage.getItem("jwtToken"),
        ActionType: ActionTypeCart.Remove,
      }, "POST")
    );
  };

  const handleMarkPurchased = (product) => {
    dispatch(
      fetchCartAction(product, {
        ProductId: product.ProductId,
        AccessToken: localStorage.getItem("jwtToken"),
        ActionType: ActionTypeCart.P,
      })
    );
  };

  const emptyCartMsg = (
    <h4 className="text-red fw-6">No items found!!!</h4>
  );

  return (
    <div className="cart-page">
      <div className="container">
        <div className="breadcrumb">
          <ul className="breadcrumb-items flex">
            <li className="breadcrumb-item">
              <Link to="/">
                <i className="fas fa-home"></i>
                <span className="breadcrumb-separator">
                  <i className="fas fa-chevron-right"></i>
                </span>
              </Link>
            </li>
            <li>Cart</li>
          </ul>
        </div>
      </div>
      <div className="bg-ghost-white py-5">
        <div className="container">
          <div className="section-title bg-ghost-white">
            <h3 className="text-uppercase fw-7 text-regal-blue ls-1">
              My Cart
            </h3>
          </div>
          {(productsAdded && productsAdded.length) === 0 ? (
            emptyCartMsg
          ) : (
            <div className="cart-content grid">
              <div className="cart-left">
                <div className="cart-items grid">
                  {productsAdded &&
                    productsAdded.map((cartProduct) => (
                      <div className="cart-item grid" key={cartProduct.productId}>
                        <div className="cart-item-img flex flex-column bg-white">
                          <img
                            src={cartProduct.images}
                            alt={cartProduct.title}
                          />
                          <button
                            type="button"
                            className="btn-square rmv-from-cart-btn"
                            onClick={() => handleRemoveFromCart(cartProduct)}
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
                          {/* <div className="qty flex">
                          <span className="text-light-blue qty-text">
                            Qty:{" "}
                          </span>
                          <div className="qty-change flex">
                            <button
                              type="button"
                              className="qty-dec fs-14"
                              onClick={() =>
                                dispatch(
                                  toggleCartQty({
                                    id: cartProduct.id,
                                    type: "DEC",
                                  })
                                )
                              }
                            >
                              <i className="fas fa-minus text-light-blue"></i>
                            </button>
                            <span className="qty-value flex flex-center">
                              {cartProduct.quantity}
                            </span>
                            <button
                              type="button"
                              className="qty-inc fs-14 text-light-blue"
                              onClick={() =>
                                dispatch(
                                  toggleCartQty({
                                    id: cartProduct.id,
                                    type: "INC",
                                  })
                                )
                              }
                            >
                              <i className="fas fa-plus"></i>
                            </button>
                          </div>
                        </div> */}
                          <div className="flex flex-between">
                            <div className="text-pine-green fw-4 fs-15 price">
                              Price : {formatPrice(cartProduct.price)}.00
                            </div>
                            <div className="sub-total fw-6 fs-18 text-regal-blue">
                              <span>Sub Total: </span>
                              <span className="">
                                {formatPrice(cartProduct.totalPrice)}
                              </span>
                            </div>
                          </div>
                        </div>
                        <button
                          type="button"
                          className="btn-danger"
                          onClick={() => handleMarkPurchased(cartProduct)}
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
    </div>
  );
};

export default CartPage;
