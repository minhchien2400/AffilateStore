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
  const { topSale: topSale, topSaleStatus: topSaleStatus } =
    useSelector((state) => state.product);

  const { data: dataFilter } = useSelector((state) => state.filter);
  console.log("dataFilter", dataFilter);


  // filter state
  const [pagination, setPagination] = useState({
    Offset: 1,
    Limit: 10,
    SearchText: "",
    Keys: ["all", "all"],
  });
  
  useEffect(() => {
    setPagination((prevPagination) => ({
      ...prevPagination,
      Offset: dataFilter.Offset,
      Limit: dataFilter.Limit,
      Keys: dataFilter.Keys,
    }));
  }, [dataFilter]);
  
  useEffect(() => {
    dispatch(fetchTopSale(pagination, "POST"));
  }, [pagination]);

  return (
    <div className="home-page">
      {/* <Slider /> */}
      {/* <Category categories={categories} status={categoryStatus} /> */}
      {/* <ProductList products={topSale.result} status={topSaleStatus} /> */}
      {topSale.result && <ProductList data={topSale} status={topSaleStatus} name="Top sale"/>}
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
