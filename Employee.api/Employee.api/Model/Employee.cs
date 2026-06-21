using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.api.Model
{
    [Table("employeeTbl")]
    public class Employee
    {
        public int employeeId { get; set; }
        [Required, MaxLength(50)]
        public string name { get; set; } = string.Empty;
        [Required, MaxLength(10), MinLength(10)]
        public string contactNo { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public string altContactNo { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public int designationId { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime modifiedDate { get; set; }
    }
}
