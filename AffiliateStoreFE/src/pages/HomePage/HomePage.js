import React, { useEffect, useState } from "react";
import Slider from "../../components/Slider/Slider";
import Category from "../../components/Category/Category";
import ProductList from "../../components/ProductList/ProductList";
import { useSelector, useDispatch } from "react-redux";
import { fetchProducts, fetchTopSale } from "../../store/productSlice";
import { fetchCategories } from "../../store/categorySlice";
import { setPageState } from "../../store/pageSlice";
import { setOrderFilter } from "../../store/filterSlice";
import "./HomePage.scss";

const HomePage = () => {
  const dispatch = useDispatch();

  const { data: categoriesData, status: categoryStatus } = useSelector(
    (state) => state.category
  );

  const { topSale: topSale, topSaleStatus: topSaleStatus } = useSelector(
    (state) => state.product
  );

  const { data: dataFilter, categoryData: categoryFilter } = useSelector(
    (state) => state.filter
  );

  useEffect(() => {
    dispatch(setPageState({
      IsHomePage: true,
    }));
    dispatch(setOrderFilter({
      Offset: 1,
      Limit: 10,
      SearchText: "",
      Keys: ["all", "all"],
    }))
  }, [])

  useEffect(() => {
    dispatch(fetchTopSale(dataFilter, "POST"));
  }, [dataFilter]);

  useEffect(() => {
    dispatch(fetchCategories(categoryFilter, "POST"));
  }, [categoryFilter]);

  return (
    <div className="home-page">
      {/* <Slider /> */}
      {categoriesData.result && (
        <Category data={categoriesData} status={categoryStatus} />
      )}
      {topSale.result && (
        <ProductList data={topSale} status={topSaleStatus} name="Top sale" />
      )}
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
