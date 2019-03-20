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

        [DisplayName("AD Email")]
        [DataType(DataType.EmailAddress)]
        public string ADEmail { get; set; }

        [DisplayName("AD Username")]
        public string ADUsername { get; set; }

        [DisplayName("AD First Name")]
        public string ADFirstName { get; set; }

        [DisplayName("Status")]
        public string ADStatus { get; set; }

        [DisplayName("Date Last Modified")]
        public DateTime ADDateModified { get; set; }

        //equals method for comparing AD and Box Users in context
        public string NeedsUpdate(object ad, object box)
        {
            var activeDirectoryUser = ad as ActiveDirectoryUser;
            var boxUser = box as BoxUsers;

            if (activeDirectoryUser.ADEmail != boxUser.Login)
            {
                return "ADEmail";
            }
            else if (activeDirectoryUser.ADFirstName != boxUser.Name)
            {
                return "ADFirstName";
            }
            else
            {
                return "";
            }

        }
    }

}

