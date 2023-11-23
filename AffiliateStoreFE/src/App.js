import "./App.scss";
import { BrowserRouter, Routes, Route } from "react-router-dom";
// pages
import { Home, Category, Cart } from "./pages/index";
// components
import Navbar from "./components/Navbar/Navbar";
import Footer from "./components/Footer/Footer";
import { Provider } from "react-redux";
import store from "./store/store";
import CategoryPage from "./pages/CategoryPage/CategoryPage";
import ProductsPage from "./pages/ProductsPage/ProductsPage";

import { formatStars } from "./utils/helpers";
import Filter from "./components/Filter/Filter";
function App() {
  return (
    <div className="App">
      <Filter/>
      {/* <Provider store = {store}>
        <BrowserRouter>
          <Navbar />
          <Routes>
            <Route path = "/" element = {<Home />} />
            <Route path = "/category/:id" element = {<Category />} />
            <Route path = "/cart" element = {<Cart />} />
            <Route path = "/search=:searchText" element = {<ProductsPage />} />
          </Routes>
          <Footer />
        </BrowserRouter>
      </Provider> */}
    </div>
  );
}

export default App;
