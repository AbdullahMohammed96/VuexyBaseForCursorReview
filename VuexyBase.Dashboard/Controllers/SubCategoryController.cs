using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VuexyBase.Application.Application.Contracts.Application.Dashboard.Category;
using VuexyBase.Application.Application.Contracts.Application.Dashboard.SubCategory;
using VuexyBase.Application.Application.Services.Dashboard.Category;
using VuexyBase.Application.Application.ViewModels.Category;
using VuexyBase.Application.Application.ViewModels.SubCategory;

namespace VuexyBase.Dashboard.Controllers
{
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly IStringLocalizer Localization;


        public SubCategoryController(ISubCategoryService subCategoryService, IStringLocalizer localization)
        {
            _subCategoryService = subCategoryService;
            Localization = localization;
        }

        public async Task<IActionResult> Index()
        {
            var subCategories = await _subCategoryService.GetAllSubCategories();
            ViewBag.Categories = _subCategoryService.GetAllCategories();
            return View(subCategories);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddSubCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _subCategoryService.GetAllCategories();


                return PartialView("_PartialAddSubCategory", model);

            }

            var result = await _subCategoryService.CreateSubCategory(model);

            if (result == false)
            {
                ModelState.AddModelError("", "An error occurred, re-adding the SubCategory");
                ViewBag.Categories = _subCategoryService.GetAllCategories();

                return PartialView("_PartialAddSubCategory", model);

            }

            Response.Headers["HX-Trigger"] = "subCategoryUpdated";

            return PartialView("_PartialAddSubCategory");


        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateSubCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _subCategoryService.GetAllCategories();


                return PartialView("_PartialEditSubCategory", model);
            }

            var res = await _subCategoryService.EditSubCategory(model);

            if (res == false)
            {
                ModelState.AddModelError("", "An error occurred, re-editting the Sub category");
                ViewBag.Categories = _subCategoryService.GetAllCategories();

                return PartialView("_PartialEditSubCategory", model);

            }

            Response.Headers["HX-Trigger"] = "subCategoryUpdated";
            return PartialView("_PartialEditSubCategory", new UpdateSubCategoryViewModel());



        }

        public async Task<IActionResult> Delete(int id)
        {
            var IsDeleted = await _subCategoryService.DeleteSubCategory(id);

            if (IsDeleted == false)
                return BadRequest(new { success = false, message = "Car model not found or delete failed." });

            return Ok(new { success = true });


        }


        [HttpPost]
        public async Task<IActionResult> ChangeState(int id)
        {
            var newState = await _subCategoryService.ChangeStatus(id);

            if (newState is false)
                return BadRequest(new { success = false, message = "SubCategory not found or update failed." });

            //return Json(new { success = true, isActive = newState });
            return Json(new { data = newState });

        }



        [HttpGet]
        public async Task<IActionResult> Filter(int length, string search, string status)
        {
            var subcategories = await _subCategoryService.FilterSubCategories(length, search, status);

            ViewData["SelectedLength"] = length;
            ViewData["SearchText"] = search;
            ViewData["SelectedStatus"] = status;

            ViewData["FilterActionUrl"] = Url.Action("Filter", "SubCategory");
            ViewData["AddButtonText"] = Localization["AddNewSubCategoryCategory"];
            ViewData["AddButtonTarget"] = "#offcanvasAddData";
            ViewData["ShowStatusFilter"] = true;
            ViewData["ShowAddButton"] = true;

            return PartialView("_SubCategoryTablePartial", subcategories);

        }


        public IActionResult GetAddTablePartial()
        {
            return PartialView("_PartialAddSubCategory", new AddSubCategoryViewModel());

        }
        public IActionResult GetEditTablePartial()
        {
            return PartialView("_PartialEditSubCategory", new UpdateSubCategoryViewModel());

        }

        [HttpGet]
        public async Task<IActionResult> GetTablePartial()
        {
            var subCategories = await _subCategoryService.GetAllSubCategories();
            return PartialView("_SubCategoryTablePartial", subCategories);
        }


    }
}
