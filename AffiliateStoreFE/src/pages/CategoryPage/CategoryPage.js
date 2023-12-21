import React, { useEffect, useState } from "react";
import ProductList from "../../components/ProductList/ProductList";
import { useSelector, useDispatch } from "react-redux";
import { useParams, Link } from "react-router-dom";
import { fetchProductsByCategoryId } from "../../store/categorySlice";
import { setPageState } from "../../store/pageSlice";
import { setFilterAction } from "../../store/filterSlice";
import { SET_PRODUCTS_FILTER as type} from "../../utils/const";
import "./CategoryPage.scss";

const CategoryPage = () => {
  const dispatch = useDispatch();
  const { id } = useParams();
  const { productsCategory: data, productsCategoryStatus: status } =
    useSelector((state) => state.category);

  const { data: dataFilter } = useSelector((state) => state.filter);
  const {IsLoggedIn: isLoggedIn} = useSelector(state => state.login);

  useEffect(() => {
    dispatch(setPageState({
      IsCategoryPage: true,
    }));
    dispatch(setFilterAction(type, {
      Offset: 1,
      Limit: 10,
      SearchText: "",
      Keys: ["all", "all"],
    }))
  }, [])

  useEffect(() => {
    dispatch(fetchProductsByCategoryId({
      Offset: dataFilter.Offset,
      Limit: dataFilter.Limit,
      SearchText: dataFilter.SearchText,
      Keys: dataFilter.Keys,
      CategoryId: id
    }, "POST"));
  }, []);

  useEffect(() => {
    dispatch(fetchProductsByCategoryId({
      Offset: dataFilter.Offset,
      Limit: dataFilter.Limit,
      SearchText: dataFilter.SearchText,
      Keys: dataFilter.Keys,
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
              {data.result && data.result.length > 0 && data.result[0].categoryName}
            </li>
          </ul>
        </div>
      </div>
      {data.result && data.result.length > 0 && <ProductList
        data={data}
        status={status}
        name={data.result[0].categoryName}
      />}
    </div>
  );
};

export default CategoryPage;
