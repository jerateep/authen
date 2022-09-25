﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace auth.models.Dto
{
    public class RegisterDto
    {
        [Required]
        public string userName { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string password { get; set; }
        public string  Firstname { get; set; }
        public string  Lastname { get; set; }

    }
}
