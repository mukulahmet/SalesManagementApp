using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public CategoryController(ICategoryService categoryService, IStringLocalizer<Resource> stringLocalizer)
        {
            _categoryService = categoryService;
            _stringLocalizer = stringLocalizer;
        }

        // Tüm kategorileri listeleyen ana sayfa görünümünü döndürür.
        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical")
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 5;

                var result = await _categoryService.GetAllAsync(sortOrder);
                var categoryListVMs = result.Data.Adapt<List<CategoryListVM>>();

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CATEGORY_LIST_EMPTY]);
                    //NotifyError(result.Message);
                    return View(Enumerable.Empty<CategoryListVM>().ToPagedList(pageNumber, pageSize));
                }

                NotifySuccess(_stringLocalizer[Messages.CATEGORY_LISTED_SUCCESS]);
                //NotifySuccess(result.Message);
                var paginatedList = result.Data.Adapt<List<CategoryListVM>>().ToPagedList(pageNumber, pageSize);
                ViewData["CurrentSortOrder"] = sortOrder;
                ViewData["CurrentPage"] = pageNumber;
                return View(paginatedList);
            }
            catch (Exception ex)
            {
                var errorMessage = "Kategorileri getirirken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }


        // Yeni bir kategori oluşturur ve ana sayfaya yönlendirir.
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                var errorMessage = "Sayfa yüklenirken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM categoryCreateVM)
        {
            try
            {
               
                categoryCreateVM.Name=StringUtilities.CapitalizeFirstLetter(categoryCreateVM.Name);

                var result = await _categoryService.AddAsync(categoryCreateVM.Adapt<CategoryCreateDTO>());

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CATEGORY_CREATE_FAILED]);
                    //  NotifyError(result.Message);
                    return View();
                }

                NotifySuccess(_stringLocalizer[Messages.CATEGORY_CREATED_SUCCESS]);
                // NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var errorMessage = "Kategori oluşturulurken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        // Belirtilen ID'li kategoriyi siler ve ana sayfaya yönlendirir.
        public async Task<IActionResult> Delete(Guid categoryId)
        {
            try
            {
                var result = await _categoryService.DeleteAsync(categoryId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer[Messages.CATEGORY_DELETE_FAILED]);
                    // NotifyError(result.Message);
                    return View();
                }

                NotifySuccess(_stringLocalizer[Messages.CATEGORY_DELETED_SUCCESS]);
                // NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var errorMessage = "Kategori silinirken bir hata meydana geldi: " + ex.Message;
                NotifyError(errorMessage);
                return View("Error");
            }
        }

        // Belirtilen ID'li kategorinin detaylarını gösterir.
        public async Task<IActionResult> Details(Guid categoryId)
        {
            var result = await _categoryService.GetByIdAsync(categoryId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CATEGORY_NOT_FOUND]);
                // NotifyError(result.Message);
                return View();
            }

            NotifySuccess(_stringLocalizer[Messages.CATEGORY_FOUND_SUCCESS]);
            // NotifySuccess(result.Message);
            return View(result.Data.Adapt<CategoryDetailsVM>());
        }

        // Belirtilen ID'li kategoriyi günceller.
        public async Task<IActionResult> Update(Guid categoryId)
        {

            var result = await _categoryService.GetByIdAsync(categoryId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CATEGORY_UPDATED_FAILED]);
                // NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.CATEGORY_UPDATED_SUCCESS]);
                // NotifySuccess(result.Message);
            }

            var categoryUpdateVM = result.Data.Adapt<CategoryUpdateVM>();
            return View(categoryUpdateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryUpdateVM categoryUpdateVM)
        {

            if (!ModelState.IsValid)
            {
                return View(categoryUpdateVM);
            }

            categoryUpdateVM.Name = StringUtilities.CapitalizeFirstLetter(categoryUpdateVM.Name);

            var result = await _categoryService.UpdateAsync(categoryUpdateVM.Adapt<CategoryUpdateDTO>());

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.CATEGORY_UPDATED_FAILED]);
                // NotifyError(result.Message);
                return View(categoryUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.CATEGORY_UPDATED_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");

        }
    }
}
