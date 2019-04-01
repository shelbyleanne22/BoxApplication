using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoxApplication.Models
{
    public class ActiveDirectoryUser
    {
        [Key]
        [DisplayName("AD Guid")]
        public Byte[] ADGUID { get; set; }

        //matches BoxUser Login
        [DisplayName("AD Email")]
        [DataType(DataType.EmailAddress)]
        public string ADEmail { get; set; }

        //[DisplayName("AD Username")]
        //public string ADUsername { get; set; }

        [DisplayName("AD Full Name")]
        public string ADFullName { get; set; }

        [DisplayName("Status")]
        public string ADStatus { get; set; }

        [DisplayName("Date Last Modified")]
        public DateTime ADDateModified { get; set; }

    }
}

