﻿using AffiliateStoreBE.Models;

namespace AffiliateStoreBE.Service.IService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoryByName(List<string> categoryNames);
    }
}
