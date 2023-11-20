import React, { useEffect, useState } from "react";
import ProductList from "../../components/ProductList/ProductList";
import { useSelector, useDispatch } from "react-redux";
import {
  fetchProductsByCategory,
  fetchProductsByCategoryId,
} from "../../store/categorySlice";
import { useParams, Link } from "react-router-dom";
import "./CategoryPage.scss";

const CategoryPage = () => {
  const dispatch = useDispatch();
  const { id } = useParams();
  const { catProduct: products, catProductStatus: status } =
    useSelector((state) => state.category);
  const [filter, setFilter] = useState({
    categoryId: id,
    Offset: 1,
    Limit: 6,
    SearchText: "",
  });

  useEffect(() => {
    dispatch(fetchProductsByCategoryId(filter, 'POST'));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  return (
    <div className="category-page">
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
            <li>
              Category
              <span className="breadcrumb-separator">
                <i className="fas fa-chevron-right"></i>
              </span>
            </li>
            <li>{products[0] && products[0].categoryName}</li>
          </ul>
        </div>
      </div>
      <ProductList
        products={products}
        status={status}
        name={`${products[0].categoryName}`}
      />
    </div>
  );
};

export default CategoryPage;
