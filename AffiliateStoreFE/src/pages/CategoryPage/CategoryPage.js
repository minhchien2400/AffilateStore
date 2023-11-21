import React, { useEffect, useState } from "react";
import ProductList from "../../components/ProductList/ProductList";
import { useSelector, useDispatch } from "react-redux";
import {
  fetchProductsByCategoryId,
} from "../../store/categorySlice";
import { useParams, Link } from "react-router-dom";
import "./CategoryPage.scss";

const CategoryPage = () => {
  const dispatch = useDispatch();
  const { id } = useParams();
  const { catProductAll: products, catProductStatus: status } =
    useSelector((state) => state.category);
  const [filterModel, setFilterModel] = useState({
    CategoryId: id,
    Offset: 1,
    Limit: 6,
    SearchText: "",
  });

  useEffect(() => {
    console.log('1');
    setFilterModel((prevFilterCategories) => ({
      ...prevFilterCategories, // Giữ nguyên các giá trị còn lại
      CategoryId: id, // Thay đổi giá trị SearchText
    }));
  }, [id]);

  useEffect(() => {
    console.log('tao');
    dispatch(fetchProductsByCategoryId(filterModel, 'POST'));
  }, [])
  useEffect(() => {
    console.log('3');
    dispatch(fetchProductsByCategoryId(filterModel, 'POST'));
  }, [filterModel])

  console.log(products);

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
            <li>{products && products.length > 0 && products[0].categoryName}</li>
          </ul>
        </div>
      </div>
      <ProductList
        products={products}
        status={status}
        name= {products && products.length > 0 && products[0].categoryName}
      />
    </div>
  );
};

export default CategoryPage;
