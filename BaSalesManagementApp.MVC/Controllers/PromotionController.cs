using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.PromotionDTOs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.PromotionVMs;
using X.PagedList;
using Microsoft.Extensions.Localization;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using Microsoft.EntityFrameworkCore;

namespace BaSalesManagementApp.MVC.Controllers
{
    /// <summary>
    /// PromotionController, IPromotionService bağımlılığını alır ve CRUD işlemlerini gerçekleştirir.
    /// </summary>
    public class PromotionController : BaseController
    {
        private readonly IPromotionService _promotionService;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;

        /// <summary>
        /// PromotionController kurucusu, IPromotionService bağımlılığını alır.
        /// </summary>
        /// <param name="promotionService">Promosyon hizmetini alıyoruz</param>
        public PromotionController(IPromotionService promotionService, IProductService productService, ICompanyService companyService, IStringLocalizer<Resource> stringLocalizer)
        {
            _promotionService = promotionService;
            _productService = productService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;

        }

        /// <summary>
        /// Index action, Promosyon Listesini getirme işlemini gerçekleştirir.
        /// </summary>
        /// <returns>Promosyon Listesini getiren sayfa</returns>
        public async Task<IActionResult> Index(int? page, string sortOrder = "date")
        {
               int pageNumber = page ?? 1;
               int pageSize = 5;
               var result = await _promotionService.GetAllAsync(sortOrder);
               if (!result.IsSuccess)
               {
                NotifyError(_stringLocalizer[Messages.PROMOTION_LISTED_ERROR]);
                // NotifyError(result.Message);
                return View(Enumerable.Empty<PromotionListVM>().ToPagedList(pageNumber, pageSize));
               }
            NotifySuccess(_stringLocalizer[Messages.PROMOTION_LISTED_SUCCESS]);
            //NotifySuccess(result.Message);
            var promotionListVM = result.Data.Adapt<List<PromotionListVM>>();
               var paginatedList = promotionListVM.ToPagedList(pageNumber, pageSize);
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentPage"] = pageNumber;
            return View(paginatedList);
        }

        /// <summary>
        /// Details action, Promosyon detaylarını getirme işlemini gerçekleştirir.
        /// </summary>
        /// <param name="promotionId">Getirilecek promosyonun ID 'si</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(Guid promotionId)
        {
            var result = await _promotionService.GetByIdAsync(promotionId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_GETBYID_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            NotifySuccess(_stringLocalizer[Messages.PROMOTION_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            var promotionDetailsVM = result.Data.Adapt<PromotionDetailsVM>();
            return View(promotionDetailsVM);
        }

        /// <summary>
        /// Create action (HttpGet), yeni bir promosyon oluşturma sayfasını döndürür.
        /// </summary>
        /// <returns>Yeni bir promosyon oluşturma sayfası</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var productResult = await _productService.GetAllAsync();
            var comnpanyResult = await _companyService.GetAllAsync();

            var model = new PromotionCreateVM
            {
                Products = productResult.Data.Adapt<List<Product>>(),
                Companies = comnpanyResult.Data.Adapt<List<Company>>()
            };

            return View(model);
        }

        /// <summary>
        /// Create action, yeni bir promosyon oluşturma işlemini gerçekleştirir.
        /// </summary>
        /// <param name="promotionCreateVM">Oluşturulacak promosyonun verileri</param>
        /// <returns>Promosyon Oluşturma işleminin sonucu</returns>
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromotionCreateVM promotionCreateVM)
        {
            var result = await _promotionService.AddAsync(promotionCreateVM.Adapt<PromotionCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_CREATE_ERROR]);
                //NotifyError(result.Message);
                return View(promotionCreateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.PROMOTION_CREATE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Update action (HttpGet), belirli bir promosyonu güncelleme sayfasını döndürür.
        /// </summary>
        /// <param name="promotionId">Güncellenecek promosyonun ID 'si</param>
        /// <returns>Güncellenecek promosyonu Getirme işleminin sonucu</returns>
        [HttpGet]
        public async Task<IActionResult> Update(Guid promotionId)
        {
            var result = await _promotionService.GetByIdAsync(promotionId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_GETBYID_ERROR]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            NotifySuccess(_stringLocalizer[Messages.PROMOTION_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);

            var productResult = await _productService.GetAllAsync();
            var comnpanyResult = await _companyService.GetAllAsync();
            //var products = productResult.Data?.Adapt<List<ProductDTO>>() ?? new List<ProductDTO>();

            var promotionUpdateVM = result.Data.Adapt<PromotionUpdateVM>();
            promotionUpdateVM.Products = productResult.Data.Adapt<List<ProductDTO>>();
            promotionUpdateVM.Companies = comnpanyResult.Data.Adapt<List<CompanyDTO>>();

            return View(promotionUpdateVM);
        }

        /// <summary>
        /// Update action, belirli bir promosyonu güncelleme işlemini gerçekleştirir.
        /// </summary>
        /// <param name="promotionUpdateVM">Güncellenecek promosyonun verileri</param>
        /// <returns>Promosyon Güncelleme işleminin sonucu</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PromotionUpdateVM promotionUpdateVM)
        {
            var result = await _promotionService.UpdateAsync(promotionUpdateVM.Adapt<PromotionUpdateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_UPDATE_ERROR]);
                //NotifyError(result.Message);
                return View(promotionUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.PROMOTION_UPDATE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete action, Id 'si belirtilen bir promosyonu silme işlemini gerçekleştirir.
        /// </summary>
        /// <param name="promotionId">silinecek promosyonun ID 'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        public async Task<IActionResult> Delete(Guid promotionId)
        {
            var result = await _promotionService.DeleteAsync(promotionId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.PROMOTION_DELETE_ERROR]);
                //NotifyError(result.Message);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.PROMOTION_DELETE_SUCCESS]);
                //NotifySuccess(result.Message);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GetProductsByCompanyId action, Id 'si belirtilen bir şirketin ürünlerini getirme işlemini gerçekleştirir.
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProductsByCompanyId(Guid companyId)
        {
            var result = await _productService.GetAllAsync();
            if (!result.IsSuccess)
            {
                return Json(new List<ProductDTO>());
            }

            var products = result.Data
                .Where(p => p.CompanyId == companyId)
                .ToList();

            return Json(products);
        }
    }
}
