using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CountryVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class CountryController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly IStringLocalizer<CountryController> stringLocalizers;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public CountryController(ICountryService countryService, IStringLocalizer<CountryController> stringLocalizers, IStringLocalizer<Resource> stringLocalizer)
        {
            _countryService = countryService;
            this.stringLocalizers = stringLocalizers;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<IActionResult> Index(int? page, string sortOrder)
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 5;
                ViewBag.CurrentSort = sortOrder;

                var result = await _countryService.GetAllAsync();
                var countryListVMs = result.Data.Adapt<List<CountryListVM>>();

                switch (sortOrder)
                {
                    case "name_asc":
                        countryListVMs = countryListVMs.OrderBy(c => c.Name).ToList();
                        break;
                    case "name_desc":
                        countryListVMs = countryListVMs.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "date_asc":
                        countryListVMs = countryListVMs.OrderBy(c => c.CreatedDate).ToList();
                        break;
                    case "date_desc":
                        countryListVMs = countryListVMs.OrderByDescending(c => c.CreatedDate).ToList();
                        break;
                    default:
                        countryListVMs = countryListVMs.OrderBy(c => c.Name).ToList();
                        break; 
                }

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_LISTED_ERROR]);
                    // NotifyError(stringLocalizers[result.Message].Value);
                    return View(Enumerable.Empty<CountryListVM>().ToPagedList(pageNumber, pageSize));
                }
                NotifySuccess(_stringLocalizer[Messages.COUNTRY_LISTED_SUCCESS]);
                // NotifySuccess(stringLocalizers[result.Message].Value);

                var paginatedList = countryListVMs.Adapt<List<CountryListVM>>().ToPagedList(pageNumber, pageSize);

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_LISTED_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(Guid countryId)
        {
            try
            {
                var result = await _countryService.GetByIdAsync(countryId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_GETBYID_ERROR]);
                    //NotifyError(stringLocalizers[result.Message].Value);
                    return RedirectToAction("Index");
                }
                NotifySuccess(_stringLocalizer[Messages.COUNTRY_GETBYID_SUCCESS]);
                //NotifySuccess(stringLocalizers[result.Message].Value);

                var countryDetailsVM = result.Data.Adapt<CountryDetailsVM>();

                return View(countryDetailsVM);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_GETBYID_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_CREATE_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryCreateVM countryCreateVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(countryCreateVM);
                }
                countryCreateVM.Name=StringUtilities.CapitalizeEachWord(countryCreateVM.Name);

                var result = await _countryService.AddAsync(countryCreateVM.Adapt<CountryCreateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_CREATE_ERROR]);
                    //NotifyError(stringLocalizers[result.Message].Value);
                    return View(countryCreateVM);

                }
                NotifySuccess(_stringLocalizer[Messages.COUNTRY_CREATE_SUCCESS]);
                //NotifySuccess(stringLocalizers[result.Message].Value);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_CREATE_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        public async Task<IActionResult> Update(Guid countryId)
        {
            try
            {
                var result = await _countryService.GetByIdAsync(countryId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_UPDATE_ERROR]);
                    //NotifyError(stringLocalizers[result.Message].Value);
                    return RedirectToAction("Index");
                }

                NotifySuccess(_stringLocalizer[Messages.COUNTRY_UPDATE_SUCCESS]);
                //NotifySuccess(stringLocalizers[result.Message].Value);

                var countryUpdateVM = result.Data.Adapt<CountryUpdateVM>();

                return View(countryUpdateVM);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_UPDATE_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CountryUpdateVM countryUpdateVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(countryUpdateVM);
                }
                 
                countryUpdateVM.Name=StringUtilities.CapitalizeEachWord(countryUpdateVM.Name);

                var result = await _countryService.UpdateAsync(countryUpdateVM.Adapt<CountryUpdateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_UPDATE_ERROR]);
                    // NotifyError(stringLocalizers[result.Message].Value);
                    return View(countryUpdateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.COUNTRY_UPDATE_SUCCESS]);
                //NotifySuccess(stringLocalizers[result.Message].Value);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_UPDATE_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(Guid countryId)
        {
            try
            {
                var result = await _countryService.DeleteAsync(countryId);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.COUNTRY_DELETE_ERROR]);
                    //NotifyError(stringLocalizers[result.Message].Value);
                }

                NotifySuccess(_stringLocalizer[Messages.COUNTRY_DELETE_SUCCESS]);
                //NotifySuccess(stringLocalizers[result.Message].Value);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer[Messages.COUNTRY_DELETE_ERROR]);
                //Console.WriteLine(ex.Message);
                return View("Error");
            }
        }
    }
}
