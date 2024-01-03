namespace CrmApp.ViewModel.AssetViewModels
{
    public class AssetCreateViewModel
    {
        public string Code { get; set; }


        public string Name { get; set; }

        public string SerialNo { get; set; }

        public string? Description { get; set; }


        public int AssetCategoryId { get; set; }


        public int AssetTypeId { get; set; }


        public int AppUserId { get; set; }
    }
}
