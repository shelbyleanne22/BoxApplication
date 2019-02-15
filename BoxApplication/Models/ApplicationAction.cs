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
        public Guid ApplicationActionID { get; set; }

        [DisplayName("Action Created By")]
        [ForeignKey("ApplicationActionADForeignKey")]
        public ActiveDirectoryUser ApplicationActionADUser { get; set; }

        public string ApplicationActionADForeignKey { get; set; }

        [DisplayName("Action Type")]
        public string ApplicationActionType { get; set; }

        [DisplayName("Action Description")]
        public string ApplicationActionDescription { get; set; }

        [DisplayName("Object Modified")]
        public string ApplicationActionObjectModified { get; set; }

        [DisplayName("Action Date")]
        public DateTime ApplicationActionDate { get; set; }
    }
}
