using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models.Entities
{
    public class AssetFault
    {
        public int Id { get; set; }

        public int AssetId { get; set; }

        public Asset Asset { get; set; }


        public int FaultId { get; set; }
        public Fault Fault { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        public ICollection<Work> Works { get; set; } = new HashSet<Work>();



    }
}
