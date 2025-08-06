using Microsoft.AspNetCore.Mvc;
using VuexyBase.Application.Application.Contracts.Application.Dashboard.Category;
using VuexyBase.Application.Application.ViewModels.Category;
using VuexyBase.Domain.Entities.General;

namespace VuexyBase.Dashboard.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            //ListCategoryViewModel categories = await _categoryService.GetAllCategories();
            var categories = await _categoryService.FilterCategories(50 ,"" ,"");
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {

                return PartialView("_PartialAddCategory", model);

            }

            var result = await _categoryService.CreateCategory(model);

            if(result == false)
            {
                ModelState.AddModelError("", "An error occurred, re-adding the category");
                return PartialView("_PartialAddCategory", model);

            }
            var categories = await _categoryService.FilterCategories(50, "", "");

                //Response.Headers["HX-Trigger"] = "toggleOffcanvas";
            Response.Headers["HX-Trigger"] = "categoryUpdated";
            //return  PartialView("_CategoryTablePartial", categories);

            return PartialView("_PartialAddCategory" );


        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {

                return PartialView("_PartialEditCategory", model);
            }

            var res = await _categoryService.EditCategory(model);

            if(res == false)
            {
                ModelState.AddModelError("", "An error occurred, re-editting the category");
                return PartialView("_PartialEditCategory", model);

            }

            var categories = await _categoryService.FilterCategories(50, "", "");
                Response.Headers["HX-Trigger"] = "toggleOffcanvas";
            //return PartialView("_CategoryTablePartial", categories);
            return PartialView("_PartialEditCategory", new UpdateCategoryViewModel() );



        }

        public IActionResult GetAddTablePartial()
        {
            return PartialView("_PartialAddCategory", new AddCategoryViewModel());

        }
        public IActionResult GetEditTablePartial()
        {
            return PartialView("_PartialEditCategory", new UpdateCategoryViewModel());

        }
        public async Task<IActionResult> GetTablePartial()
        {
            var categories = await _categoryService.FilterCategories(50, "", "");
            return PartialView("_CategoryTablePartial", categories);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await _categoryService.DeleteCategory(id);
            var categories =  await _categoryService.FilterCategories(50, "", "");
            //return PartialView("_CategoryTablePartial", categories);
            if (res)
            {
                return PartialView("_CategoryTablePartial", categories);

            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        public async Task<IActionResult> Status(int id)
        {
            var res = await _categoryService.ChangeStatus(id);
            var categories = await _categoryService.FilterCategories(50, "", "");
            if (res)
            {
                return PartialView("_CategoryTablePartial", categories);

            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Filter(int length, string search, string status)
        {
            var categories = await _categoryService.FilterCategories(length , search , status);

            ViewData["SelectedLength"] = length;
            ViewData["SearchText"] = search;
            ViewData["SelectedStatus"] = status;

            return PartialView("_CategoryTablePartial", categories);

        }
    }
}
