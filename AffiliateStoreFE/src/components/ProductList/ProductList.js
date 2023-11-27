import React, { useState } from "react";
import { STATUS } from "../../utils/status";
import "./ProductList.scss";
import SingleProduct from "../SingleProduct/SingleProduct";
import { useSelector, useDispatch } from "react-redux";
import Loader from "../Loader/Loader";
import Error from "../Error/Error";
import ProductCard from "../ProductCard/ProductCard";
import Filter from "../Filter/Filter";
import Pagination from "../Pagination/Pagination";

const ProductList = ({
  data,
  status,
  name = "Our Products",
  hasFilter = true,
}) => {
  const { isModalVisible } = useSelector((state) => state.modal);

  if (status === STATUS.ERROR) return <Error />;
  if (status === STATUS.LOADING) return <Loader />;

  return (
    <section className="product py-5 bg-ghost-white" id="products">
      {isModalVisible && <SingleProduct />}
      <div className="container">
        <div className="product-content">
          <div className="section-title flex">
            <h3 className="text-uppercase fw-7 text-regal-blue ls-1">{name}</h3>
            {hasFilter && <Filter data={data} />}
          </div>
          <div className="product-items bg-white grid">
            {data.result.map((product) => (
              <ProductCard product={product} />
            ))}
          </div>
        </div>
      </div>
      <Pagination data={data} />
    </section>
  );
};

export default ProductList;
