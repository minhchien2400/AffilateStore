import React, { useState, useEffect } from "react";
import "./Navbar.scss";
import { Link } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { setFilterAction } from "../../store/filterSlice";
import { DecodedJwtTokenData } from "../../utils/helpers";
import { fetchTotalAdded } from "../../store/cartSlice";
import { SET_PRODUCTS_FILTER as type} from "../../utils/const";

const Navbar = () => {
  const dispatch = useDispatch();

  const { data: page } = useSelector((state) => state.page);

  const { data: categoriesData } = useSelector((state) => state.category);

  const { ProductsFilter: dataFilter } = useSelector((state) => state.filter);

  const { ProductsAdded: productsAdded } = useSelector((state) => state.cart);

  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const [searchText, setSearchText] = useState("");
  const { IsLoggedIn: isLoggedIn } = useSelector((state) => state.login);

  const accessToken = localStorage.getItem("jwtToken");

  // useEffect(() => {
  //   dispatch(fetchCategories(filterCategories, 'POST'));
  //   dispatch(getCartTotal());
  //   // eslint-disable-next-line react-hooks/exhaustive-deps
  // }, []);
  useEffect(() => {
    dispatch(fetchTotalAdded())
  },[productsAdded.totalProducts])

  const handleSearch = (newSearchText) => {
    dispatch(
      setFilterAction(type, {
        Offset: dataFilter.Offset,
        Limit: dataFilter.Limit,
        SearchText: newSearchText,
        Keys: dataFilter.Keys,
      })
    );
    setSearchText("");
  };

  const handelClickBtn = () => {};

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
              <Link to={`search=${searchText}`} className="">
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
                  <span className="cart-count-value">{productsAdded.totalProducts}</span>
                </div>
              </Link>
            </div>

            <Link to="/login">
              <div className="login-btn">
                <button onClick={() => handelClickBtn()}>
                  {accessToken ? DecodedJwtTokenData(accessToken).UserName : "Login"}
                </button>
              </div>
            </Link>

            <button
              onClick={() => {
                localStorage.removeItem("jwtToken");
              }}
            >
              LogOut
            </button>
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
              {categoriesData.result &&
                categoriesData.result.map((category) => (
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
