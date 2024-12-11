using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.EmployeeDTOs;
using BaSalesManagementApp.MVC.Models.AdminVMs;
using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.MVC.Models.EmployeeVMs;
using X.PagedList;
using Microsoft.Extensions.Localization;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace BaSalesManagementApp.MVC.Controllers
{
	public class AdminController : BaseController
	{
		private readonly IAdminService _adminService;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IStringLocalizer<Resource> _stringLocalizer;

		public AdminController(IAdminService adminService, IWebHostEnvironment webHostEnvironment, IStringLocalizer<Resource> stringLocalizer)
		{
			_adminService = adminService;
			_webHostEnvironment = webHostEnvironment;
			_stringLocalizer = stringLocalizer;
		}


		public async Task<IActionResult> Index(int? page, string sortAdmin = "name")
		{
			try
			{
				int pageNumber = page ?? 1;
				int pageSize = 5;

				var result = await _adminService.GetAllAsync(sortAdmin);
				if (!result.IsSuccess)
				{
					NotifyError(result.Message);
					return View(Enumerable.Empty<AdminListVM>().ToPagedList(pageNumber, pageSize));
				}

				var adminListVM = result.Data.Adapt<List<AdminListVM>>();

				var paginatedList = adminListVM.ToPagedList(pageNumber, pageSize);
				NotifySuccess(_stringLocalizer[Messages.ADMIN_LISTED_SUCCESS]);
                ViewData["CurrentSortAdmin"] = sortAdmin;
                ViewData["CurrentPage"] = pageNumber;
                return View(paginatedList);
			}
			catch (Exception ex)
			{
				NotifyError(_stringLocalizer[Messages.ADMIN_LISTED_ERROR] + ": " + ex.Message);
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
				NotifyError(_stringLocalizer[Messages.ADMIN_CREATE_ERROR] + ": " + ex.Message);

				return View("Error");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AdminCreateVM adminCreateVM)
		{
			if (ModelState.IsValid)
			{

				adminCreateVM.FirstName = StringUtilities.CapitalizeEachWord(adminCreateVM.FirstName);
				adminCreateVM.LastName = StringUtilities.CapitalizeFirstLetter(adminCreateVM.LastName);

				var adminDto = adminCreateVM.Adapt<AdminCreateDTO>();
				byte[] photoBytes = null;
				if (adminCreateVM.Photo != null && adminCreateVM.Photo.Length > 0)
				{
					using (var memoryStream = new MemoryStream())
					{
						await adminCreateVM.Photo.CopyToAsync(memoryStream);
						photoBytes = memoryStream.ToArray();
					}
				}
				else
				{
					adminDto.PhotoData = null;
				}
                if (adminCreateVM.Photo != null)
                {
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(adminCreateVM.Photo.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Photo", _stringLocalizer["Invalid file type. Please upload an image file."]);
                        
                        return View(adminCreateVM);
                    }
                }
                adminDto.PhotoData = photoBytes;
				var result = await _adminService.AddAsync(adminDto);
				if (!result.IsSuccess)
				{
					NotifyError(_stringLocalizer[Messages.ADMIN_CREATE_ERROR]);
					//  NotifyError(result.Message);
					return View(adminCreateVM);
				}
				NotifySuccess(_stringLocalizer[Messages.ADMIN_CREATED_SUCCESS]);
				// NotifySuccess(result.Message);
				return RedirectToAction("Index");
			}
			return View(adminCreateVM);
		}

		public async Task<IActionResult> Delete(Guid adminId)
		{
			try
			{
				var result = await _adminService.DeleteAsync(adminId);
				if (!result.IsSuccess)
				{
					NotifySuccess(_stringLocalizer[Messages.ADMIN_DELETED_SUCCESS]);
					// NotifyError(result.Message);
				}

				NotifySuccess(result.Message);
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				NotifyError(_stringLocalizer[Messages.ADMIN_DELETE_ERROR] + ": " + ex.Message);
				//NotifyError("Admin silinirken bir hata meydana geldi: " + ex.Message);
				return View("Index");
			}
		}
        public async Task<IActionResult> Update(Guid adminId)
        {
            var result = await _adminService.GetByIdAsync(adminId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.ADMIN_GETBYID_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            

            var adminUpdateVM = result.Data.Adapt<AdminUpdateVM>();

            if (adminUpdateVM.PhotoData != null)
            {
                string base64 = Convert.ToBase64String(adminUpdateVM.PhotoData);
                adminUpdateVM.PhotoUrl = $"data:image/png;base64,{base64}";
            }

            return View(adminUpdateVM);
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(AdminUpdateVM adminUpdateVM)
		{

			if (ModelState.IsValid)
			{

				adminUpdateVM.FirstName = StringUtilities.CapitalizeEachWord(adminUpdateVM.FirstName);
				adminUpdateVM.LastName = StringUtilities.CapitalizeFirstLetter(adminUpdateVM.LastName);

				var adminDto = adminUpdateVM.Adapt<AdminUpdateDTO>();
				if (adminUpdateVM.Photo != null && adminUpdateVM.Photo.Length > 0)
				{
					using (var memoryStream = new MemoryStream())
					{
						await adminUpdateVM.Photo.CopyToAsync(memoryStream);
						adminDto.PhotoData = memoryStream.ToArray();
					}
				}
				else
				{
					var existingAdmin = await _adminService.GetByIdAsync(adminDto.Id);
					adminDto.PhotoData = existingAdmin.Data.PhotoData;
				}

				var result = await _adminService.UpdateAsync(adminDto);
				if (!result.IsSuccess)
				{
					NotifyError(_stringLocalizer[Messages.ADMIN_UPDATE_ERROR]);
					// NotifyError(result.Message);
					return View(adminUpdateVM);
				}

				var updateResult = await _adminService.UpdateAsync(adminUpdateVM.Adapt<AdminUpdateDTO>());
				if (!updateResult.IsSuccess)
				{
					NotifyError(_stringLocalizer[Messages.ADMIN_UPDATE_ERROR]);
					//NotifyError(updateResult.Message);
					return View(adminUpdateVM);
				}
				NotifySuccess(_stringLocalizer[Messages.ADMIN_UPDATE_SUCCESS]);
				// NotifySuccess(updateResult.Message);
			}
			else
			{
				return View(adminUpdateVM);
			}
			return RedirectToAction("Index");

		}

		public async Task<IActionResult> Details(Guid adminId)
		{
			var result = await _adminService.GetByIdAsync(adminId);
			if (!result.IsSuccess)
			{
				NotifyError(_stringLocalizer[Messages.ADMIN_GETBYID_ERROR]);
				// NotifyError(result.Message);
				return RedirectToAction("Index");
			}
			NotifySuccess(_stringLocalizer[Messages.ADMIN_GETBYID_SUCCESS]);
			// NotifySuccess(result.Message);
			return View(result.Data.Adapt<AdminDetailsVM>());
		}
	}
}
