using System.ComponentModel.DataAnnotations;

namespace CrmApp.Models.Entities
{
    public class Work
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(250)]
        public string? FinishedDescription { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public byte Progress { get; set; }


        [StringLength(50)]
        public string WhoIsCreate { get; set; }

        public DateTime Create { get; set; }

        public DateTime Update { get; set; }

        public DateTime DeadLine { get; set; }

        public DateTime Finished { get; set; }

        public int DepartmentId { get; set; }

        public int WorkOpenDepartmanId { get; set; }


        [StringLength(25)]
        public string WorkOrderNumber { get; set; }


        [StringLength(50)]
        public string? ApprovedNote { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int AssetFaultId { get; set; }
        public AssetFault AssetFault { get; set; }

        public int TaskCategoryId { get; set; }
        public TaskCategory TaskCategory { get; set; }


    }
}
