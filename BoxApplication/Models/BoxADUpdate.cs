using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoxApplication.Models
{
    public class BoxADUpdate
    {
        [Key]
        public Guid BoxADUpdateID { get; set; }

        [ForeignKey("ActiveDirectoryUserID")]
        public Guid UserID { get; set; }

        [DisplayName("Update Box Account?")]
        public bool UpdateBoxOption { get; set; }

        [DisplayName("Associated User")]
        public ActiveDirectoryUser ADUser { get; set; }
               
        [DisplayName("Field Changed")]
        public string ADFieldChanged { get; set; }

        [DisplayName("Previous Data")]
        public string BoxPreviousData { get; set; }

        [DisplayName("New Data")]
        public string ADNewData { get; set; }
    }
}
