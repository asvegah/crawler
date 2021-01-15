﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.LinkFilters
{
    //Discards mailto: urls
    public class NoMailToFilter : ILinkFilter
    {
        public bool Matches(Uri link)
        {
            return link.Scheme == "mailto";
        }
    }
}
