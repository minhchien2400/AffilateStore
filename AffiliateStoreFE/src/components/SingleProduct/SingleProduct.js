import React, { useState } from "react";
import "./SingleProduct.scss";
import { useSelector, useDispatch } from "react-redux";
import { setIsModalVisible } from "../../store/modalSlice";
import { addToCart } from "../../store/cartSlice";
import { useNavigate } from "react-router-dom";
import { formatPrice } from "../../utils/helpers";

const SingleProduct = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const [qty, setQty] = useState(1);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

  const { data: product } = useSelector((state) => state.modal);

  const increaseQty = () => {
    setQty((prevQty) => prevQty + 1);
  };

  const decreaseQty = () => {
    setQty((prevQty) => Math.max(prevQty - 1, 1));
  };

  const nextImage = () => {
    setCurrentImageIndex((prevIndex) =>
      prevIndex === product.images.length - 1 ? 0 : prevIndex + 1
    );
  };

  const prevImage = () => {
    setCurrentImageIndex((prevIndex) =>
      prevIndex === 0 ? product.images.length - 1 : prevIndex - 1
    );
  };

  const addToCartHandler = () => {
    let totalPrice = qty * product.price;
    const tempProduct = {
      ...product,
      quantity: qty,
      totalPrice,
    };
    dispatch(addToCart(tempProduct));
    dispatch(setIsModalVisible(false));
    navigate("/cart");
  };

  const modalOverlayHandler = (e) => {
    if (e.target.classList.contains("overlay-bg")) {
      dispatch(setIsModalVisible(false));
    }
  };

  const selectImage = (index) => {
    setCurrentImageIndex(index);
  };


  return (
    <div className="overlay-bg" onClick={modalOverlayHandler}>
      <div className="product-details-modal bg-white">
        <button
          type="button"
          className="modal-close-btn flex flex-center fs-14"
          onClick={() => dispatch(setIsModalVisible(false))}
        >
          <i className="fas fa-times"></i>
        </button>
        <div className="details-content grid">
          {/* details left */}
          <div className="details-right">
            <div className="details-img">
              <img src={product.images[currentImageIndex]} alt={product.name} />
              {product.images.length > 1 && (
                <div className="image-navigation">
                  <button
                    type="button"
                    className="prev-image"
                    onClick={prevImage}
                  >
                    &lt;
                  </button>
                  <button
                    type="button"
                    className="next-image"
                    onClick={nextImage}
                  >
                    &gt;
                  </button>
                </div>
              )}
            </div>

            <div className="details-imgs-bottom">
              {product.images.map((img, index) => (
                  <img
                  key={index}
                  src={img}
                  alt={product.name}
                  className={index === currentImageIndex ? 'selected' : ''}
                  onClick={() => selectImage(index)}
                />
              ))}
            </div>
          </div>
          {/* details right */}
          <div className="details-left">
            <div className="details-info">
              <h3 className="title text-regal-blue fs-22 fw-5">
                {product.name}
              </h3>
              <p className="description text-pine-green">
                {product.description}
              </p>
              <div className="price fw-7 fs-24">
                Giá: {formatPrice(product.price)}
              </div>
              <div className="qty flex">
                <span className="text-light-blue qty-text">Số lượng: </span>
                <div className="qty-change flex">
                  <button
                    type="button"
                    className="qty-dec fs-14"
                    onClick={decreaseQty}
                  >
                    <i className="fas fa-minus text-light-blue"></i>
                  </button>
                  <span className="qty-value flex flex-center">{qty}</span>
                  <button
                    type="button"
                    className="qty-inc fs-14 text-light-blue"
                    onClick={increaseQty}
                  >
                    <i className="fas fa-plus"></i>
                  </button>
                </div>
              </div>
              <button
                type="button"
                className="btn-primary add-to-cart-btn"
                onClick={addToCartHandler}
              >
                <span className="btn-icon">
                  <i className="fas fa-cart-shopping"></i>
                </span>
                <span className="btn-text">Thêm vào giỏ hàng</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SingleProduct;
