using Microsoft.AspNetCore.Mvc;
using VuexyBase.Application.Application.Contracts.Application.Dashboard.Region;
using VuexyBase.Application.Application.ViewModels.Region;
using VuexyBase.Domain.Entities.General;

namespace VuexyBase.Dashboard.Controllers
{
    public class RegionController : Controller
    {
        private readonly IRegionService _regionService;
        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }
        public async Task<IActionResult> Index()
        {
            //ListRegionViewModel regions = await _regionService.GetAllRegions();
            var regions = await _regionService.FilterRegions(50, "", "");
            return View(regions);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddRegionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_PartialAddRegion", model);
            }

            var result = await _regionService.CreateRegion(model);

            if (result == false)
            {
                ModelState.AddModelError("", "An error occurred, re-adding the region");
                return PartialView("_PartialAddRegion", model);
            }
            var regions = await _regionService.FilterRegions(50, "", "");

            Response.Headers["HX-Trigger"] = "regionUpdated";

            return PartialView("_PartialAddRegion");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateRegionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_PartialEditRegion", model);
            }

            var res = await _regionService.EditRegion(model);

            if (res == false)
            {
                ModelState.AddModelError("", "An error occurred, re-editting the region");
                return PartialView("_PartialEditRegion", model);
            }

            var regions = await _regionService.FilterRegions(50, "", "");
            Response.Headers["HX-Trigger"] = "toggleOffcanvas";
            return PartialView("_PartialEditRegion", new UpdateRegionViewModel());
        }

        public IActionResult GetAddTablePartial()
        {
            return PartialView("_PartialAddRegion", new AddRegionViewModel());
        }

        public IActionResult GetEditTablePartial()
        {
            return PartialView("_PartialEditRegion", new UpdateRegionViewModel());
        }

        public async Task<IActionResult> GetTablePartial()
        {
            var regions = await _regionService.FilterRegions(50, "", "");
            return PartialView("_RegionTablePartial", regions);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await _regionService.DeleteRegion(id);
            var regions = await _regionService.FilterRegions(50, "", "");
            if (res)
            {
                return PartialView("_RegionTablePartial", regions);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Status(int id)
        {
            var res = await _regionService.ChangeStatus(id);
            var regions = await _regionService.FilterRegions(50, "", "");
            if (res)
            {
                return PartialView("_RegionTablePartial", regions);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Filter(int length, string search, string status)
        {
            var regions = await _regionService.FilterRegions(length, search, status);

            ViewData["SelectedLength"] = length;
            ViewData["SearchText"] = search;
            ViewData["SelectedStatus"] = status;

            return PartialView("_RegionTablePartial", regions);
        }
    }
}