using System.ComponentModel.DataAnnotations;

namespace CrmApp.ViewModel.AssetFaultViewModels
{
    public class AssetFaultCreateViewModel
    {
        public int Id { get; set; }

        public int AssetId { get; set; }

        public int FaultsId { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        public string AppUserName { get; set; }

    }
}
