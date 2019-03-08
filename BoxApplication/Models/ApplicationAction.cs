using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoxApplication.Models
{
    public class ApplicationAction
    {
        [Key]
        [DisplayName("Action ID")]
        public Guid ID { get; set; }

        [DisplayName("Action Description")]
        public string Type { get; set; }

        [DisplayName("Affected Account")]
        [ForeignKey("BoxUsers")]
        public string User { get; set; }

        [DisplayName("Date")]
        public DateTime Date { get; set; }
    }
}
