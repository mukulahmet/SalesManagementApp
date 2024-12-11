using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.WarehouseDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.ProductVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.Extensions.Localization;
using System.Formats.Tar;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICompanyService _companyService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public ProductController(IProductService productService, ICategoryService categoryService, ICompanyService companyService, IStringLocalizer<Resource> stringLocalizer)
        {
            _productService = productService;
            _categoryService = categoryService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var result = await _productService.GetAllAsync();
                 
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_LISTED_ERROR]);
                //NotifyError(result.Message);
                return View(Enumerable.Empty<ProductListVM>().ToPagedList(pageNumber, pageSize));
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCT_LISTED_SUCCESS]);
            //NotifySuccess(result.Message);
            var paginatedList = result.Data.Adapt<List<ProductListVM>>().ToPagedList(pageNumber, pageSize);
            return View(paginatedList);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categoriesResult = await _categoryService.GetAllAsync();
            var categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            var companyResult = await _companyService.GetAllAsync();
            var companies = companyResult.Data?.Adapt<List<CompanyDTO>>() ?? new List<CompanyDTO>();

            var productCreateVM = new ProductCreateVM
            {
                Categories = categories,
                Companies = companies,
            };

            return View(productCreateVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM productCreateVM)
        {   
            var categoriesResult = await _categoryService.GetAllAsync();
            productCreateVM.Categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();
            if (!ModelState.IsValid)
            {
                return View(productCreateVM);
            }

            productCreateVM.Name=StringUtilities.CapitalizeFirstLetter(productCreateVM.Name);
            
            var result = await _productService.AddAsync(productCreateVM.Adapt<ProductCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_CREATED_ERROR]);
                //NotifyError(result.Message);
                return View(productCreateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.PRODUCT_CREATED_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid productId)
        {
            var result = await _productService.DeleteAsync(productId);

            if (result.IsSuccess)
            {
                NotifySuccess(_stringLocalizer[result.Message]);

            }
            else
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_DELETED_ERROR]);

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(Guid productId)
        {
            var result = await _productService.GetByIdAsync(productId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_NOT_FOUND]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var categoriesResult = await _categoryService.GetAllAsync();
            var categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();

            var companyResult = await _companyService.GetAllAsync();

            var productEditVM = result.Data.Adapt<ProductUpdateVM>();
            productEditVM.Categories = categories;
            productEditVM.Companies = companyResult.Data.Adapt<List<CompanyDTO>>();
            return View(productEditVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM productUpdateVM)
        {
            productUpdateVM.Name=StringUtilities.CapitalizeFirstLetter(productUpdateVM.Name);

            var categoriesResult = await _categoryService.GetAllAsync();
            productUpdateVM.Categories = categoriesResult.Data?.Adapt<List<CategoryDTO>>() ?? new List<CategoryDTO>();
            var companiesResult = await _companyService.GetAllAsync();
            productUpdateVM.Companies = companiesResult.Data?.Adapt<List<CompanyDTO>>() ?? new List<CompanyDTO>();  
            //if (!ModelState.IsValid)
            //{
            //    return View(productUpdateVM);
            //}
            var result = await _productService.UpdateAsync(productUpdateVM.Adapt<ProductUpdateDTO>());
            if (!result.IsSuccess)
            {

                NotifyError(_stringLocalizer[Messages.PRODUCT_UPDATED_ERROR]);
                //NotifyError(result.Message);
                return View(productUpdateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCT_UPDATED_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid productId)
        {
            var result = await _productService.GetByIdAsync(productId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCT_NOT_FOUND]);
                //NotifyError(result.Message);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.PRODUCT_GET_SUCCESS]);
                //NotifySuccess(result.Message);
            }
            return View(result.Data.Adapt<ProductDetailsVM>());
        }
        public async Task<JsonResult> CheckProductInOrder(Guid productId)
        {
            var isInOrder = await _productService.IsProductInOrderAsync(productId);
            return Json(new { isInOrder });
        }
    }
}
