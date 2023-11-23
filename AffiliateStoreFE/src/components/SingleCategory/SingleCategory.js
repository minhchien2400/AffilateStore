import React from "react";
import { useSelector, useDispatch } from "react-redux";
import { setIsModalVisible, setModalData } from "../../store/modalSlice";
import { formatPrice } from "../../utils/helpers";
import SingleProduct from "../SingleProduct/SingleProduct";
import Error from "../Error/Error";
import Loader from "../Loader/Loader";
import { STATUS } from "../../utils/status";

import ProductCard from "../ProductCard/ProductCard";

const SingleCategory = ({ products, status }) => {
  const { isModalVisible } = useSelector((state) => state.modal);

  if (status === STATUS.ERROR) return <Error />;
  if (status === STATUS.LOADING) return <Loader />;

  return (
    <section className="cat-single py-5 bg-ghost-white">
      {isModalVisible && <SingleProduct />}
      <div className="container">
        <div className="cat-single-content">
          <div className="section-title">
            <h3 className="text-uppercase fw-7 text-regal-blue ls-1">
              {products.categoryName}
            </h3>
          </div>
          <div className="product-items grid">
            {products.map((product) => (
              <ProductCard
                products={product}
              />
            ))}
          </div>
        </div>
      </div>
    </section>
  );
};

export default SingleCategory;
