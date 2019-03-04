using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoxApplication.Models
{
    public class BoxUsers
    {
        [Key]
        [DisplayName("ID")]
        public string BoxID { get; set; }

        [DisplayName("Name")]
        public string BoxName { get; set; }

        [DisplayName("Email")]
        public string BoxLogin { get; set; }

        [DisplayName("Space Used")]
        public long BoxSpaceUsed { get; set; }

        [DisplayName("Date Created")]
        public DateTime BoxDateCreated { get; set; }

        [DisplayName("Date Modified")]
        public DateTime BoxDateModified { get; set; }
    }
}
