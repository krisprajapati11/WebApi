using Microsoft.AspNetCore.Mvc;
using MyFirstApi.DTOs;

namespace MyFirstApi.IServices
{
    public interface IEmpServices
    {
        Task<Tuple<int, List<EmployeeDto>>> GetAllEmployeeAsync();
        Task<Tuple<int, string>> CreateEmployee(EmployeeDto employee);
        Task<Tuple<int, string>> UpdateEmployee(EmployeeDto employee);
        Task<Tuple<int, string>> DeleteEmployee(Guid id);
        Task<Tuple<int, EmployeeDto>> GetEmpById(Guid id);
    }
}
