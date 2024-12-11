using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.CityDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CountryDTOs;
using BaSalesManagementApp.Dtos.CustomerDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.CustomerVMs;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly ICompanyService _companyService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        /// <summary>
        /// Müşteri kontrolcüsü
        /// </summary>
        /// <param name="customerService">Müşteri işlemleri servisi</param>
        /// <param name="companyService">Şirket işlemleri servisi</param>
        /// /// <param name="stringLocalizer">Çeviri hizmeti.</param>
        /// <param name="countryService">Ülke işlemleri servisi.</param>
        /// <param name="cityService">Şehir işlemleri servisi.</param>
        public CustomerController(ICustomerService customerService, ICompanyService companyService, IStringLocalizer<Resource> stringLocalizer, ICountryService countryService, ICityService cityService)
        {
            _customerService = customerService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;
            _countryService = countryService;
            _cityService = cityService;
        }


        /// <summary>
        /// Müşterileri gösteren ana sayfa görünümü
        /// </summary>
        /// <returns>Tüm müşterileri listeleyen ana sayfa görünümünü döndürür.</returns>
        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical")
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 5;

                var result = await _customerService.GetAllAsync(sortOrder);
               var customerListVMs = result.Data.Adapt<List<CustomerListVM>>();

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CUSTOMER_LIST_FAILED]);
                    // NotifyError(result.Message);
                    return View(Enumerable.Empty<CustomerListVM>().ToPagedList(pageNumber, pageSize));
                }

                NotifySuccess(_stringLocalizer[Messages.CUSTOMER_LISTED_SUCCESS]);
                //NotifySuccess(result.Message);
                var paginatedList = result.Data.Adapt<List<CustomerListVM>>().ToPagedList(pageNumber, pageSize);
                ViewData["CurrentSortOrder"] = sortOrder;
                ViewData["CurrentPage"] = pageNumber;
                return View(paginatedList);
            }
            catch (Exception ex)
            {
                
                NotifyError(_stringLocalizer[Messages.CUSTOMER_GET_FAILED] + ": " + ex.Message);
                return View("Error");
            }

        }
        /// <summary>
        /// Yeni bir müşteri oluşturma sayfası
        /// /// Şirket, şehir ve ülke bilgilerini doldurarak sayfa modelini hazırlar.
        /// </summary>
        /// <returns>Yeni bir müşteri oluşturma sayfasını döndürür.</returns>
        public async Task<IActionResult> Create()
        {
            var companiesRes = await _companyService.GetAllAsync();
            var citiesRes = await _cityService.GetAllAsync(); 
            var countriesRes = await _countryService.GetAllAsync();
            var model = new CustomerCreateVM
            {
                Companies = companiesRes.Data.Adapt<List<Company>>(),
                Cities = citiesRes.Data.Adapt<List<City>>(),
                Countries = countriesRes.Data.Adapt<List<Country>>()
            };
            return View(model);
        }

        /// <summary>
        /// Yeni bir müşteri oluşturma işlemi
        /// Form doğrulaması başarılı ise müşteri oluşturulur ve başarılı sonuç mesajıyla müşteri listesine yönlendirilir.
        /// </summary>
        /// <param name="model">Müşteri oluşturmak için gerekli bilgileri içeren model</param>
        /// <returns>İşlem başarılı ise müşteri listesine yönlendirir. Başarısız ise hata mesajı ile birlikte aynı sayfayı tekrar döndürür.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                // olası bir hata durumunda dropdown list'in tekrar doldurulması
                var companiesRes = await _companyService.GetAllAsync();
                if (companiesRes.IsSuccess && companiesRes.Data != null)
                {
                    model.Companies = companiesRes.Data.Adapt<List<Company>>();
                }
                var citiesRes = await _cityService.GetAllAsync();
                if (citiesRes.IsSuccess && citiesRes.Data != null)
                {
                    model.Cities = citiesRes.Data.Adapt<List<City>>();
                }
                var countriesRes = await _countryService.GetAllAsync();
                if (countriesRes.IsSuccess && countriesRes.Data != null)
                {
                    model.Countries = countriesRes.Data.Adapt<List<Country>>();
                }
                return View(model);
            }

            
            model.Name=StringUtilities.CapitalizeEachWord(model.Name);
            model.Address=StringUtilities.CapitalizeEachWord(model.Address);

            var result = await _customerService.AddAsync(model.Adapt<CustomerCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_ADD_ERROR]);
                //NotifyError(result.Message);
                return View(model);
            }

            NotifySuccess(_stringLocalizer[Messages.CUSTOMER_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Müşteri detaylarını gösteren sayfa
        /// </summary>
        /// <param name="customerId">Müşteri Id</param>
        /// <returns>Müşteri detaylarını gösteren sayfayı döndürür.</returns>
        public async Task<IActionResult> Details(Guid customerId)
        {
            var result = await _customerService.GetByIdAsync(customerId);


            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_NOT_FOUND]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var customerDetailsVM = result.Data.Adapt<CustomerDetailsVM>();

            return View(customerDetailsVM);
        }

        /// <summary>
        /// Müşteri bilgilerini güncellemek için sayfa
        /// Müşterinin şirket, şehir ve ülke bilgilerini de günceller.
        /// </summary>
        /// <param name="customerId">Müşteri Id</param>
        /// <returns>Müşteri bilgilerini güncellemek için sayfayı döndürür.</returns>
        public async Task<IActionResult> Update(Guid customerId)
        {
            var result = await _customerService.GetByIdAsync(customerId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var companiesResult = await _companyService.GetAllAsync();
            var citiesResult = await _cityService.GetAllAsync();
            var countriesResult = await _countryService.GetAllAsync();

            var customerUpdateVM = result.Data.Adapt<CustomerUpdateVM>();

            

            customerUpdateVM.Companies = companiesResult.Data.Adapt<List<CompanyDTO>>();
            customerUpdateVM.Cities = citiesResult.Data.Adapt<List<CityDTO>>();
            customerUpdateVM.Countries = countriesResult.Data.Adapt<List<CountryDTO>>();

            return View(customerUpdateVM);
        }

        /// <summary>
        /// Müşteri bilgilerini güncellemek için işlem
        /// </summary>
        /// <param name="customerUpdateVM">Müşteri bilgilerini güncellemek için gerekli bilgileri içeren model</param>
        /// <returns>İşlem başarılı ise müşteri listesine yönlendirir. Başarısız ise hata mesajı ile birlikte aynı sayfayı tekrar döndürür.</returns>
        [HttpPost]
        public async Task<IActionResult> Update(CustomerUpdateVM customerUpdateVM)
        {
            if (!ModelState.IsValid)
            {
                
                var companiesRes = await _companyService.GetAllAsync();
                var citiesRes = await _cityService.GetAllAsync();
                var countriesRes = await _countryService.GetAllAsync();

                if (companiesRes.IsSuccess && companiesRes.Data != null)
                {
                    customerUpdateVM.Companies = companiesRes.Data.Adapt<List<CompanyDTO>>();
                }
                if (citiesRes.IsSuccess && citiesRes.Data != null)
                {
                    customerUpdateVM.Cities = citiesRes.Data.Adapt<List<CityDTO>>();
                }

                if (countriesRes.IsSuccess && countriesRes.Data != null)
                {
                    customerUpdateVM.Countries = countriesRes.Data.Adapt<List<CountryDTO>>();
                }
                return View(customerUpdateVM);
            }

            
            customerUpdateVM.Name=StringUtilities.CapitalizeEachWord(customerUpdateVM.Name);
            customerUpdateVM.Address=StringUtilities.CapitalizeEachWord(customerUpdateVM.Address);

            var result = await _customerService.UpdateAsync(customerUpdateVM.Adapt<CustomerUpdateDTO>());

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_UPDATED_FAILED]);
                //NotifyError(result.Message);
                return View(customerUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.CUSTOMER_UPDATED_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index"); ;
        }

        /// <summary>
        /// Müşteri silme işlemi
        /// </summary>
        /// <param name="customerId">Müşteri Id</param>
        /// <returns>İşlem başarılı ise müşteri listesine yönlendirir. Başarısız ise hata mesajı ile birlikte aynı sayfayı tekrar döndürür.</returns>
        public async Task<IActionResult> Delete(Guid customerId)
        {
            var result = await _customerService.DeleteAsync(customerId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CUSTOMER_DELETE_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.CUSTOMER_DELETE_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }
    }
}
