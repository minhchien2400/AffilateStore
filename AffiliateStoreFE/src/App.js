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

import SignIn from "./components/SignIn/SignIn";
import SignUp from "./components/SignUp/SignUp";

import ProductDetailPage from "./pages/ProductDetailPage/ProductDetailPage";

import CartFilter from "./components/Filter/CartFilter";

function App() {
  return (
    <div className="App">
      <Provider store={store}>
        <BrowserRouter>
          {/* <Navbar />
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/category/:id" element={<Category />} />
            <Route path="/cart" element={<Cart />} />
            <Route path="/search=:searchText" element={<ProductsPage />} />
            <Route path="/product/:id" element={<ProductDetailPage />} />
            <Route
              path="/category/:categoryid/product/:id"
              element={<ProductDetailPage />}
            />
            <Route path="cart/search=:searchCart" element={<Cart />} />
            <Route path="/login" element={<SignIn />} />
            <Route path="/signup" element={<SignUp />} />
          </Routes>
          <Footer /> */}
          <SignIn/>
        </BrowserRouter>
      </Provider>
    </div>
  );
}

export default App;
