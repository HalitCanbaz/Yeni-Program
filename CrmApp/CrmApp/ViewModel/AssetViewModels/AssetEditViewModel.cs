namespace CrmApp.ViewModel.AssetViewModels
{
    public class AssetEditViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }


        public string Name { get; set; }

        public string SerialNo { get; set; }

        public string? Description { get; set; }


        public int AssetCategoryId { get; set; }


        public int AssetTypeId { get; set; }


        public int AppUserId { get; set; }

        public string UserName { get; set; }
    }
}
