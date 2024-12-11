using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.EmployeeDTOs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.EmployeeVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
	public class EmployeeController : BaseController
	{
        private readonly IEmployeeService _employeeService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICompanyService _companyService;
        private readonly RoleManager<IdentityRole> _roleManager;
        public List<SelectListItem> roleOptions;
        private readonly IStringLocalizer<Resource> _stringLocalizer;

        public EmployeeController(IEmployeeService employeeService, IWebHostEnvironment webHostEnvironment, ICompanyService companyService, RoleManager<IdentityRole> roleManager, IStringLocalizer<Resource> stringLocalizer)
        {
            _employeeService = employeeService;
            _webHostEnvironment = webHostEnvironment;
            _companyService = companyService;
            _roleManager = roleManager;
            _stringLocalizer = stringLocalizer;
        }
        

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var result = await _employeeService.GetAllAsync();
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_LISTED_ERROR]);
                //NotifyError(result.Message);
                return View(Enumerable.Empty<EmployeeListVM>().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var employeeList = result.Data?.Adapt<List<EmployeeListVM>>();

                foreach (var employee in employeeList)
                {
                    var companyResult = await _companyService.GetByIdAsync(employee.CompanyId);
                    if (companyResult.IsSuccess)
                    {
                        employee.CompanyName = companyResult.Data.Name;
                    }
                    else
                    {
                        employee.CompanyName = "N/A";
                    }
                }

                var paginatedList = employeeList.ToPagedList(pageNumber, pageSize);
                NotifySuccess(_stringLocalizer[Messages.EMPLOYEE_LISTED_SUCCESS]);
                //NotifySuccess(result.Message);
                return View(paginatedList);
            }
        }

        // Aynı işlemleri create get ve postunda kullandığımız için buraya action yazıp buradan ilgili actionlara uygun model gönderiyoruz.
        private async Task<EmployeeCreateVM> PrepareEmployeeCreateVMAsync(EmployeeCreateVM model = null)
        {
            var roleOptions = new List<SelectListItem>();
            foreach (var role in _roleManager.Roles)
            {
                if (role.Name.ToLower() != "admin")
                {
                    roleOptions.Add(new SelectListItem
                    {
                        Value = role.Name,  // Rol adını kullanıyoruz
                        Text = role.Name
                    });
                }
            }

            var companiesResult = await _companyService.GetAllAsync();
            var companyOptions = new List<SelectListItem>();
            if (companiesResult.IsSuccess)
            {
                var companies = companiesResult.Data.Adapt<List<CompanyListVM>>();
                companyOptions = companies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
            }

            if (model == null)
            {
                model = new EmployeeCreateVM();
            }

            model.RoleOptions = roleOptions;
            model.CompanyOptions = companyOptions;

            return model;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await PrepareEmployeeCreateVMAsync();
            // Rolleri direkt controller içerisinde çekiyoruz
            model.RoleOptions = _roleManager.Roles
                .Where(r => r.Name.ToLower() != "admin")  // 'Admin' rolünü hariç tutuyoruz
                .Select(r => new SelectListItem
                {
                    Value = r.Name,  // Rol adını değer olarak kullanıyoruz
                    Text = _stringLocalizer[r.Name]  // Çevrilmiş rol adını gösteriyoruz
                })
                .ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateVM model)
        {
            // Model geçerli değilse
            if (!ModelState.IsValid)
            {

                NotifyError(_stringLocalizer["The_information_entered_is_not_valid."]);
                // Modeli yeniden hazırlayın (Rolleri ve şirketleri yeniden doldurmak için)
                model = await PrepareEmployeeCreateVMAsync(model);
                return View(model);
            }

            if (model.Photo != null)
            {
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Photo", _stringLocalizer["Invalid file type. Please upload an image file."]);
                    model = await PrepareEmployeeCreateVMAsync(model);
                    return View(model);
                }
            }

            // Kültürel bilgi ile ilk harfi büyük yap
            var turkishCulture = new CultureInfo("tr-TR");
            model.FirstName = StringUtilities.CapitalizeEachWord(model.FirstName);
            model.LastName = StringUtilities.CapitalizeFirstLetter(model.LastName);

            // EmployeeCreateDTO nesnesini doldurun
            var employeeDto = new EmployeeCreateDTO
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhotoData = null,
                Title = model.Title,  // Role ismini doğrudan kullanıyoruz
                CompanyId = model.CompanyId
            };

            // Eğer bir fotoğraf yüklenmişse, fotoğraf verisini al ve DTO'ya ekle
            if (model.Photo != null && model.Photo.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.Photo.CopyToAsync(memoryStream);
                    employeeDto.PhotoData = memoryStream.ToArray();
                }
            }

            // EmployeeService'i kullanarak çalışanı ekleyin
            var result = await _employeeService.AddAsync(employeeDto);

            // İşlem başarısızsa, hata mesajını göster ve formu yeniden doldurun
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_ADD_ERROR]);
                //NotifyError(result.Message);
                // Modeli yeniden hazırlayın (Rolleri ve şirketleri yeniden doldurmak için)
                model = await PrepareEmployeeCreateVMAsync(model);
                return View(model);
            }

            // Başarılı olduysa, başarı mesajını göster ve listeye yönlendirin
            NotifySuccess(_stringLocalizer[Messages.EMPLOYEE_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(Guid employeeId)
        {
            var result = await _employeeService.GetByIdAsync(employeeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_GETBYID_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var employeeDetails = result.Data.Adapt<EmployeeDetailsVM>();
            var companyResult = await _companyService.GetByIdAsync(employeeDetails.CompanyId);
            if (companyResult.IsSuccess)
            {
                employeeDetails.CompanyName = companyResult.Data.Name;
            }

            return View(employeeDetails);
        }

        public async Task<IActionResult> Update(Guid employeeId)
		{
			var result = await _employeeService.GetByIdAsync(employeeId);

			if (!result.IsSuccess)
			{
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_GETBYID_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
			}

            var companiesResult = await _companyService.GetAllAsync();
            if (!companiesResult.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_LISTED_ERROR]);
                //NotifyError(companiesResult.Message);
                return RedirectToAction("Index");
            }

            var companies = companiesResult.Data.Adapt<List<CompanyListVM>>();
            ViewBag.Companies = new SelectList(companies, "Id", "Name");
            var employeeUpdateVM = result.Data.Adapt<EmployeeUpdateVM>();

            
            if (employeeUpdateVM.PhotoData != null)
            {
                string base64 = Convert.ToBase64String(employeeUpdateVM.PhotoData);
                employeeUpdateVM.PhotoUrl = $"data:image/png;base64,{base64}";
            }

            return View(employeeUpdateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(EmployeeUpdateVM employeeUpdateVM)
        {
            if (ModelState.IsValid)
            {
                var turkishCulture=new CultureInfo("tr-TR");
                employeeUpdateVM.FirstName=StringUtilities.CapitalizeEachWord(employeeUpdateVM.FirstName);
                employeeUpdateVM.LastName=StringUtilities.CapitalizeFirstLetter(employeeUpdateVM.LastName);
                
                var employeeDto = employeeUpdateVM.Adapt<EmployeeUpdateDTO>();

                if (employeeUpdateVM.Photo != null && employeeUpdateVM.Photo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await employeeUpdateVM.Photo.CopyToAsync(memoryStream);
                        employeeDto.PhotoData = memoryStream.ToArray();
                    }
                }
                else
                {
                    employeeDto.PhotoData = employeeUpdateVM.PhotoData;
                }

                var result = await _employeeService.UpdateAsync(employeeDto);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.EMPLOYEE_UPDATE_ERROR]);
                    //NotifyError(result.Message);
                    var companiesResult = await _companyService.GetAllAsync();
                    if (companiesResult.IsSuccess)
                    {
                        var companies = companiesResult.Data.Adapt<List<CompanyListVM>>();
                        ViewBag.Companies = new SelectList(companies, "Id", "Name");
                    }
                    return View(employeeUpdateVM);
                }

                NotifySuccess(_stringLocalizer[Messages.EMPLOYEE_UPDATE_SUCCESS]);
                //NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }

            if (employeeUpdateVM.PhotoData != null)
            {
                string base64 = Convert.ToBase64String(employeeUpdateVM.PhotoData);
                employeeUpdateVM.PhotoUrl = $"data:image/png;base64,{base64}";
            }
            else
            {
                var existingEmployeeResult = await _employeeService.GetByIdAsync(employeeUpdateVM.Id);
                if (existingEmployeeResult.IsSuccess && existingEmployeeResult.Data.PhotoData != null)
                {
                    string base64 = Convert.ToBase64String(existingEmployeeResult.Data.PhotoData);
                    employeeUpdateVM.PhotoUrl = $"data:image/png;base64,{base64}";
                    employeeUpdateVM.PhotoData = existingEmployeeResult.Data.PhotoData;
                }
            }

            // If ModelState is not valid, return to the view with the model
            var companiesResultInvalid = await _companyService.GetAllAsync();
            if (companiesResultInvalid.IsSuccess)
            {
                var companies = companiesResultInvalid.Data.Adapt<List<CompanyListVM>>();
                ViewBag.Companies = new SelectList(companies, "Id", "Name");
            }
            return View(employeeUpdateVM);
        }

        public async Task<IActionResult> Delete(Guid employeeId)
		{
			var result = await _employeeService.DeleteAsync(employeeId);
			if (!result.IsSuccess)
			{
                NotifyError(_stringLocalizer[Messages.EMPLOYEE_DELETE_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.EMPLOYEE_DELETE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
		}
	}
}
