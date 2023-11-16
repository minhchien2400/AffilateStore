import "./Filter.scss";
import { useState } from "react";


const Filter = () => {

const[filter, setFilter] = useState('');
const[topSale, setTopSale] = useState(false);
const[overStars, setOverStars] = useState(false);
const[price, setPrice] = useState(false);

const handleFilter = (className, filter) => {

}

  return (
    <div className="container">
      <div class="flex filter">
        <button className="filter-top-sale" onClick={() => setFilter("topsale")}>Top Sale</button>
        <button className="filter-over-stars" onClick={() => setFilter("overstars")}>Over 4 Stars</button>
        <div className="filter-price flex">
          <div>Gia</div>
          <div className="filter-price-top">
            <div>
                ^
            </div>
            <div>
                v
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Filter;
