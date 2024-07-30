using DotNetCoreSqlDb.Models;
using DotNetCoreSqlDb.Repository;
using DotNetCoreSqlDb.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreSqlDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly IUnitOFWork _unitOfWork;
        public CategoryController(IUnitOFWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
     
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Category.GetAll() });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = new Category();
            category = await _unitOfWork.Category.GetCategoryById(id);
            if(category == null)
            {
                return NotFound();
            }
            return category;
        }
        #endregion
    }
}
