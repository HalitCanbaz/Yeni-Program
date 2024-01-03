using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models.Entities
{
    public class Asset
    {
        public int Id { get; set; }


        [StringLength(50)]

        public string Code { get; set; }

        [StringLength(150)]

        public string Name { get; set; }

        [StringLength(150)]

        public string? SerialNo { get; set; }

        [StringLength(500)]

        public string? Description { get; set; }


        public int AssetCategoryId { get; set; }
        public AssetCategory AssetCategory { get; set; }

        public int AssetTypeId { get; set; }
        public AssetType AssetType { get; set; }

        public ICollection<AssetFault> AssetFaults { get; set; } = new HashSet<AssetFault>();

        public int AppUserId { get; set; }


    }
}
