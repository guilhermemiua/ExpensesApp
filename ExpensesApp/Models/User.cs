using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExpensesApp.Models
{
    public class User
    {
        public int Id { get; set; }

        private string _Name;

        [Required]
        public string Name
        {
            get { return _Name; }
            set { _Name = value.Trim(); }
        }

        private string _Email;

        [Required]
        public string Email
        {
            get { return _Email; }
            set { _Email = value.Trim(); }
        }

        private string _Password;

        [Required]
        public string Password
        {
            get { return _Password; }
            set { _Password = value.Trim(); }
        }
    }
}