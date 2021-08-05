﻿using System;

namespace Laobian.Share.Util
{
    public static class StringUtil
    {
        public static bool EqualsIgnoreCase(string left, string right)
        {
            return string.Equals(left, right, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}