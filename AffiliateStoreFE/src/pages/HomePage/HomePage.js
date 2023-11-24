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
import Pagination from "../../components/Pagination/Pagination";

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

    const { data: filter } =
    useSelector((state) => state.filter);

    // filter state
  const [filterProducts, SetFilterProducts] = useState({
    Offset: 1,
    Limit: 10,
    SearchText: "",
  });
  const [filterCategories, SetFilterCategories] = useState({
    Offset: 1,
    Limit: 10,
    SearchText: "",
    Keys: [],
  });

  const [filterSingleCategory1, SetFilterSingleCategory1] = useState({
    CategoryName: "Electronics",
    Offset: 1,
    Limit: 10,
    SearchText: "",
    Keys: [],
  });

  const [filterSingleCategory2, SetFilterSingleCategory2] = useState({
    CategoryName: "Pets",
    Offset: 1,
    Limit: 10,
    SearchText: "",
    Keys: [],
  });

  // pagination state
  const [paginationCategories, setPaginationCategories] = useState({
    
  });

  const SetFilterDefault = (filter) => {

  }

  useEffect(() => {
    dispatch(fetchCategories(filterCategories, "POST"));
    dispatch(fetchProducts(filterProducts, "POST"));
    dispatch(fetchProductsByCategory("Electronics"));
    dispatch(fetchProductsByCategory("Pets"));
    //eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    dispatch(fetchProducts(filterProducts, "POST"));
    dispatch(fetchProductsByCategory("Electronics"));
    dispatch(fetchProductsByCategory("Pets"));
    //eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filter]);

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
      <Pagination />
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
