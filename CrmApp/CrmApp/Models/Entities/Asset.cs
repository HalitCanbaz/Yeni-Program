namespace CrmApp.Models.Entities
{
    public class Asset
    {
        public int Id { get; set; }


        public string Code { get; set; }


        public string Name { get; set; }


        public string? Description { get; set; }


        public int AssetCategoryId { get; set; }
        public AssetCategory AssetCategory { get; set; }

        public int AssetTypeId { get; set; }
        public AssetType AssetType { get; set; }

        public ICollection<AssetFault> AssetFaults { get; set; } = new HashSet<AssetFault>();

        public int AppUserId { get; set; }


    }
}
