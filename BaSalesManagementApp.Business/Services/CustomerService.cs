using BaSalesManagementApp.DataAccess.EFCore.Repositories;
using BaSalesManagementApp.DataAccess.Interfaces.Repositories;
using BaSalesManagementApp.Dtos.CategoryDTOs;
using BaSalesManagementApp.Dtos.CompanyDTOs;
using BaSalesManagementApp.Dtos.CustomerDTOs;
using BaSalesManagementApp.Dtos.PromotionDTOs;
using BaSalesManagementApp.Entites.DbSets;

namespace BaSalesManagementApp.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;

        /// <summary>
        /// Müşteri servisi sınıfının yeni bir örneğini başlatır.
        /// </summary>
        /// <param name="customerRepository">Müşteri veri erişim katmanı sınıfı.</param>
        /// <param name="companyRepository">Şirket veri erişim katmanı sınıfı.</param>
        /// <param name="countryRepository">Ülke veri erişim katmanı sınıfı.</param>
        /// <param name="cityRepository">Şehir veri erişim katmanı sınıfı.</param>
        public CustomerService(ICustomerRepository customerRepository, ICompanyRepository companyRepository, ICountryRepository countryRepository, ICityRepository cityRepository)
        {
            _customerRepository = customerRepository;
            _companyRepository = companyRepository;
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
        }

        /// <summary>
        /// Belirtilen sıralama kriterine göre müşteri listesini alır ve sıralar.
        /// </summary>
        /// <param name="sortOrder">Sıralama kriterleri: "date" (tarih, azalan), "datedesc" (tarih, artan), "alphabetical" (alfabetik, artan), "alphabeticaldesc" (alfabetik, azalan).</param>
        /// <returns>Sıralanmış müşteri listesini ve şirket isimlerini içeren bir görev döndürür. İşlem başarısız olursa hata mesajı döner.</returns>

        public async Task<IDataResult<List<CustomerListDTO>>> GetAllAsync(string sortOrder)
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var companies = await _companyRepository.GetAllAsync();
                var cities = await _cityRepository.GetAllAsync();
                var countries = await _countryRepository.GetAllAsync();

                var customerListDTOs = customers.Adapt<List<CustomerListDTO>>() ?? new List<CustomerListDTO>();
                
                foreach (var customer in customerListDTOs)
                {
                    var company = companies.FirstOrDefault(x => x.Id == customer.CompanyId);
                    if (company != null)
                        customer.CompanyName = company.Name;

                    var city = cities.FirstOrDefault(x => x.Id == customer.CityId);
                    if (city != null)
                        customer.CityName = city.Name;

                    var country = countries.FirstOrDefault(x => x.Id == customer.CountryId);
                    if (country != null)
                        customer.CountryName = country.Name;
                }

               
                // Sıralama işlemi
                switch (sortOrder.ToLower())
                {
                    case "date":
                        customerListDTOs = customerListDTOs.OrderByDescending(c => c.CreatedDate).ToList();
                        break;
                    case "datedesc":
                        customerListDTOs = customerListDTOs.OrderBy(c => c.CreatedDate).ToList();
                        break;
                    case "alphabetical":
                        customerListDTOs = customerListDTOs.OrderBy(c => c.Name).ToList();
                        break;
                    case "alphabeticaldesc":
                        customerListDTOs = customerListDTOs.OrderByDescending(c => c.Name).ToList();
                        break;
                }

                return new SuccessDataResult<List<CustomerListDTO>>(customerListDTOs, Messages.CUSTOMER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CustomerListDTO>>(Messages.CUSTOMER_LIST_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Yeni bir Müşteri ekler.
        /// Şirket, şehir ve ülke bilgileri doğrulanır; geçerli değilse hata mesajı döndürülür.
        /// </summary>
        /// <param name="customerCreateDTO">Eklenmek istenen müşteriyle ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda eklenen müşterilerin verilerini içerir.</returns>
        public async Task<IDataResult<CustomerDTO>> AddAsync(CustomerCreateDTO customerCreateDTO)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(customerCreateDTO.CompanyId);
                var city = await _cityRepository.GetByIdAsync(customerCreateDTO.CityId);
                var country = await _countryRepository.GetByIdAsync(customerCreateDTO.CountryId);

                if (company == null || city == null || country == null)
                {
                    return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_ADD_ERROR);
                }

                var customer = customerCreateDTO.Adapt<Customer>();
                await _customerRepository.AddAsync(customer);
                await _customerRepository.SaveChangeAsync();

                return new SuccessDataResult<CustomerDTO>(customer.Adapt<CustomerDTO>(), Messages.CUSTOMER_ADD_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_ADD_ERROR + ex.Message);
            }
        }

        /// <summary>
        /// Benzersiz kimliğiyle bir müşteriyi siler.
        /// </summary>
        /// <param name="customerId">Silinmek istenen müşterinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda silme işleminin başarılı olup olmadığını belirtir.</returns>
        public async Task<IResult> DeleteAsync(Guid customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return new ErrorResult(Messages.CUSTOMER_NOT_FOUND);
                }

                await _customerRepository.DeleteAsync(customer);
                await _customerRepository.SaveChangeAsync();

                return new SuccessResult(Messages.CUSTOMER_DELETE_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorResult(Messages.CUSTOMER_DELETE_ERROR + ex.Message);
            }
        }

        /// <summary>
        /// Tüm müşterileri alır ve her müşterinin şirket, şehir ve ülke bilgilerini ekler.
        /// </summary>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda tüm müşterilerin listesini içerir.</returns>
        public async Task<IDataResult<List<CustomerListDTO>>> GetAllAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var companies = await _companyRepository.GetAllAsync();
                var cities = await _cityRepository.GetAllAsync();
                var countries = await _countryRepository.GetAllAsync();
                var customerListDTOs = customers.Adapt<List<CustomerListDTO>>() ?? new List<CustomerListDTO>();

                if (customers == null || customers.Count() == 0)
                {
                    return new ErrorDataResult<List<CustomerListDTO>>(customerListDTOs, Messages.CUSTOMER_LIST_EMPTY);
                }

                foreach (var customer in customerListDTOs)
                {
                    var company = companies.FirstOrDefault(x => x.Id == customer.CompanyId);
                    var city = cities.FirstOrDefault(x => x.Id == customer.CityId);
                    var country = countries.FirstOrDefault(x => x.Id == customer.CountryId);

                    if (company != null) customer.CompanyName = company.Name;
                    if (city != null) customer.CityName = city.Name;
                    if (country != null) customer.CountryName = country.Name;
                }

                return new SuccessDataResult<List<CustomerListDTO>>(customerListDTOs, Messages.CUSTOMER_LISTED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CustomerListDTO>>(Messages.CUSTOMER_LIST_FAILED + ex.Message);
            }
        }

        /// <summary>
        /// Benzersiz kimliğiyle bir müşteri alır.
        /// </summary>
        /// <param name="customerId">Alınmak istenen müşterinin benzersiz kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda müşteri verilerini içerir, bulunamazsa null döner.</returns>
        public async Task<IDataResult<CustomerDTO>> GetByIdAsync(Guid customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_NOT_FOUND);
                }

                var customerDTO = customer.Adapt<CustomerDTO>();
                var company = await _companyRepository.GetByIdAsync(customerDTO.CompanyId);

                if (company != null)
                    customerDTO.CompanyName = company.Name;

                return new SuccessDataResult<CustomerDTO>(customer.Adapt<CustomerDTO>(), Messages.CUSTOMER_FOUND_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_GET_FAILED + ex.Message);
            }
        }


        /// <summary>
        /// Bir müşteriyi günceller.
        /// Müşteri, şirket, şehir ve ülke bilgileri doğrulanır; geçerli değilse hata mesajı döndürülür.
        /// </summary>
        /// <param name="customerUpdateDTO">Güncellenmiş müşteriyle ilgili bilgileri içeren veri transfer nesnesi.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucunda güncellenmiş müşteri verilerini içerir.</returns>
        public async Task<IDataResult<CustomerDTO>> UpdateAsync(CustomerUpdateDTO customerUpdateDTO)
        {
            try
            {
                var oldCustomer = await _customerRepository.GetByIdAsync(customerUpdateDTO.Id);
                if (oldCustomer == null)
                {
                    return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_NOT_FOUND);
                }

                var company = await _companyRepository.GetByIdAsync(customerUpdateDTO.CompanyId);
                if (company == null)
                {
                    return new ErrorDataResult<CustomerDTO>(Messages.COMPANY_GETBYID_ERROR);
                }

                var city = await _cityRepository.GetByIdAsync(customerUpdateDTO.CityId);
                var country = await _countryRepository.GetByIdAsync(customerUpdateDTO.CountryId);

                customerUpdateDTO.Adapt(oldCustomer);
                await _customerRepository.UpdateAsync(customerUpdateDTO.Adapt(oldCustomer));
                await _customerRepository.SaveChangeAsync();

                return new SuccessDataResult<CustomerDTO>(oldCustomer.Adapt<CustomerDTO>(), Messages.CUSTOMER_UPDATED_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CustomerDTO>(Messages.CUSTOMER_UPDATED_FAILED);
            }
        }
    }
}
