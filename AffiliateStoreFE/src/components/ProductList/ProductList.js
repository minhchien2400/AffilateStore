import React from 'react';
import { STATUS } from '../../utils/status';
import "./ProductList.scss";
import { setModalData, setIsModalVisible } from '../../store/modalSlice';
import SingleProduct from '../SingleProduct/SingleProduct';
import { useSelector, useDispatch } from 'react-redux';
import { formatPrice } from '../../utils/helpers';
import Loader from '../Loader/Loader';
import Error from '../Error/Error';
import ProductCard from '../ProductCard/ProductCard';

const ProductList = ({products, status, name = "Our Products"}) => {
    const dispatch = useDispatch();
    const {isModalVisible} = useSelector((state) => state.modal);

    const viewModalHandler = (data) => {
        dispatch(setModalData(data));
        dispatch(setIsModalVisible(true));
    }

    if(status === STATUS.ERROR) return (<Error />);
    if(status === STATUS.LOADING) return (<Loader />);

    return (
        <section className='product py-5 bg-ghost-white' id = "products">
            { isModalVisible && <SingleProduct />}

            <div className='container'>
                <div className='product-content'>
                    <div className='section-title'>
                        <h3 className='text-uppercase fw-7 text-regal-blue ls-1'>{name}</h3>
                    </div>
                    <ProductCard products={products}/>
                </div>
            </div>
        </section>
    )
}

export default ProductList