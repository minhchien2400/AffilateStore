import { setIsModalVisible, setModalData } from "../../store/modalSlice";
import { formatPrice } from "../../utils/helpers";
import { useDispatch } from "react-redux";
import "./ProductCard.scss";
import { formatStars } from "../../utils/helpers";
import { Link } from "react-router-dom";

const ProductCard = (props) => {
  const dispatch = useDispatch();
  const viewModalHandler = (product) => {
    dispatch(setModalData(product));
    dispatch(setIsModalVisible(true));
  };

  const handleClickDetail = () => {

  }

  return (
    <div className="product-item">
      <div className="product-item-img">
        <img
          src={props.product.images[0]}
          alt=""
          onClick={() => viewModalHandler(props.product)}
        />
        <div className="product-item-sale text-white fs-13 text-uppercase fw-6">
          {`-${
              100 - Math.floor((props.product.price / props.product.cost) * 100)
            }%`}
        </div>
        <div className="product-item-cat text-white fs-13 text-uppercase bg-gold fw-6">
          {props.product.categoryName}
        </div>
      </div>
      <Link to={`product/${props.product.productId}`} key={props.product.productId}>
      <div className="product-item-body" onClick={() => handleClickDetail(props.product.productId)}>
        <h6 className="product-item-title text-pine-green fw-4 fs-15">
          {props.product.productName}
        </h6>
        <div className="product-item-details">
          <span className="product-item-details-right">
            <div className="product-item-price text-regal-blue fw-7 fs-10">
              {formatPrice(props.product.cost)}
            </div>
            <div className="product-item-price-sale text-regal-blue fw-7 fs-18">
              {formatPrice(props.product.price)}
            </div>
            <div className="product-item-price-sale text-pine-green fw-5 fs-10">
              Da ban:
              {props.product.totalSales <= 1000
                ? props.product.totalSales
                : `${props.product.totalSales / 1000}k`}
            </div>
          </span>
          <span className="product-item-details-left">
            <button>Xem chi tiet</button>
            <a href={props.product.Link}>Link mua hang</a>
            <div className="product-item-stars">
              {formatStars(props.product.stars).map((star, index) => (
                <img src={star} alt="" key={index} />
              ))}
              {props.product.stars % 10 !== 0
                ? `(${props.product.stars / 10})`
                : `(${props.product.stars / 10}.0)`}
            </div>
          </span>
        </div>
      </div>
      </Link>
    </div>
  );
};

export default ProductCard;
