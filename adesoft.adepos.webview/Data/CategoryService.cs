using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class CategoryService
    {
        private readonly IConfiguration _configuration;
        private readonly CategoryController _categoryController;

        public CategoryService(IConfiguration configuration, CategoryController categoryController)
        {
            _configuration = configuration;
            _categoryController = categoryController;
        }

        public async Task<List<Category>> selectAll(Category category)
        {
            return await Task.FromResult(_categoryController.selectAll(category));
        }

        public async Task<Category> Create(Category model)
        {
            return await Task.FromResult(_categoryController.Create(model));
        }

        public async Task<Category> Update(Category model)
        {
            return await Task.FromResult(_categoryController.Update(model));
        }
        public async Task<Category> SelectById(Category model)
        {
            return await Task.FromResult(_categoryController.SelectById(model));
        }
    }
}
