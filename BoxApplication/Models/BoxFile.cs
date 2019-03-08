using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoxApplication.Models
{
    public class BoxFile
    {
        [Key]
        public string BoxFileID { get; set; }

        [DisplayName("Object Type")]
        public string BoxFileType { get; set; }

        [DisplayName("File Name")]
        public string BoxFileName { get; set; }

    }
}
