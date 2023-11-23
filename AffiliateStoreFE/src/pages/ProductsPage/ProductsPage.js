import React, {useEffect, useState} from 'react';
import ProductList from '../../components/ProductList/ProductList';
import { useSelector, useDispatch } from 'react-redux';
import { fetchProducts } from '../../store/productSlice';
import { fetchSearchProducts } from '../../store/searchSlice';
import { useParams, Link } from "react-router-dom";

const ProductsPage = () => {
  const { searchText } = useParams();
  const {data: products, status: productStatus} = useSelector((state) => state.search);
  const [filterProducts, SetFilterProducts] = useState({
    Offset: 1,
    Limit: 6,
    SearchText:searchText,
  });
  useEffect(() => {
    //eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchText]);


  return (
    <div className = "home-page">
      <ProductList products = {products} status = {productStatus}/>
    </div>
  )
}

export default ProductsPage;