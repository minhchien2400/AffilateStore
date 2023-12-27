import React from "react";
import { STATUS } from "../../utils/status";
import "./Category.scss";
import { Link } from "react-router-dom";
import Error from "../Error/Error";
import Loader from "../Loader/Loader";
import { setFilterAction } from "../../store/filterSlice";
import { useDispatch } from "react-redux";
import { SET_CATEGORY_FILTER as filterType } from "../../utils/const";
import CategoryFilter from "../Filter/CategoryFilter";
const Category = ({ data, status }) => {
  const dispatch = useDispatch();

  const showButton = data.totalCount > 1 ? true : false;

  // select limit items page
  const handleLimitChange = (event) => {
    const selectedValue = event.target.value;

    dispatch(
      setFilterAction(filterType, {
        Offset: 1,
        Limit: selectedValue,
        SearchText: data.filter.searchText,
        Keys: data.filter.keys,
      })
    );
  };

  //switch categories page
  const handlePrevPage = () => {
    // Xử lý khi người dùng nhấn vào nút "Previous Page"
    if (data.filter.offset > 1) {
      dispatch(
        setFilterAction(filterType, {
          Offset: data.filter.offset - 1,
          Limit: data.filter.limit,
          SearchText: data.filter.searchText,
          Keys: data.filter.keys,
        })
      );
    } else {
      dispatch(
        setFilterAction(filterType, {
          Offset: data.totalCount,
          Limit: data.filter.limit,
          SearchText: data.filter.searchText,
          Keys: data.filter.keys,
        })
      );
    }
  };

  const handleNextPage = () => {
    // Xử lý khi người dùng nhấn vào nút "Next Page"
    if (data.filter.offset < data.totalCount) {
      dispatch(
        setFilterAction(filterType, {
          Offset: data.filter.offset + 1,
          Limit: data.filter.limit,
          SearchText: data.filter.searchText,
          Keys: data.filter.keys,
        })
      );
    } else {
      dispatch(
        setFilterAction(filterType, {
          Offset: 1,
          Limit: data.filter.limit,
          SearchText: data.filter.searchText,
          Keys: data.filter.keys,
        })
      );
    }
  };

  const handleClickCategory = (id) => {
  }

  if (status === STATUS.ERROR) return <Error />;
  if (status === STATUS.LOADING) return <Loader />;

  return (
    <section className="categories py-5 bg-ghost-white" id="categories">
      <div className="container">
        <div className="categories-content">
          <div className="section-title flex">
            <h3 className="text-uppercase fw-7 text-regal-blue ls-1">
              Category
            </h3>
            <CategoryFilter filter={data.filter}/>
            <select
              className="limit-items"
              name="selectedNumber"
              value={data.filter.limit}
              onChange={handleLimitChange}
            >
              <option value="5">5</option>
              <option value="10">10</option>
              <option value="15">15</option>
              <option value="100">{"All"}</option>
            </select>
          </div>
          <div className="category-items grid">
            {showButton && (
              <button onClick={handlePrevPage} className="button-prev">
                <i class="fa-solid fa-angles-left"></i>
              </button>
            )}
            {data.result.map((category) => (
              <Link to={`category/${category.id}`} key={category.id} onClick={() => handleClickCategory(category.id)}>
                <div className="category-item">
                  <div className="category-item-img">
                    <img src={category.image} alt="" />
                  </div>
                  <div className="category-item-name text-center">
                    <h6 className="fs-20">{category.name}</h6>
                  </div>
                </div>
              </Link>
            ))}
            {showButton && (
              <button onClick={handleNextPage} className="button-next">
                <i class="fa-solid fa-angles-right"></i>
              </button>
            )}
          </div>
        </div>
      </div>
    </section>
  );
};

export default Category;
