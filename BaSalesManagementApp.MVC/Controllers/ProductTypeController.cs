using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.ProductTypeDtos;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.ProductTypeVMs;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class ProductTypeController : BaseController
    {
        private readonly IStockTypeService productTypeService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public ProductTypeController(IStockTypeService productTypeService, IStringLocalizer<Resource> stringLocalizer)
        {
            this.productTypeService = productTypeService;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var result = await productTypeService.GetAllAsync();
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_LISTED_UNSUCCESS]);
                //NotifyError(result.Message);
                return View(Enumerable.Empty<ProductTypeListVM>().ToPagedList(pageNumber, pageSize));
            }


            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_LISTED_SUCCESS]);
            //NotifySuccess(result.Message);
            var paginatedList = result.Data.Adapt<List<ProductTypeListVM>>().ToPagedList(pageNumber, pageSize);
            return View(paginatedList);
        }

        [HttpGet("ProductType/Detail/{productTypeId}")]
        public async Task<IActionResult> Detail(Guid productTypeId)
        {
            var result = await productTypeService.GetByAsync(productTypeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_UNSUCCESS]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            var productTypeDetailVM = result.Data.Adapt<ProductTypeDetailVM>();
            return View(productTypeDetailVM);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductTypeAddVM productTypeAddVM)
        {
            if (!ModelState.IsValid) return View(productTypeAddVM);

            productTypeAddVM.Name=StringUtilities.CapitalizeFirstLetter(productTypeAddVM.Name);

            var result = await productTypeService.AddAsync(productTypeAddVM.Adapt<StockTypeAddDto>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_ADD_UNSUCCESS]);
                //NotifyError(result.Message);
                return View(productTypeAddVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        [HttpGet("ProductType/Delete/{productTypeId}")]
        public async Task<IActionResult> Delete(Guid productTypeId)
        {
            var result = await productTypeService.DeleteAsync(productTypeId);
            if (!result.IsSuccess) NotifyError(result.Message);
            else NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_DELETE_UNSUCCESS]);
            //NotifySuccess(result.Message);

            return RedirectToAction("Index");
        }

        [HttpGet("ProductType/Update/{productTypeId}")]
        public async Task<IActionResult> Update(Guid productTypeId)
        {
            var result = await productTypeService.GetByAsync(productTypeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_UNSUCCESS]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            return View(result.Data.Adapt<ProductTypeUpdateVM>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductTypeUpdateVM productTypeUpdateVM)
        {
            if (!ModelState.IsValid) return View(productTypeUpdateVM);

            productTypeUpdateVM.Name=StringUtilities.CapitalizeFirstLetter(productTypeUpdateVM.Name);

            var result = await productTypeService.UpdateAsync(productTypeUpdateVM.Adapt<StockTypeUpdateDto>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PRODUCTTYPE_UPDATE_UNSUCCESS]);
                //NotifyError(result.Message);
                return View(productTypeUpdateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.PRODUCTTYPE_UPDATE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }
    }
}