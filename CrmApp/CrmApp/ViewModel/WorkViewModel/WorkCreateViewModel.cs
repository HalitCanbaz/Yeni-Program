using CrmApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace CrmApp.ViewModel.WorkViewModel
{
    public class WorkCreateViewModel
    {

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } = "beklemede";

        public byte Progress { get; set; } = 0;

        public string WhoIsCreate { get; set; }

        public DateTime Create { get; set; } = DateTime.Now;

        //public DateTime Update { get; set; }

        public DateTime DeadLine { get; set; }

        //public DateTime Finished { get; set; }

        public int DepartmentId { get; set; }

        public int WorkOpenDepartmanId { get; set; }

        public string WorkOrderNumber { get; set; }

        //public string? ApprovedNote { get; set; }

        public int AppUserId { get; set; }

        public int AssetFaultId { get; set; }

        public int TaskCategoryId { get; set; }


    }
}
