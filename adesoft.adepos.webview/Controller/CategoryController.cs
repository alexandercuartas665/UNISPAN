using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using adesoft.adepos.Extensions;
namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;

        public CategoryController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public Category Create(Category category)
        {
            Category find = _dbcontext.Categorys.Where(x => x.Name == category.Name).FirstOrDefault();
            if (find == null)
            {
                _dbcontext.Categorys.Add(category);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return category;
        }


        public Category Update(Category category)
        {
            Category find = _dbcontext.Categorys.Where(x => x.CategoryId == category.CategoryId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<Category>(category).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return category;
        }

        public Category SelectById(Category category)
        {
            Category find = _dbcontext.Categorys.Where(x => x.CategoryId == category.CategoryId).FirstOrDefault();

            return find;
        }
        public List<Category> selectAll(Category Category)
        {
            if (Category.TransOption == 1)
            {
                return _dbcontext.Categorys.ToList();
            }
            else if (Category.TransOption == 2)
            {
                //categorias de medicion
                return _dbcontext.Categorys.Where(x => x.TypeCategoryId == 2).ToList();
            }
            else
            {
                return _dbcontext.Categorys.ToList();
            }
        }


    }
}