﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Laobian.Share.Extension;

namespace Laobian.Blog.Models
{
    public class PostViewModel
    {
        public BlogPost Current { get; set; }

        public BlogPost Previous { get; set; }

        public BlogPost Next { get; set; }

        public string MetadataHtml { get; private set; }

        public string TagHtml { get; set; }

        public void SetAdditionalInfo()
        {
            var metadata = new List<string>
            {
                $"<span>发表于</span> <time class=\"muted-bolder\" datetime=\"{Current.Metadata.PublishTime.ToDateAndTime()}\" title=\"{Current.Metadata.PublishTime.ToChinaDateAndTime()}\">{Current.PublishTimeString}</time>",
                $"<span class=\"muted-bolder\" title=\"{Current.AccessCount}\">{Current.AccessCountString}</span> <span>次阅读</span>"
            };

            MetadataHtml = string.Join(" &middot; ", metadata);

            var tags = new List<string>();

            foreach (var tag in Current.Tags)
            {
                tags.Add($"<a href='/tag#{tag.Link}' title='{tag.DisplayName}'>{tag.DisplayName}</a>");
            }

            if (tags.Any())
            {
                TagHtml = $"标签：<span>{string.Join(", ", tags)}</span>";
            }
        }
    }
}
