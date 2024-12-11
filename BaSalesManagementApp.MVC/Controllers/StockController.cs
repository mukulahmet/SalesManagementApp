using BaSalesManagementApp.Business.Interfaces;
using BaSalesManagementApp.Business.Services;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.StockDTOs;
using BaSalesManagementApp.MVC.Models.StockVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class StockController : BaseController
    {
        private readonly IStockService _stockService;
        private readonly IProductService _productService;
        private readonly IStringLocalizer<StockController> _localizer;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public StockController(IStockService stockService, IProductService productService, IStringLocalizer<StockController> localizer, IStringLocalizer<Resource> stringLocalizer)
        {
            _stockService = stockService;
            _productService = productService;
            _localizer = localizer;
            _stringLocalizer = stringLocalizer;
        }


        /// <summary>
        /// Eldeki tüm stokları listeleyen ana sayfayı döndürür.
        /// </summary>
        /// <returns>Stok listesini gösteren ana sayfa görünümü</returns>
        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical")
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var stock = await _stockService.GetAllAsync(sortOrder);
            var stockListVM = stock.Data.Adapt<List<StockListVM>>();

            if (!stock.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_LIST_FAILED]);
                // NotifyError(_localizer[stock.Message].Value);
                return View(Enumerable.Empty<StockListVM>().ToPagedList(pageNumber, pageSize));
            }
            NotifySuccess(_stringLocalizer[Messages.STOCK_LISTED_SUCCESS]);
            // NotifySuccess(_localizer[stock.Message].Value);
            var paginatedList = stock.Data.Adapt<List<StockListVM>>().ToPagedList(pageNumber, pageSize);

            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentPage"] = pageNumber;
            return View(paginatedList);


        }

        /// <summary>
        /// Yeni bir stok girme sayfasını döndürür.
        /// </summary>
        /// <returns>Yeni bir stok girme sayfası</returns>
        public async Task<IActionResult> Create()
        {
            var productResult = await _productService.GetAllAsync();
            var products = productResult.Data?.Adapt<List<ProductDTO>>() ?? new List<ProductDTO>();

            var stockResult = await _stockService.GetAllAsync();
            var stocks = stockResult.Data?.Adapt<List<StockDTO>>() ?? new List<StockDTO>();

            var productsWithoutStock = products.Where(p => !stocks.Any(s => s.ProductId == p.Id)).ToList();

            var stockCreateVM = new StockCreateVM
            {
                Products = productsWithoutStock
            };

            return View(stockCreateVM);
        }

        /// <summary>
        /// Yeni bir stok girer ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockCreateVM">Girilen stok verileri</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockCreateVM stockCreateVM)
        {
            var productResult = await _productService.GetAllAsync();
            stockCreateVM.Products = productResult.Data?.Adapt<List<ProductDTO>>() ?? new List<ProductDTO>();

            if (!ModelState.IsValid)
            {
                return View(stockCreateVM);
            }
            var stock = await _stockService.AddAsync(stockCreateVM.Adapt<StockCreateDTO>());

            if (!stock.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_CREATE_FAILED]);
                //NotifyError(_localizer[stock.Message].Value);
                return View(stockCreateVM);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.STOCK_CREATED_SUCCESS]);
                //NotifySuccess(_localizer[stock.Message].Value);
            }

            return RedirectToAction("Index");

        }

        /// <summary>
        /// Girilen stok detaylarını gösterir.
        /// </summary>
        /// <param name="stockId">Girilen stok ID'si</param>
        /// <returns>Stok detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid stockId)
        {
            var stock = await _stockService.GetByIdAsync(stockId);

            if (!stock.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_NOT_FOUND]);
                //NotifyError(_localizer[stock.Message].Value);
                return RedirectToAction("Index");
            }

            var stockDetailsVM = stock.Data.Adapt<StockDetailVM>();
            NotifySuccess(_stringLocalizer[Messages.STOCK_FOUND_SUCCESS]);
            // NotifySuccess(stock.Message);

            return View(stockDetailsVM);
        }

        /// <summary>
        /// Belirtilen stok güncelleme sayfasını gösterir.
        /// </summary>
        /// <param name="stockId">Güncellenecek stok ID'si</param>
        /// <returns>Stok güncelleme sayfası</returns>
        public async Task<IActionResult> Update(Guid stockId)
        {
            var stockResult = await _stockService.GetByIdAsync(stockId);
            if (!stockResult.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_UPDATE_FAILED]);
                //NotifyError(_localizer[stockResult.Message].Value);
                return RedirectToAction("Index");
            }
            else
            {
                NotifyError(_stringLocalizer[Messages.STOCK_UPDATE_SUCCESS]);
                // NotifySuccess(_localizer[stockResult.Message].Value);
            }
            var stock = stockResult.Data;

            var productResult = await _productService.GetAllAsync();
            var products = productResult.Data?.Adapt<List<ProductDTO>>() ?? new List<ProductDTO>();

            var stockListResult = await _stockService.GetAllAsync();
            var stocks = stockListResult.Data?.Adapt<List<StockDTO>>() ?? new List<StockDTO>();

            
            var productsWithoutStock = products.Where(p => !stocks.Any(s => s.ProductId == p.Id && s.Id != stockId)).ToList();

            var stockUpdateVM = stock.Adapt<StockUpdateVM>();
            stockUpdateVM.Products = productsWithoutStock;

            return View(stockUpdateVM);
            
        }


        /// <summary>
        ///  Stok bilgilerini günceller.
        /// </summary>
        /// <param name="stockUpdateVM"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(StockUpdateVM stockUpdateVM)
        {
            var productResult = await _productService.GetAllAsync();
            stockUpdateVM.Products = productResult.Data?.Adapt<List<ProductDTO>>() ?? new List<ProductDTO>();
            if (!ModelState.IsValid)
            {
                return View(stockUpdateVM);
            }

            var stock = await _stockService.UpdateAsync(stockUpdateVM.Adapt<StockUpdateDTO>());

            if (!stock.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_UPDATE_FAILED]);
                //NotifyError(_localizer[stock.Message].Value);
                return View(stockUpdateVM);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.STOCK_UPDATE_SUCCESS]);
                //NotifySuccess(_localizer[stock.Message].Value);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen ID'li stoğu siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="stockId">Silinecek stok ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid stockId)
        {
            var stock = await _stockService.DeleteAsync(stockId);

            if (!stock.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.STOCK_DELETE_FAILED]);
                //NotifyError(_localizer[stock.Message].Value);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.STOCK_DELETED_SUCCESS]);
                //NotifySuccess(_localizer[stock.Message].Value);
            }
            return RedirectToAction("Index");

        }
    }
}
