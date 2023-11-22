import { setIsModalVisible, setModalData } from "../../store/modalSlice";
import { formatPrice } from "../../utils/helpers";
import { useDispatch } from "react-redux";

import { formatStars } from "../../utils/helpers";

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
          onClick={() => viewModalHandler(product)}
        >
          <div className="product-item-img">
            <img src={product.images[0]} alt="" />
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
                <div className="product-item-price-sale text-regal-blue fw-7 fs-18">
                  {formatPrice(product.price)}
                </div>
                <div className="product-item-price-sale text-pine-green fw-5 fs-10">
                  Da ban: {product.totalSales}
                </div>
              </span>
              <span className="product-item-details-left">
                <div className="product-item-stars text-regal-blue fw-7 fs-10">
                  {formatStars(product.stars).map((star, index) => (
                    <img src={star} alt="" key={index}></img>
                  ))}
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
