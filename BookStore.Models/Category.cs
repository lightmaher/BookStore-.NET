using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
   public class Category
    {
        public int Id { get; set; }
        [Required]
        [Display( Name ="Category Name")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
