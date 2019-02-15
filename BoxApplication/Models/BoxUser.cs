using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoxApplication.Models
{
    public class BoxUser
    {
        [Key]
        [DisplayName("Box ID")]
        public string BoxID { get; set; }

        [ForeignKey("BoxADForeignKey")]
        public ActiveDirectoryUser BoxEmail { get; set; }

        public string BoxADForeignKey { get; set; }

        [DisplayName("Box Name")]
        public string BoxName { get; set; }

        [DisplayName("Box Login")]
        public string BoxLogin { get; set; }

        [DisplayName("Box Space Used")]
        public int BoxSpaceUsed { get; set; }

        [DisplayName("Status")]
        public string BoxStatus { get; set; }

        [DisplayName("Date Created")]
        public DateTime BoxDateCreated { get; set; }

        [DisplayName("Date Modified")]
        public DateTime BoxDateModified { get; set; }
    }
}
