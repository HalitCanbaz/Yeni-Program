﻿namespace CrmApp.ViewModel.UserViewModels
{
    public class UserApprovedListViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string NameSurname { get; set; }

        public string Departman { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Status { get; set; }
        public string? Description { get; set; }
    }
}
