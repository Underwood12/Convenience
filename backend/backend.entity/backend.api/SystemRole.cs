﻿using Microsoft.AspNetCore.Identity;

using System.ComponentModel;

namespace backend.entity.backend.api
{
    public class SystemRole : IdentityRole
    {
        [Description("成员数量")]
        public int Number { get; set; }

        [Description("备注")]
        public string Remark { get; set; }
    }
}
