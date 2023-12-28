namespace CrmApp.Models.Entities
{
    public class TaskCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public ICollection<Work> Works { get; set; }= new HashSet<Work>();
    }
}
