using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.StockTypeSizeDTOs;
using BaSalesManagementApp.MVC.Models.StockTypeSizeVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class StockTypeSizeController : BaseController
    {

        private readonly IStockTypeSizeService _stockTypeSizeService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;

        public StockTypeSizeController(IStockTypeSizeService stockTypeSizeService, IStringLocalizer<Resource> stringLocalizer)
        {
            _stockTypeSizeService = stockTypeSizeService;
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Tüm Stok Tipi Boyutlarını listeleyen ana sayfa
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(int? page, string sortOrder) 
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;
            ViewBag.CurrentSort=sortOrder;

            var result = await _stockTypeSizeService.GetAllAsync();
            var stockTypeSizeVM=result.Data.Adapt<List<StockTypeSizeListVM>>();

            switch (sortOrder)
                {
                    case "sizeName_asc":
                        stockTypeSizeVM = stockTypeSizeVM.OrderBy(x=>x.Size).ToList();
                        break;
                    case "sizeName_desc":
                        stockTypeSizeVM=stockTypeSizeVM.OrderByDescending(x=>x.Size).ToList();
                        break;
                    case "date_asc":
                        stockTypeSizeVM=stockTypeSizeVM.OrderBy(x=>x.CreatedDate).ToList();
                        break;
                    case "date_desc":
                        stockTypeSizeVM=stockTypeSizeVM.OrderByDescending(x=>x.CreatedDate).ToList();
                        break;
                    default: 
                        stockTypeSizeVM=stockTypeSizeVM.OrderBy(x=>x.Size).ToList();
                        break;
                }

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_LIST_FAILED]);
                // NotifyError(result.Message);
                return View(Enumerable.Empty<StockTypeSizeListVM>().ToPagedList(pageNumber, pageSize));
            }
            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_LISTED_SUCCESS]);
            // NotifySuccess(result.Message);
            var paginatedList = stockTypeSizeVM.Adapt<List<StockTypeSizeListVM>>().ToPagedList(pageNumber, pageSize);
            return View(paginatedList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var stockTypeSizeCreateVM = new StockTypeSizeCreateVM();
            return View(stockTypeSizeCreateVM);
        }

        /// <summary>
        /// Yeni bir Stok Tipi Boyutu oluşturur ve ana sayfaya yönlendirir.      
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(StockTypeSizeCreateVM stockTypeSizeCreateVM)
        {
            if (!ModelState.IsValid)
            {
                return View(stockTypeSizeCreateVM);
            }

            
            stockTypeSizeCreateVM.Size=StringUtilities.CapitalizeEachWord(stockTypeSizeCreateVM.Size);
            stockTypeSizeCreateVM.Description=StringUtilities.CapitalizeEachWord(stockTypeSizeCreateVM.Description);

            var result = await _stockTypeSizeService.AddAsync(stockTypeSizeCreateVM.Adapt<StockTypeSizeCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_CREATE_FAILED]);
                //NotifyError(result.Message);
                return View(stockTypeSizeCreateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_CREATED_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen ID'li Stok Tipi Boyutunu siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockTypeSizeId">Silinecek Stok Tipi Boyutunun ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid stockTypeSizeId)
        {
            var result = await _stockTypeSizeService.DeleteAsync(stockTypeSizeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_DELETE_FAILED]);
                //NotifyError(result.Message);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_DELETED_SUCCESS]);
                //NotifySuccess(result.Message);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen Stok Tipi Boyutu bilgilerini alıp güncelleme sayfası oluşturur
        /// </summary>
        /// <param name="stockTypeSizeId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> Update(Guid stockTypeSizeId)
        {
            var result = await _stockTypeSizeService.GetByIdAsync(stockTypeSizeId);
            if (!result.IsSuccess)
            {

                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_NOT_FOUND]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var stockTypeSizeEditVM = result.Data.Adapt<StockTypeSizeUpdateVM>();
            return View(stockTypeSizeEditVM);
        }

        /// <summary>
        /// Belirtilen Stok Tipi Boyutunun bilgilerini günceller ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockTypeSizeUpdateVM"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update(StockTypeSizeUpdateVM stockTypeSizeUpdateVM)
        {
            if (!ModelState.IsValid)
            {
                return View(stockTypeSizeUpdateVM);
            }

            stockTypeSizeUpdateVM.Size=StringUtilities.CapitalizeEachWord(stockTypeSizeUpdateVM.Size);
            stockTypeSizeUpdateVM.Description=StringUtilities.CapitalizeFirstLetter(stockTypeSizeUpdateVM.Description);

            var result = await _stockTypeSizeService.UpdateAsync(stockTypeSizeUpdateVM.Adapt<StockTypeSizeUpdateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_UPDATE_FAILED]);
                //NotifyError(result.Message);
                return View(stockTypeSizeUpdateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_UPDATE_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Girilen Stok Tipi Boyutu detaylarını gösterir.
        /// </summary>
        /// <param name="stockTypeSizeId">Girilen Stok Tipi Boyutu ID'si</param>
        /// <returns>Stok Tipi Boyutu detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid stockTypeSizeId)
        {
            var result = await _stockTypeSizeService.GetByIdAsync(stockTypeSizeId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_TYPE_SIZE_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var stockTypeSizeDetailVM = result.Data.Adapt<StockTypeSizeDetailVM>();
            NotifySuccess(_stringLocalizer[Messages.STOCK_TYPE_SIZE_FOUND_SUCCESS]);
            return View(stockTypeSizeDetailVM);
        }
    }
}

