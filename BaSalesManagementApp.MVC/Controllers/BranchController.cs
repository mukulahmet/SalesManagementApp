using BaSalesManagementApp.Business.Utilities;
using BaSalesManagementApp.Dtos.BranchDTOs;
using BaSalesManagementApp.Entites.DbSets;
using BaSalesManagementApp.MVC.Models.BranchVMs;
using BaSalesManagementApp.MVC.Models.CompanyVMs;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BaSalesManagementApp.MVC.Controllers
{
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public BranchController(IBranchService branchService, ICompanyService companyService, IStringLocalizer<Resource> stringLocalizer)
        {
            _branchService = branchService;
            _companyService = companyService;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<IActionResult> Index(int? page, string sortOrder)
             
        {
            int pageNumber = page ?? 1;

            int pageSize = 5;
            ViewBag.CurrentSort = sortOrder;

            var result = await _branchService.GetAllAsync();
            var branchListVMs = result.Data.Adapt<List<BranchListVM>>();

            switch (sortOrder)
            {
                case "name_asc":
                    branchListVMs = branchListVMs.OrderBy(x => x.Name).ToList();
                    break;
                case "name_desc":
                    branchListVMs = branchListVMs.OrderByDescending(x => x.Name).ToList();
                    break;
                case "date_asc":
                    branchListVMs = branchListVMs.OrderBy(x => x.CreatedDate).ToList();
                    break;
                case "date_desc":
                    branchListVMs = branchListVMs.OrderByDescending(x => x.CreatedDate).ToList();
                    break;
                default:
                    branchListVMs = branchListVMs.OrderBy(x => x.Name).ToList();
                    break;
            }

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_LISTED_ERROR]);
                //NotifyError(result.Message);
                return View(Enumerable.Empty<BranchListVM>().ToPagedList(pageNumber, pageSize));
            }
            NotifySuccess(_stringLocalizer[Messages.BRANCH_LISTED_SUCCESS]);
            // NotifySuccess(result.Message);
            var paginatedList = branchListVMs.Adapt<List<BranchListVM>>().ToPagedList(pageNumber, pageSize);

            return View(paginatedList);
        }

        public async Task<IActionResult> Details(Guid branchId)
        {
            var result = await _branchService.GetByIdAsync(branchId);

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_GETBYID_ERROR]);
                // NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            NotifySuccess(_stringLocalizer[Messages.BRANCH_GETBYID_SUCCESS]);
            //NotifySuccess(result.Message);
            var branchDetailsVM = result.Data.Adapt<BranchDetailsVM>();

            return View(branchDetailsVM);
        }

        public async Task<IActionResult> Create()
        {
            var result = await _companyService.GetAllAsync();

            var model = new BranchCreateVM()
            {
                Companies = result.Data.Adapt<List<Company>>(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BranchCreateVM branchCreateVM)
        {
           
                branchCreateVM.Name=StringUtilities.CapitalizeFirstLetter(branchCreateVM.Name);
                branchCreateVM.Address=StringUtilities.CapitalizeEachWord(branchCreateVM.Address);

                var result = await _branchService.AddAsync(branchCreateVM.Adapt<BranchCreateDTO>());

                if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_ADD_ERROR]);
                //NotifyError(result.Message);
                return View(branchCreateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.BRANCH_ADD_SUCCESS]);
            //NotifySuccess(result.Message);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(Guid branchId)
        {
            var result = await _branchService.GetByIdAsync(branchId);

            var companyResult = await _companyService.GetAllAsync();

            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_UPDATE_ERROR]);
                // NotifyError(result.Message);
                return RedirectToAction("Index");
            }
            NotifySuccess(_stringLocalizer[Messages.BRANCH_UPDATE_SUCCESS]);
            // NotifySuccess(result.Message);

            var branchUpdateVM = result.Data.Adapt<BranchUpdateVM>();

            branchUpdateVM.Companies = companyResult.Data.Adapt<List<Company>>();

            return View(branchUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(BranchUpdateVM branchUpdateVM)
        {    
                branchUpdateVM.Name = StringUtilities.CapitalizeFirstLetter(branchUpdateVM.Name);      
                branchUpdateVM.Address = StringUtilities.CapitalizeEachWord(branchUpdateVM.Address);      

                var result = await _branchService.UpdateAsync(branchUpdateVM.Adapt<BranchUpdateDTO>());

                if (!result.IsSuccess)
                {
                NotifyError(_stringLocalizer[Messages.BRANCH_UPDATE_ERROR]);
                // NotifyError(result.Message);
                return View(branchUpdateVM);
            }
            NotifySuccess(_stringLocalizer[Messages.BRANCH_UPDATE_SUCCESS]);
            // NotifySuccess(result.Message);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid branchId)
        {
            var result = await _branchService.DeleteAsync(branchId);
            if (!result.IsSuccess)
            {
                NotifyError(_stringLocalizer[Messages.BRANCH_DELETE_ERROR]);
                // NotifyError(result.Message);
            }
            else
            {
                NotifySuccess(_stringLocalizer[Messages.BRANCH_DELETE_SUCCESS]);
                // NotifySuccess(result.Message);
            }

            return RedirectToAction("Index");
        }

    }
}
