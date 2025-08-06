using Microsoft.AspNetCore.Mvc;
using VuexyBase.Application.Application.Contracts.Application.Dashboard.City;
using VuexyBase.Application.Application.ViewModels.City;
using VuexyBase.Domain.Entities.General;

namespace VuexyBase.Dashboard.Controllers
{
    public class CityController : Controller
    {
        private readonly ICityService _cityService;
        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }
        public async Task<IActionResult> Index()
        {
            var cities = await _cityService.GetAllCities();
            return View(cities);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddCityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Regions = _cityService.GetAllRegions();
                return PartialView("_PartialAddCity", model);
            }

            var result = await _cityService.CreateCity(model);

            if (result == false)
            {
                ModelState.AddModelError("", "An error occurred, re-adding the city");
                ViewBag.Regions = _cityService.GetAllRegions();
                return PartialView("_PartialAddCity", model);
            }
            var cities = await _cityService.GetAllCities();

            Response.Headers["HX-Trigger"] = "cityUpdated";

            return PartialView("_PartialAddCity");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Regions = _cityService.GetAllRegions();
                return PartialView("_PartialEditCity", model);
            }

            var res = await _cityService.EditCity(model);

            if (res == false)
            {
                ModelState.AddModelError("", "An error occurred, re-editting the city");
                ViewBag.Regions = _cityService.GetAllRegions();
                return PartialView("_PartialEditCity", model);
            }

            var cities = await _cityService.GetAllCities();
            Response.Headers["HX-Trigger"] = "toggleOffcanvas";
            return PartialView("_PartialEditCity", new UpdateCityViewModel());
        }

        public IActionResult GetAddTablePartial()
        {
            ViewBag.Regions = _cityService.GetAllRegions();
            return PartialView("_PartialAddCity", new AddCityViewModel());
        }

        public IActionResult GetEditTablePartial()
        {
            ViewBag.Regions = _cityService.GetAllRegions();
            return PartialView("_PartialEditCity", new UpdateCityViewModel());
        }

        public async Task<IActionResult> GetTablePartial()
        {
            var cities = await _cityService.GetAllCities();
            return PartialView("_CityTablePartial", cities);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await _cityService.DeleteCity(id);
            var cities = await _cityService.GetAllCities();
            if (res)
            {
                return PartialView("_CityTablePartial", cities);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Status(int id)
        {
            var res = await _cityService.ChangeStatus(id);
            var cities = await _cityService.GetAllCities();
            if (res)
            {
                return PartialView("_CityTablePartial", cities);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Filter(int length, string search, string status)
        {
            var cities = await _cityService.FilterCities(length, search, status);

            ViewData["SelectedLength"] = length;
            ViewData["SearchText"] = search;
            ViewData["SelectedStatus"] = status;

            return PartialView("_CityTablePartial", cities);
        }
    }
}