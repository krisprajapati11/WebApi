using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.DTOs;
using MyFirstApi.Entities;
using MyFirstApi.IServices;
using System.Security.Cryptography.Pkcs;

namespace MyFirstApi.Services
{
    public class EmpService(AppDbContext _context) : IEmpServices
    {
        public async Task<Tuple<int, List<EmployeeDto>>> GetAllEmployeeAsync()
        {

            try
            {
                return new Tuple<int, List<EmployeeDto>>(1, await _context.Employees.AsNoTracking().Select(x => new EmployeeDto
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedDate,
                    LastModifiedDate = x.LastModifiedDate,
                    Department = x.Department,
                    DOB = x.DOB,
                    Name = x.Name,
                    EmailAddress = x.EmailAddress,
                    Position = x.Position
                }).ToListAsync());
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<Tuple<int, EmployeeDto>> GetEmpById(Guid id)
        {
            var data = await _context.Employees.AsNoTracking().Select(x => new EmployeeDto
            {
                Id = x.Id,
                CreatedDate = x.CreatedDate,
                LastModifiedDate = x.LastModifiedDate,
                Department = x.Department,
                DOB = x.DOB,
                Name = x.Name,
                EmailAddress = x.EmailAddress,
                Position = x.Position
            }).FirstOrDefaultAsync(x=> x.Id == id);

            if(data == null)
            {
            return new Tuple<int, EmployeeDto>(0, null);
            }
            return new Tuple<int, EmployeeDto>(1, data);
        }

        public async Task<Tuple<int, string>> CreateEmployee(EmployeeDto employee)
        {
            var existing = await _context.Employees.AnyAsync(x => x.EmailAddress == employee.EmailAddress);

            if (existing)
            {
                return new Tuple<int, string>(0, "Employee Already Exist with Same Email Id");
            }

            await _context.Employees.AddAsync(new Entities.Employee
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                LastModifiedDate = null,
                Department = employee.Department,
                DOB = employee.DOB,
                Name = employee.Name,
                EmailAddress = employee.EmailAddress,
                Position = employee.Position
            });

            await _context.SaveChangesAsync();

            return new Tuple<int, string>(1, "Employee Created Successfully");
        }

        public async Task<Tuple<int, string>> UpdateEmployee(EmployeeDto employee)
        {
            if(employee == null)
            {
                return new Tuple<int, string>(0, "Please fill all the details");
            }
            var existing = await _context.Employees.FirstOrDefaultAsync(x => x.EmailAddress == employee.EmailAddress);

            if (existing == null)
            {
                return new Tuple<int, string>(0, "Enter Correct Email Id");
            }


            existing.Department = string.IsNullOrWhiteSpace(employee.Department) ? existing.Department : employee.Department;
            existing.Name = string.IsNullOrWhiteSpace(employee.Name) ? existing.Name : employee.Name;
            existing.DOB = employee.DOB ?? existing.DOB;
            existing.Position = string.IsNullOrWhiteSpace(employee.Position) ? existing.Position : employee.Position;

            _context.Employees.Update(existing);

            await _context.SaveChangesAsync();

            return new Tuple<int, string>(1, "Employee Updated Successfully");
        }

        public async Task<Tuple<int,string>> DeleteEmployee(Guid id)
        {
            var data = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if(data == null)
            {
                return new Tuple<int, string>(0, "Employee not exist with this id");
            }

            _context.Employees.Remove(data);
            await _context.SaveChangesAsync();

            return new Tuple<int, string>(0, "Employee Delteted Succesfully");
        }
    }
}
