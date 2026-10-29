using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Data;
using MyFirstApi.DTOs;
using MyFirstApi.GenericResponse;
using MyFirstApi.IServices;

namespace MyFirstApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmpController(IEmpServices empServices) : ControllerBase
    {
        [HttpGet("GetAllEmp")]
        public async Task<IActionResult> GetAllEmp()
        {
            try
            {
                var result = await empServices.GetAllEmployeeAsync();

                if(result.Item2== null || !result.Item2.Any())
                {
                    //return Ok(ResponseResult<List<EmployeeDto>>.Failure(null, "No Employee Found"));
                    return NotFound(ResponseResult<List<EmployeeDto>>
                        .Failure(null, "No Employee Found"));
                }
                return Ok(ResponseResult<List<EmployeeDto>>.Success(result.Item2, "Employee Found"));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("CreateEmp")]
        public async Task<IActionResult> CreateEmp([FromBody]EmployeeDto employeeDto)
        {
            try
            {
                var result = await empServices.CreateEmployee(employeeDto);

                if(result.Item1 == 0)
                {
                    return Ok(ResponseResult<string>.Failure(null, result.Item2));
                }

                return Ok(ResponseResult<string>.Success(null, result.Item2));

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut("UpdateEmp")]
        public async Task<IActionResult> UpdateEmp([FromBody] EmployeeDto employeeDto)
        {
            try
            {
                var result = await empServices.UpdateEmployee(employeeDto);

                if (result.Item1 == 0)
                {
                    return Ok(ResponseResult<string>.Failure(null, result.Item2));
                }

                return Ok(ResponseResult<string>.Success(null, result.Item2));

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpDelete("DeleteEmp")]
        public async Task<IActionResult> DeleteEmp(Guid id)
        {
            try
            {
                var result = await empServices.DeleteEmployee(id);

                if (result.Item1 == 0)
                {
                    return Ok(ResponseResult<string>.Failure(null, result.Item2));
                }

                return Ok(ResponseResult<string>.Success(null, result.Item2));

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("GetEmpById")]
        public async Task<IActionResult> GetEmpById([FromBody] Guid id)
        {
            try
            {
                var result = await empServices.GetEmpById(id);

                if(result.Item1 == 0)
                {
                    return Ok(ResponseResult<string>.Failure(null, "Data not found"));
                }
                return Ok(ResponseResult<EmployeeDto>.Success(result.Item2,"Employee find successfulyy"));
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

    }
}
