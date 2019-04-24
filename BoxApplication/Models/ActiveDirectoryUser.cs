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

        [DisplayName("AD Full Name")]
        public string ADFullName { get; set; }

        [DisplayName("Status")]
        public string ADStatus { get; set; }

        [DisplayName("Date Last Modified")]
        public DateTime ADDateModified { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                ActiveDirectoryUser aduser = obj as ActiveDirectoryUser;
                if ((aduser.ADGUID.SequenceEqual(this.ADGUID) &&
                    aduser.ADEmail == this.ADEmail &&
                    aduser.ADFullName == this.ADFullName &&
                    aduser.ADDateModified.Equals(this.ADDateModified) &&
                    aduser.ADStatus == this.ADStatus))
                    return true;
                else
                    return false;
            }
        }
    }
}

