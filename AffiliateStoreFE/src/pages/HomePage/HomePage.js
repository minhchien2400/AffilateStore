import React, { useEffect, useState } from "react";
import Slider from "../../components/Slider/Slider";
import Category from "../../components/Category/Category";
import ProductList from "../../components/ProductList/ProductList";
import SingleCategory from "../../components/SingleCategory/SingleCategory";
import { useSelector, useDispatch } from "react-redux";
import { fetchProducts } from "../../store/productSlice";
import {
  fetchCategories,
  fetchProductsByCategory,
} from "../../store/categorySlice";
import "./HomePage.scss";

const HomePage = () => {
  const dispatch = useDispatch();
  const { data: categories, status: categoryStatus } = useSelector(
    (state) => state.category
  );
  const { data: products, status: productStatus } = useSelector(
    (state) => state.product
  );
  const { catProductAll: productsByCategory, catProductAllStatus } =
    useSelector((state) => state.category);
  const [filterProducts, SetFilterProducts] = useState({
    Offset: 1,
    Limit: 6,
    SearchText: "",
  });
  const [filterCategories, SetFilterCategories] = useState({
    Offset: 1,
    Limit: 6,
    SearchText: "",
    Keys: [],
  });

  const [filterSingleCategory1, SetFilterSingleCategory1] = useState({
    CategoryName: "Electronics",
    Offset: 1,
    Limit: 6,
    SearchText: "",
    Keys: [],
  });

  const [filterSingleCategory2, SetFilterSingleCategory2] = useState({
    CategoryName: "Pets",
    Offset: 1,
    Limit: 6,
    SearchText: "",
    Keys: [],
  });
  useEffect(() => {
    dispatch(fetchProducts(filterProducts, "POST"));
    dispatch(fetchCategories(filterCategories, "POST"));
    dispatch(fetchProductsByCategory("Electronics"));
    dispatch(fetchProductsByCategory("Pets"));
    //eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSingleCategoryFilter1 = () => {
    SetFilterSingleCategory1({
      CategoryName: "Electronics",
      Offset: 1,
      Limit: 6,
      SearchText: "",
      Keys: [],
    })
  }

  return (
    <div className="home-page">
      
      <Slider />
      <Category categories={categories} status={categoryStatus} />
      <ProductList products={products} status={productStatus} />
      <section>
        {productsByCategory[0] && (
          <SingleCategory
            products={productsByCategory[0]}
            status={catProductAllStatus}
          />
        )}
      </section>
      <section>
        {productsByCategory[1] && (
          <SingleCategory
            products={productsByCategory[1]}
            status={catProductAllStatus}
          />
        )}
      </section>
    </div>
  );
};

export default HomePage;
