using System.ComponentModel.DataAnnotations;

namespace CrmApp.ViewModel.WorkViewModel
{
    public class WorkPendingApprovalDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string WhoIsCreate { get; set; }

        public string Status { get; set; }

        public DateTime Create { get; set; }

        public DateTime Update { get; set; }

        public DateTime DeadLine { get; set; }

        public string AppUser { get; set; }

        public int AppUserId { get; set; }

        public string AssetFault { get; set; }

        public string TaskCategory { get; set; }

        public string WorkOrderNumber { get; set; }

        public string ApprovedNote { get; set; }


        public string OnaylaReddet { get; set; }

    }
}
