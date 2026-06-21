using Employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeMasterController : ControllerBase
    {
        public readonly EmployeeDbContext _context;
        public EmployeeMasterController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.Employees.ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                    return NotFound(new { Message = "Employee not found" });
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                bool exists = await _context.Employees
                    .AnyAsync(x => x.contactNo == model.contactNo || x.email == model.email);
                if (exists)
                    return BadRequest(new { Message = "Contact no. or email already exists" });
                model.createdDate = DateTime.Now;
                model.modifiedDate = DateTime.Now;
                _context.Employees.Add(model);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Employee created successfully!", Data = model });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeModel model)
        {
            try
            {
                if (id != model.employeeId)
                    return BadRequest("ID Mismatch");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var existing = await _context.Employees.FindAsync(id);
                if (existing == null)
                    return NotFound(new { Message = "Employee not found" });
                bool exists = await _context.Employees
                    .AnyAsync(x => (x.contactNo == model.contactNo || x.email == model.email)
                    & x.employeeId != id);
                if (exists)
                    return BadRequest(new { Message = "Contact no. or email exists" });

                existing.name = model.name;
                existing.contactNo = model.contactNo;
                existing.altContactNo = model.altContactNo;
                existing.email = model.email;
                existing.city = model.city;
                existing.state = model.state;
                existing.pincode = model.pincode;
                existing.address = model.address;
                existing.designationId = model.designationId;
                existing.designationName = model.designationName;
                existing.modifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Employee updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                    return NotFound(new { Message = "Employee not found" });
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Employee deleted successfully!" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            string? search,
            int? designationId,
            string? sortBy = "name",
            string? sortDir = "asc",
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Employees.AsQueryable();
                // search
                if(!string.IsNullOrEmpty(search))
                {
                    query = query.Where(x =>
                    x.name.Contains(search) ||
                    x.contactNo.Contains(search) ||
                    x.email.Contains(search) ||
                    x.city.Contains(search));
                }
                // filter
                if(designationId.HasValue)
                {
                    query = query.Where(x => x.designationId == designationId);
                }

                // sorting
                switch(sortBy?.ToLower())
                {
                    case "name":
                        query = sortDir == "desc"
                        ? query.OrderByDescending(x => x.name)
                        : query.OrderBy(x => x.name);
                        break;
                    case "createdDate":
                        query = sortDir == "desc"
                        ? query.OrderByDescending(x => x.createdDate)
                        : query.OrderBy(x => x.createdDate);
                        break;
                    deafult:
                        query = query.OrderBy(x => x.employeeId);
                        break;
                }

                //pagination
                int totalRecords = await query.CountAsync();
                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = data
                });

            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
