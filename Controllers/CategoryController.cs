using DotNetCoreSqlDb.Models;
using DotNetCoreSqlDb.Repository;
using DotNetCoreSqlDb.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreSqlDb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOFWork _unitOfWork;
        public CategoryController(IUnitOFWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upsert([FromBody] Category category)
        {
            if (category.Id == 0)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
            }else
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
            }
            return NoContent();
        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Category.GetAll() });
        }
        [HttpGet]
        public IActionResult GetCategoryById(int id)
        {
            Category category = _unitOfWork.Category.Get(id);
            if (category == null)
            { 
              return NotFound();
            }
            else
            {
                return Json(new { data = category });

            }
        }
        #endregion
    }
}
