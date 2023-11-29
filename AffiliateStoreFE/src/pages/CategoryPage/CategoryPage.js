import React, { useEffect, useState } from "react";
import ProductList from "../../components/ProductList/ProductList";
import { useSelector, useDispatch } from "react-redux";
import { useParams, Link } from "react-router-dom";
import { fetchProductsByCategoryId } from "../../store/categorySlice";
import "./CategoryPage.scss";

const CategoryPage = () => {
  const dispatch = useDispatch();
  const { id } = useParams();
  const { productsCategory: data, productsCategoryStatus: status } =
    useSelector((state) => state.category);
    console.log("data at category page:", data);

  const { data: dataFilter } = useSelector((state) => state.filter);

  // useEffect(() => {
  //   dispatch(fetchProducts(filterModel, 'POST'));
  // }, [])
  useEffect(() => {
    dispatch(fetchProductsByCategoryId({
      Offset: dataFilter.Offset,
      Limit: dataFilter.Limit,
      SearchText: dataFilter.SearchText !== null ? dataFilter.SearchText : "",
      Keys: dataFilter.Keys !== null ? dataFilter.Keys : ["all", "all"],
      CategoryId: id
    }, "POST"));
  }, [dataFilter, id]);


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
            <li>
              {data.result && data.result[0].categoryName}
            </li>
          </ul>
        </div>
      </div>
      {data.result && data.filter.keys && <ProductList
        data={data}
        status={status}
        name={data.result[0].categoryName}
      />}
    </div>
  );
};

export default CategoryPage;
