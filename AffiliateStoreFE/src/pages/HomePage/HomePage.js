import React, { useEffect, useState } from "react";
import Slider from "../../components/Slider/Slider";
import Category from "../../components/Category/Category";
import ProductList from "../../components/ProductList/ProductList";
import SingleCategory from "../../components/SingleCategory/SingleCategory";
import { useSelector, useDispatch } from "react-redux";
import { fetchProducts, fetchTopSale } from "../../store/productSlice";
import {
  fetchCategories,
  fetchProductsByCategory,
} from "../../store/categorySlice";
import "./HomePage.scss";
import Pagination from "../../components/Pagination/Pagination";

const HomePage = () => {
  const dispatch = useDispatch();
  // const { data: categories, status: categoryStatus } = useSelector(
  //   (state) => state.category
  // );
  // const { data: products, status: productStatus } = useSelector(
  //   (state) => state.product
  // );
  const { topSale: topSale, topSaleStatus: topSaleStatus } =
    useSelector((state) => state.product);

  // const { data: filter } = useSelector((state) => state.filter);

  const { data: pagination } = useSelector((state) => state.pagination);

  // filter state
  const [filterProducts, SetFilterProducts] = useState({
    Offset: pagination.Offset,
    Limit: pagination.Limit,
    SearchText: "",
    Keys: [],
  });
  // const [filterCategories, SetFilterCategories] = useState({
  //   Offset: 1,
  //   Limit: 10,
  //   SearchText: "",
  //   Keys: [],
  // });

  // const [filterSingleCategory1, SetFilterSingleCategory1] = useState({
  //   CategoryName: "Electronics",
  //   Offset: 1,
  //   Limit: 10,
  //   SearchText: "",
  //   Keys: [],
  // });

  // const [filterSingleCategory2, SetFilterSingleCategory2] = useState({
  //   CategoryName: "Pets",
  //   Offset: 1,
  //   Limit: 10,
  //   SearchText: "",
  //   Keys: [],
  // });

  // // pagination state
  // const [paginationCategories, setPaginationCategories] = useState({});

  // const SetFilterDefault = (filter) => {};

  useEffect(() => {
    // dispatch(fetchCategories(filterCategories, "POST"));
    dispatch(fetchTopSale(filterProducts, "POST"));
    // dispatch(fetchProductsByCategory("Electronics"));
    // dispatch(fetchProductsByCategory("Pets"));
    //eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // useEffect(() => {
  //   dispatch(fetchTopSale(filterProducts, "POST"));
  //   // dispatch(fetchProductsByCategory("Electronics"));
  //   // dispatch(fetchProductsByCategory("Pets"));
  // }, [pagination]);

  // const handleSingleCategoryFilter1 = () => {
  //   SetFilterSingleCategory1({
  //     CategoryName: "Electronics",
  //     Offset: 1,
  //     Limit: 6,
  //     SearchText: "",
  //     Keys: [],
  //   });
  // };

  return (
    <div className="home-page">
      <Slider />
      {/* <Category categories={categories} status={categoryStatus} /> */}
      {/* <ProductList products={topSale.result} status={topSaleStatus} /> */}
      {topSale.result && <ProductList products={topSale.result} status={topSaleStatus} />}
      <Pagination totalCount = {topSale.totalCount}/>
      
      {/* <section>
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
      </section> */}
    </div>
  );
};

export default HomePage;
