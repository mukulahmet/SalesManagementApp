using BaSalesManagementApp.Dtos.AdminDTOs;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.OrderDetailDTOs;
using BaSalesManagementApp.Dtos.OrderDTOs;
using BaSalesManagementApp.Dtos.ProductDTOs;
using BaSalesManagementApp.Dtos.WarehouseDTOs;
using BaSalesManagementApp.MVC.Models.CategoryVMs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using BaSalesManagementApp.MVC.Models.OrderVMs;
using BaSalesManagementApp.MVC.Models.WarehouseVMs;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly IAdminService _adminService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;

        /// <summary>
        /// OrderController kurucusu, IOrderService bağımlılığını alır.
        /// </summary>
        /// <param name="orderService">Sipariş hizmeti</param>
        public OrderController(IOrderService orderService, IProductService productService, ICompanyService companyService, IAdminService adminService, IStringLocalizer<Resource> stringLocalizer)
        {
            _orderService = orderService;
            _productService = productService;
            _companyService = companyService;
            _adminService = adminService;
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Tüm siparişleri listeleyen ana sayfa görünümünü döndürür.
        /// </summary>
        /// <returns>Sipariş listesini gösteren ana sayfa görünümü</returns>
        public async Task<IActionResult> Index(int? page, string sortOrder = "date")
        {
            try
            {
                int pageNumber = page ?? 1;
                int pageSize = 5;

                var result = await _orderService.GetAllAsync(sortOrder);
                if (!result.IsSuccess)
                {

                    NotifyError(_stringLocalizer["ORDER_LIST_FAILED"]);
                    // NotifyError(result.Message);
                    return View(Enumerable.Empty<OrderListVM>().ToPagedList(pageNumber, pageSize));
                }

                var orderListVM = result.Data.Select(order => new OrderListVM
                {
                    ProductName= order.ProductName,
                    Id = order.Id,
                    Quantity = order.Quantity,
                    TotalPrice = order.TotalPrice,
                    OrderDate = order.OrderDate,
                    AdminName = order.AdminName // Admin adını ekleyelim
                }).ToList();

                var paginatedList = orderListVM.ToPagedList(pageNumber, pageSize);
                NotifySuccess(_stringLocalizer["ORDER_LISTED_SUCCESS"]);
                return View(paginatedList);
            }
            catch (Exception ex)
            {
                NotifySuccess(_stringLocalizer["ORDER_LISTED_SUCCESS"]);
                // NotifyError(ex.Message);
                return View("Error");
            }
        }

        /// <summary>
        /// Yeni bir sipariş oluşturma sayfasını döndürür.
        /// </summary>
        /// <returns>Yeni bir sipariş oluşturma sayfası</returns>
        public async Task<IActionResult> Create()
        {
            try
            {
                var productsResult = await _productService.GetAllAsync();
                var orderCreateVM = new OrderCreateVM
                {
                    Products = productsResult.Data
                };
                return View(orderCreateVM);
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer["ORDER_CREATE_FAILED"] + ": " + ex.Message);
                //NotifyError(ex.Message);
                return View("Error");
            }
        }

        /// <summary>
        /// Siparişi oluşturan adminin bilgileriyle birlikte bir siparişi oluşturur ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="orderCreateVM">Oluşturulacak siparişin verileri</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateVM orderCreateVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    orderCreateVM.Products = (await _productService.GetAllAsync()).Data ?? new List<ProductListDTO>();
                    return View(orderCreateVM);
                }

                var productResult = await _productService.GetByIdAsync(orderCreateVM.ProductId);
                var product = productResult.Data;

                // Doğru claim türünü kullanarak IdentityId'yi alın
                var identityIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
                var identityId = identityIdClaim?.Value;

                if (string.IsNullOrEmpty(identityId))
                {
                    throw new Exception(_stringLocalizer[Messages.IDENTITY_ID_NOT_DETERMINED]);
                }

                var orderDto = orderCreateVM.Adapt<OrderCreateDTO>(); // Mapster ile dönüştürülme
                orderDto.Id = Guid.NewGuid(); // Yeni bir orderId oluşturma
                orderDto.ProductId = orderCreateVM.ProductId;
                orderDto.OrderDetails = new List<OrderDetailCreateDTO>
            {
                new OrderDetailCreateDTO
                {
                    ProductId = orderCreateVM.ProductId,
                    ProductName = product.Name,
                    Quantity = orderCreateVM.Quantity,
                    UnitPrice = product.Price,
                    Discount = orderCreateVM.DiscountRate
                }
            };

                var result = await _orderService.AddAsync(orderDto, identityId); // AddAsync metodunu iki parametre ile çağırın

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_CREATE_FAILED"]);
                    //NotifyError(result.Message);
                    orderCreateVM.Products = (await _productService.GetAllAsync()).Data ?? new List<ProductListDTO>();
                    return View(orderCreateVM);
                }

                NotifySuccess(_stringLocalizer["ORDER_CREATED_SUCCESS"]);
                //NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                NotifyError(_stringLocalizer["ORDER_CREATE_FAILED"] + ": " + ex.Message);
                //NotifyError($"An error occurred: {detailedMessage}");
                return View("Error");
            }
        
    }

        /// <summary>
        /// Ürün silinmiş olsa bile belirtilen siparişin detaylarını gösterir. 
        /// </summary>
        /// <param name="orderId">Gösterilecek siparişin ID'si</param>
        /// <returns>Sipariş detaylarının görüntülendiği sayfa</returns>
        public async Task<IActionResult> Details(Guid orderId)
        {
            try
            {
                var result = await _orderService.GetByIdAsync(orderId);
                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_NOT_FOUND"]);
                    //NotifyError(result.Message);
                    return RedirectToAction("Index");
                }

                
                var adminResult = await _adminService.GetByIdAsync(result.Data.AdminId);
                var adminDTO = adminResult?.Data ?? new AdminDTO { FirstName = "Bilinmeyen", LastName = "Admin" }; 

                
                var productResult = await _productService.GetByIdAsync(result.Data.ProductId);
                var productDTO = productResult?.Data ?? new ProductDTO { Name = "Silinmiş Ürün" }; 

                
                var companyResult = productResult?.Data != null ? await _companyService.GetByIdAsync(productDTO.CompanyId) : null;
                var companyDTO = companyResult?.Data;

                // OrderDetailsVM'i oluştur ve değerleri ata
                var orderDetailsVM = result.Data.Adapt<OrderDetailsVM>();
                orderDetailsVM.Admin = adminDTO;
                orderDetailsVM.Company = companyDTO;
                orderDetailsVM.ProductName = productDTO.Name;

                NotifySuccess(_stringLocalizer["ORDER_FOUND_SUCCESS"]);
                // NotifySuccess(result.Message);
                return View(orderDetailsVM);
                return View(orderDetailsVM);
            }
            catch (Exception ex)
            {
                var detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                NotifyError($"An error occurred: {detailedMessage}");
                return View("Error");
            }
        }

        /// <summary>
        /// Belirtilen siparişin güncelleme sayfasını gösterir.
        /// </summary>
        /// <param name="orderId">Güncellenecek siparişin ID'si</param>
        /// <returns>Sipariş güncelleme sayfası</returns>
        public async Task<IActionResult> Update(Guid orderId)
        {
            var result = await _orderService.GetByIdAsync(orderId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer["ORDER_NOT_FOUND"]);
                //NotifyError(result.Message);
                return RedirectToAction("Index");
            }

            var orderUpdateVM = result.Data.Adapt<OrderUpdateVM>();
            orderUpdateVM.Products = (await _productService.GetAllAsync()).Data;

            return View(orderUpdateVM);
        }

        /// <summary>
        ///  Sipariş bilgilerini günceller.
        /// </summary>
        /// <param name="orderUpdateVM"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(OrderUpdateVM orderUpdateVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    orderUpdateVM.Products = (await _productService.GetAllAsync()).Data;
                    return View(orderUpdateVM);
                }

                var productResult = await _productService.GetByIdAsync(orderUpdateVM.ProductId);

                if (!productResult.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_UPDATE_FAILED"]);
                    //NotifyError(productResult.Message)
                    orderUpdateVM.Products = (await _productService.GetAllAsync()).Data;
                    return View(orderUpdateVM);
                }

                var product = productResult.Data;

                var orderUpdateDto = new OrderUpdateDTO
                {
                    Id = orderUpdateVM.Id,
                    Quantity = orderUpdateVM.Quantity,
                    TotalPrice = orderUpdateVM.TotalPrice,
                    OrderDate = orderUpdateVM.OrderDate,
                    OrderDetails = new List<OrderDetailUpdateDTO>
            {
                new OrderDetailUpdateDTO
                {
                    OrderId = orderUpdateVM.Id,
                    ProductId = orderUpdateVM.ProductId,
                    Quantity = orderUpdateVM.Quantity,
                    UnitPrice = product.Price,
                    Discount = orderUpdateVM.DiscountRate
                }
            }
                };

                var result = await _orderService.UpdateAsync(orderUpdateDto);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_UPDATE_FAILED"]);
                    //NotifyError(result.Message);
                    orderUpdateVM.Products = (await _productService.GetAllAsync()).Data;
                    return View(orderUpdateVM);
                }


                NotifySuccess(_stringLocalizer["ORDER_UPDATE_SUCCES"]);
                // NotifySuccess(result.Message);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer["ORDER_UPDATE_FAILED"] + ": " + ex.Message);
                //NotifyError(ex.Message);
                return View("Error");
            }
        }

        /// <summary>
        /// Belirtilen ID'li siparişi siler ve ana sayfaya yönlendirir.
        /// </summary>
        /// <param name="orderId">Silinecek siparişin ID'si</param>
        /// <returns>Ana sayfaya yönlendirme</returns>
        public async Task<IActionResult> Delete(Guid orderId)
        {
            try
            {
                var result = await _orderService.DeleteAsync(orderId);

                if (!result.IsSuccess)
                {
                    NotifyError(_stringLocalizer["ORDER_DELETE_FAILED"]);
                    //NotifyError(result.Message);
                }
                else
                {
                    NotifySuccess(_stringLocalizer["ORDER_DELETED_SUCCESS"]);
                    //NotifySuccess(result.Message);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                NotifyError(_stringLocalizer["ORDER_DELETE_FAILED"] + ": " + ex.Message);
                //NotifyError(ex.Message);
                return View("Error");
            }
        }
    }
}
