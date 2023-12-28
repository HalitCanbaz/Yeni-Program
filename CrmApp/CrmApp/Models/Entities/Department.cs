using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string DepartmanName { get; set; }

        public ICollection<AppUser> AppUsers { get; set; } = new HashSet<AppUser>();

    }
}
