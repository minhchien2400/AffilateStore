import React, { useEffect, useState } from "react";
import "./ProductDetailPage.scss";
import { useSelector, useDispatch } from "react-redux";
import { setIsModalVisible } from "../../store/modalSlice";
import { addToCart } from "../../store/cartSlice";
import { useNavigate } from "react-router-dom";
import { formatPrice, formatStars } from "../../utils/helpers";
import { fetchProduct } from "../../store/productSlice";
import Loader from "../../components/Loader/Loader";
import { useParams } from "react-router-dom";

const ProductDetailPage = () => {
  const { id } = useParams();
  console.log("productId o product detail page la", id);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const [qty, setQty] = useState(1);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

  const { productData: product } = useSelector((state) => state.product);
  console.log("product o day la:", product);

  useEffect(() => {
    console.log("co useEffect");
    dispatch(fetchProduct(id));
  }, [id]);

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

    if (product.length === 0) return <Loader />;

  return (
    <div className="container">
      <div className="details-top grid">
        <div className="details-left">
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
                className={index === currentImageIndex ? "selected" : ""}
                onClick={() => selectImage(index)}
              />
            ))}
          </div>
        </div>
        {/* details right */}
        <div className="details-right">
          <h3 className="title text-regal-blue fs-22 fw-5">
            {product.productName}
          </h3>
          <div className="details-info flex">
            <div className="details-info-left">
              <div className="price-detail">
                <span className="price-sale">{`-${
                  100 - Math.floor((product.price / product.cost) * 100)
                }%`}</span>
                <span className="price-cost fw-7 fs-24">
                  {formatPrice(product.cost)}
                </span>
                <span className="price-price fw-7 fs-24">
                  {formatPrice(product.price)}
                </span>
              </div>
              <div className="sold-stars-detail flex text-pine-green fw-5 fs-10">
                <div className="sold-detail">
                  Sold:{" "}
                  {product.totalSales <= 1000
                    ? product.totalSales
                    : `${product.totalSales / 1000}k`}
                </div>
                <div className="stars-detail">
                  {formatStars(product.stars).map((star, index) => (
                    <img src={star} alt="" key={index} />
                  ))}
                  {product.stars % 10 !== 0
                    ? `(${product.stars / 10})`
                    : `(${product.stars / 10}.0)`}
                </div>
              </div>
              <button className="btn-primary">Xem chi tiet</button>
            </div>
            <div className="details-info-right">
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
              <div className="detail-button-sold">
                <button className="btn-primary">Link mua hang</button>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className="details-bottom">
        
      </div>
    </div>
  );
};

export default ProductDetailPage;
