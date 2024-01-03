using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models.Entities
{
    public class AssetType
    {
        public int Id { get; set; }

        [StringLength(50)]

        public string Code { get; set; }

        [StringLength(150)]

        public string Name { get; set; }

        [StringLength(500)]

        public string? Description { get; set; }

        public ICollection<Asset> Assets { get; set; } = new HashSet<Asset>();

    }
}
