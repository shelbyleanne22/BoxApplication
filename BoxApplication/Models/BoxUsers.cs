﻿using System;
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
        [DisplayName("Box ID")]
        public string ID { get; set; }

        [DisplayName("Active Directroy ID")]
        public string ADGUID { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Email")]
        public string Login { get; set; }

        [DisplayName("Space Used")]
        public long SpaceUsed { get; set; }

        [DisplayName("Date Created")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Date Modified")]
        public DateTime DateModified { get; set; }

        [ForeignKey("ADGUID")]
        public ActiveDirectoryUser Owner { get; set; }
    }
}
