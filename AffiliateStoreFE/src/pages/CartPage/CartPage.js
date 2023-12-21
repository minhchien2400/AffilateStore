import React, { useEffect } from "react";
import "./CartPage.scss";
import { useSelector, useDispatch } from "react-redux";
import { Link } from "react-router-dom";
import { formatPrice } from "../../utils/helpers";
import { fetchCartProducts, fetchCartAction } from "../../store/cartSlice";
import Pagination from "../../components/Pagination/Pagination";
import {
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

  const handleRemoveFromCart = (productId) => {
    dispatch(
      fetchCartAction({
        ProductId: productId,
        AccessToken: localStorage.getItem("jwtToken"),
        ActionType: ActionTypeCart.Remove,
      }, "POST")
    );
  };

  const handleMarkPurchased = (product) => {
    dispatch(
      fetchCartAction(product, {
        ProductId: product.productId,
        AccessToken: localStorage.getItem("jwtToken"),
        ActionType: ActionTypeCart.Purchase,
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
          <div className="search-product">
            <input className="search-product-input"/>
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
                            onClick={() => handleRemoveFromCart(cartProduct.productId)}
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
                          onClick={() => handleMarkPurchased(cartProduct.productId)}
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
      <Pagination data={productsAdded}/>
    </div>
  );
};

export default CartPage;
