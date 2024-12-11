using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.WarehouseDTOs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class WarehouseController : BaseController
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IBranchService _branchService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public WarehouseController(IWarehouseService warehouseService, IBranchService branchService, IStringLocalizer<Resource> stringLocalizer)
        {
            _warehouseService = warehouseService;
            _branchService = branchService;
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(int? page, string sortOrder = "alphabetical")
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var result = await _warehouseService.GetAllAsync(sortOrder);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_LIST_FAILED]);
                //NotifyError(result.Message);
                return View(Enumerable.Empty<WarehouseListVM>().ToPagedList(pageNumber, pageSize));
            }
            NotifySuccess(_stringLocalizer[Messages.Warehouse_LISTED_SUCCESS]);
            //NotifySuccess(result.Message);
            var paginatedList = result.Data.Adapt<List<WarehouseListVM>>().ToPagedList(pageNumber, pageSize);
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CurrentPage"] = pageNumber;
            return View(paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var branchesResult = await _branchService.GetAllAsync();
            var branches = branchesResult.Data?.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();

            var warehouseCreateVM = new WarehouseCreateVM
            {
                Branches = branches
            };

            return View(warehouseCreateVM);
        }

        /// <summary>
        /// Yeni bir depo oluşturur ve ana sayfaya yönlendirir.      
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(WarehouseCreateVM warehouseCreateVM)
        {
            //var branchesResult = await _branchService.GetAllAsync();
            //warehouseCreateVM.Branches = branchesResult.Data?.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();
            if (!ModelState.IsValid)
            {
                return View(warehouseCreateVM);
            }

            warehouseCreateVM.Name=StringUtilities.CapitalizeEachWord(warehouseCreateVM.Name);
            warehouseCreateVM.Address=StringUtilities.CapitalizeEachWord(warehouseCreateVM.Address);

            var result = await _warehouseService.AddAsync(warehouseCreateVM.Adapt<WarehouseCreateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_CREATE_FAILED]);
                //NotifyError(result.Message);
                return View(warehouseCreateVM);
            }

            NotifySuccess(_stringLocalizer[Messages.Warehouse_CREATED_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen ID'li depoyu siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="warehouseId">Silinecek siparişin ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid warehouseId)
        {
            var result = await _warehouseService.DeleteAsync(warehouseId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_DELETE_FAILED]);
                //NotifyError(result.Message);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.Warehouse_DELETED_SUCCESS]);
                // NotifySuccess(result.Message);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Belirtilen depo bilgilerini alıp güncelleme sayfası oluşturur
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> Update(Guid warehouseId)
        {

            var result = await _warehouseService.GetByIdAsync(warehouseId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var branchesResult = await _branchService.GetAllAsync();
            var branches = branchesResult.Data?.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();

            var warehouseEditVM = result.Data.Adapt<WarehouseUpdateVM>();
            warehouseEditVM.Branches = branches;
            return View(warehouseEditVM);
        }
        /// <summary>
        /// Belirtilen deponun bilgilerini günceller ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="warehouseUpdateVM "></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update(WarehouseUpdateVM warehouseUpdateVM)
        {
            var branchesResult = await _branchService.GetAllAsync();
            warehouseUpdateVM.Branches = branchesResult.Data?.Adapt<List<BranchDTO>>() ?? new List<BranchDTO>();
            if (!ModelState.IsValid)
            {
                return View(warehouseUpdateVM);
            }

            warehouseUpdateVM.Name=StringUtilities.CapitalizeEachWord(warehouseUpdateVM.Name);
            warehouseUpdateVM.Address=StringUtilities.CapitalizeEachWord(warehouseUpdateVM.Address);

            var result = await _warehouseService.UpdateAsync(warehouseUpdateVM.Adapt<WarehouseUpdateDTO>());
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_UPDATE_FAILED]);
                //NotifyError(result.Message);
                return View(warehouseUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.Warehouse_UPDATED_SUCCESS]);
            // NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Girilen stok detaylarını gösterir.
        /// </summary>
        /// <param name="warehouseId">Girilen stok ID'si</param>
        /// <returns>Stok detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid warehouseId)
        {
            var result = await _warehouseService.GetByIdAsync(warehouseId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.Warehouse_GET_FAILED]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var warehouseDetailVM = result.Data.Adapt<WarehouseDetailVM>();
            return View(warehouseDetailVM);
        }

    }


}
