using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.MVC.Models.CityVMs;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using System;
using BaSalesManagementApp.Business.Utilities;
using Microsoft.Extensions.Localization;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class CityController : BaseController
    {
        private readonly ICityService _cityService;
        private readonly ICountryService _countryService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public CityController(ICityService cityService, ICountryService countryService, IStringLocalizer<Resource> stringLocalizer)
        {
            _cityService = cityService;
            _countryService = countryService;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical")
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 5;
                
                var result = await _cityService.GetAllAsync(sortOrder);
                var cityListVMs = result.Data.Adapt<List<CityListVM>>();
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_LISTED_ERROR]);
                    //NotifyError(result.Message);
                    return View(Enumerable.Empty<CityListVM>().ToPagedList(pageNumber, pageSize));
                }

                //var cityListVMs = result.Data.Adapt<List<CityListVM>>();
                //var paginatedList = cityListVMs.ToPagedList(pageNumber, pageSize);

                NotifySuccess(_stringLocalizer[Messages.CITY_LISTED_SUCCESS]);
                // NotifySuccess(result.Message);
                var paginatedList = result.Data.Adapt<List<CityListVM>>().ToPagedList(pageNumber, pageSize);
                ViewData["CurrentSortOrder"] = sortOrder;
                ViewData["CurrentPage"] = pageNumber;
               
                return View(paginatedList);
            }
            catch (Exception ex)
            {
                var errorMessage = _stringLocalizer[Messages.CITY_LISTED_ERROR] + ": " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(Guid cityId)
        {
            try
            {
                var result = await _cityService.GetByIdAsync(cityId);

                if (!result.IsSuccess)
                {

                    NotifyError(_stringLocalizer[Messages.CITY_NOT_FOUND]);
                    //NotifyError(result.Message);
                    return RedirectToAction("Index");
                }
                NotifySuccess(_stringLocalizer[Messages.CITY_GET_SUCCESS]);
                // NotifySuccess(result.Message);

                var cityDetailsVM = result.Data.Adapt<CityDetailsVM>();

                return View(cityDetailsVM);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var countriesResult = await _countryService.GetAllAsync();
                var countries = countriesResult.Data?.Adapt<List<CountryDTO>>() ?? new List<CountryDTO>();

                var cityCreateVM = new CityCreateVM()
                {
                    Countries = countries,
                };
                return View(cityCreateVM);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CityCreateVM cityCreateVM)
        {
            try
            {
                cityCreateVM.Name=StringUtilities.CapitalizeFirstLetter(cityCreateVM.Name);

                var countriesResult = await _countryService.GetAllAsync();
                cityCreateVM.Countries = countriesResult.Data?.Adapt<List<CountryDTO>>() ?? new List<CountryDTO>();

                if (!ModelState.IsValid)
                {
                    return View(cityCreateVM);
                }

                var result = await _cityService.AddAsync(cityCreateVM.Adapt<CityCreateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_CREATED_ERROR]);
                    // NotifyError(result.Message);
                    return View(cityCreateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.CITY_CREATED_SUCCESS]);
                // NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.CITY_CREATED_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }


        public async Task<IActionResult> Update(Guid cityId)
        {
            try
            {
                var result = await _cityService.GetByIdAsync(cityId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_NOT_FOUND]);
                    // NotifyError(result.Message);
                    return RedirectToAction("Index");
                }

                NotifySuccess(_stringLocalizer[Messages.CITY_GET_SUCCESS]);
                // NotifySuccess(result.Message);
                var countriesResult = await _countryService.GetAllAsync();
                var countries = countriesResult.Data?.Adapt<List<CountryDTO>>() ?? new List<CountryDTO>();

                var cityUpdateVM = result.Data.Adapt<CityUpdateVM>();
                cityUpdateVM.Countries = countries;
                return View(cityUpdateVM);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.CITY_UPDATED_ERROR]);
                // Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CityUpdateVM cityUpdateVM)
        {
            try
            {
                
                cityUpdateVM.Name=StringUtilities.CapitalizeFirstLetter(cityUpdateVM.Name);

                var countriesResult = await _countryService.GetAllAsync();
                cityUpdateVM.Countries = countriesResult.Data?.Adapt<List<CountryDTO>>() ?? new List<CountryDTO>();

                if (!ModelState.IsValid)
                {
                    return View(cityUpdateVM);
                }

                var result = await _cityService.UpdateAsync(cityUpdateVM.Adapt<CityUpdateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_UPDATED_ERROR]);
                    //NotifyError(result.Message);
                    return View(cityUpdateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.CITY_UPDATED_SUCCESS]);
                // NotifySuccess(result.Message);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.CITY_UPDATED_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(Guid cityId)
        {
            try
            {
                var result = await _cityService.DeleteAsync(cityId);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CITY_DELETED_ERROR]);
                    //NotifyError(result.Message);
                }

                NotifySuccess(_stringLocalizer[Messages.CITY_DELETED_SUCCESS]);

                // NotifySuccess(result.Message);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.CITY_DELETED_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }
    }
}
