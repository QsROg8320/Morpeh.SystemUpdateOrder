﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scellecs.Morpeh.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IncludeAttribute : Attribute
    {
        public IncludeAttribute() { }
    }
}
