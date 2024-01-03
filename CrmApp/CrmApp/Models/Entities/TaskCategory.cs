using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models.Entities
{
    public class TaskCategory
    {
        public int Id { get; set; }
        [StringLength(50)]

        public string Name { get; set; }


        public ICollection<Work> Works { get; set; }= new HashSet<Work>();
    }
}
