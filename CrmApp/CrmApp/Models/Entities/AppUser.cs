using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models.Entities
{
    public class AppUser : IdentityUser<int>
    {
        [StringLength(50)]
        public string NameSurName { get; set; }

        public string? Picture { get; set; }

        public DateTime? RegisterDate { get; set; }

        public bool IsActive { get; set; }


        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

        //[ForeignKey("DepartmanId")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public ICollection<Work> Works { get; set; } = new HashSet<Work>();



    }
}
