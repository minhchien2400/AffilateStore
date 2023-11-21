import React, {useEffect, useState} from 'react';
import ProductList from '../../components/ProductList/ProductList';
import { useSelector, useDispatch } from 'react-redux';
import { fetchProducts } from '../../store/productSlice';
import { fetchSearchProducts } from '../../store/searchSlice';

const ProductsPage = () => {
  const dispatch = useDispatch();
  const {data: products, status: productStatus} = useSelector((state) => state.product);
  const [filterProducts, SetFilterProducts] = useState({
    Offset: 1,
    Limit: 6,
    SearchText:'',
  });
  useEffect(() => {
    dispatch(fetchSearchProducts(filterProducts, 'POST'));
    //eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);


  return (
    <div className = "home-page">
      <ProductList products = {products} status = {productStatus}/>
    </div>
  )
}

export default ProductsPage;