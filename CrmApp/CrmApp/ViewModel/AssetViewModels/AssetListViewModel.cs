namespace CrmApp.ViewModel.AssetViewModels
{
    public class AssetListViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public string SerialNo { get; set; }

        public string? Description { get; set; }


        public string AssetCategoryName { get; set; }


        public string AssetTypneName { get; set; }


        public string AppUser { get; set; }

        public int AppUserId { get; set; }
    }
}
