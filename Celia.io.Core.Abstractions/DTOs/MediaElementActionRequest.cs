﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Celia.io.Core.StaticObjects.Abstractions.DTOs
{
    public class MediaElementActionRequest
    {
        [Required]
        public string ObjectId { get; set; }
    }
}
