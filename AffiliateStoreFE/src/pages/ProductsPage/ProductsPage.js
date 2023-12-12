import React, {useEffect, useState} from 'react';
import ProductList from '../../components/ProductList/ProductList';
import { useSelector, useDispatch } from 'react-redux';
import { fetchProducts } from '../../store/productSlice';
import { useParams } from 'react-router-dom';
import { setPageState } from '../../store/pageSlice';
import { setOrderFilter } from '../../store/filterSlice';

const ProductsPage = () => {
  const dispatch = useDispatch();
  const { id } = useParams();
  const {data: data, status: productStatus} = useSelector((state) => state.product);
  const { data: dataFilter } = useSelector((state) => state.filter);

  const {IsLoggedIn: isLoggedIn} = useSelector(state => state.login);

  

  useEffect(() => {
    dispatch(setPageState({
      IsProductsPage: true,
    }));
    // dispatch(setOrderFilter({
    //   Offset: 1,
    //   Limit: 10,
    //   SearchText: "",
    //   Keys: ["all", "all"],
    // }))
  }, [])


  useEffect(() => {
    dispatch(fetchProducts({
      Offset: dataFilter.Offset,
      Limit: dataFilter.Limit,
      SearchText: dataFilter.SearchText,
      Keys: dataFilter.Keys,
    }, "POST"));
  }, [dataFilter]);


  return (
    <div className = "home-page">
      {data.result && <ProductList data = {data} status = {productStatus}/>}
    </div>
  )
}

export default ProductsPage;