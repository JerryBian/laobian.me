﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog;

namespace Laobian.Blog.Models
{
    public class AboutViewModel
    {
        #region System
        public string SystemLastBoot { get; set; }

        public string SystemRunningInterval { get; set; }

        public string SystemDotNetVersion { get; set; }

        public string SystemAppVersion { get; set; }

        #endregion

        #region Post

        public BlogPost LatestPost { get; set; }

        public string PostTotalCount { get; set; }

        public string PostTotalAccessCount { get; set; }

        public IEnumerable<BlogPost> TopPosts { get; set; }

        public string TagTotalCount { get; set; }

        public IDictionary<BlogTag, int> TopTags { get; set; }

        #endregion
    }
}
