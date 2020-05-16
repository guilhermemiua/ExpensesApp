using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExpensesApp.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        public int type { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}