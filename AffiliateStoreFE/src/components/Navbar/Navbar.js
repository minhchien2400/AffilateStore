import React, { useState, useEffect, useCallback } from "react";
import "./Navbar.scss";
import { Link } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { getCartTotal } from "../../store/cartSlice";
import { fetchCategories } from "../../store/categorySlice";
import {  } from "../../store/searchSlice";
import { setOrderFilter } from "../../store/filterSlice";

const Navbar = () => {
  const dispatch = useDispatch();
  const { data: categoriesData } = useSelector((state) => state.category);

  const {data: dataFilter} = useSelector((state) => state.filter)

  const { totalItems } = useSelector((state) => state.cart);

  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const [searchText, setSearchText] = useState("");
  const [filterCategories, setFilterCategories] = useState({
    Offset: 1,
    Limit: 6,
    SearchText: searchText,
    Keys: ["all"]
  });

  // useEffect(() => {
  //   dispatch(fetchCategories(filterCategories, 'POST'));
  //   dispatch(getCartTotal());
  //   // eslint-disable-next-line react-hooks/exhaustive-deps
  // }, []);

  const handleSearch = (newSearchText) => {
    setFilterCategories((prevFilterCategories) => ({
      ...prevFilterCategories, // Giữ nguyên các giá trị còn lại
      SearchText: newSearchText, // Thay đổi giá trị SearchText
    }));
    dispatch(setOrderFilter({
      Offset: 1,
      Limit: 10,
      SearchText: "",
      Keys: ["all", "all"],
    },))
    setSearchText('');
  };


  return (
    <nav className="navbar">
      <div className="navbar-content">
        <div className="container">
          <div className="navbar-top flex flex-between">
            <Link to="/" className="navbar-brand">
              <span className="text-regal-blue">ABC</span>
              <span className="text-gold"> XYZ</span>
            </Link>

            <form className="navbar-search flex">
              <input
                type="text"
                placeholder="Search here ..."
                value={searchText}
                onChange={(e) => setSearchText(e.target.value)}
              />
              <Link to= {`search=${searchText}`} className="">
              <button
                type="submit"
                className="navbar-search-btn"
                onClick={() => handleSearch(searchText)}
              >
                <i className="fas fa-search"></i>
              </button>
              </Link>
            </form>

            <div className="navbar-btns">
              <Link to="/cart" className="add-to-cart-btn flex">
                <span className="btn-ico">
                  <i className="fas fa-shopping-cart"></i>
                </span>
                <div className="btn-txt fw-5">
                  Cart
                  <span className="cart-count-value">{totalItems}</span>
                </div>
              </Link>
            </div>
          </div>
        </div>

        <div className="navbar-bottom bg-regal-blue">
          <div className="container flex flex-between">
            <ul
              className={`nav-links flex ${
                isSidebarOpen ? "show-nav-links" : ""
              }`}
            >
              <button
                type="button"
                className="navbar-hide-btn text-white"
                onClick={() => setIsSidebarOpen(false)}
              >
                <i className="fas fa-times"></i>
              </button>
              {categoriesData.result && categoriesData.result.map((category) => (
                <li key={category.id}>
                  <Link
                    to={`/category/${category.id}`}
                    className="nav-link text-white"
                    onClick={() => setIsSidebarOpen(false)}
                  >
                    {category.name}
                  </Link>
                </li>
              ))}
            </ul>

            <button
              type="button"
              className="navbar-show-btn text-gold"
              onClick={() => setIsSidebarOpen(true)}
            >
              <i className="fas fa-bars"></i>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
