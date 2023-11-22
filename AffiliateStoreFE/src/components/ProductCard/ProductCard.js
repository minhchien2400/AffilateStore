import { setIsModalVisible, setModalData } from "../../store/modalSlice";
import { formatPrice } from "../../utils/helpers";
import { useDispatch } from "react-redux";
import "./ProductCard.scss";
import { formatStars } from "../../utils/helpers";
import { Link } from "react-router-dom";

const ProductCard = (props) => {
  const dispatch = useDispatch();
  const viewModalHandler = (data) => {
    dispatch(setModalData(data));
    dispatch(setIsModalVisible(true));
  };

  return (
    <div className="product-items grid">
      {props.products.map((product) => (
        <div
          className="product-item bg-white"
          key={product.id}
        >
          <div className="product-item-img">
            <img src={product.images[0]} alt="" onClick={() => viewModalHandler(product)}/>
            <div className="product-item-cat text-white fs-13 text-uppercase bg-gold fw-6">
              {product.categoryName}
            </div>
          </div>
          <div className="product-item-body">
            <h6 className="product-item-title text-pine-green fw-4 fs-15">
              {product.productName}
            </h6>
            <div className="product-item-details">
              <span className="product-item-details-right">
                <div className="product-item-price text-regal-blue fw-7 fs-10">
                  {formatPrice(product.cost)}
                </div>
                {`-${100-Math.floor((product.price/product.cost)*100)}%`}
                <div className="product-item-price-sale text-regal-blue fw-7 fs-18">
                  {formatPrice(product.price)}
                </div>
                <div className="product-item-price-sale text-pine-green fw-5 fs-10">
                  Da ban:{product.totalSales <= 1000 ? product.totalSales : `${product.totalSales/1000}k`}
                </div>
              </span>
              <span className="product-item-details-left">
                <button>Xem chi tiet</button>
                <a href={product.Link}>Link mua hang</a>
                <div className="product-item-stars">
                  {formatStars(product.stars).map((star, index) => (
                    <img src={star} alt="" key={index} />
                  ))}
                  {product.stars % 10 !== 0
                    ? `(${product.stars / 10})`
                    : `(${product.stars / 10}.0)`}
                </div>
              </span>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default ProductCard;
