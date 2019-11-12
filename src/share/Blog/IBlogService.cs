﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog
{
    public interface IBlogService
    {
        List<BlogPost> GetPosts(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true);

        BlogPost GetPost(int year, int month, string link, bool onlyPublic = true);

        List<BlogCategory> GetCategories(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true);

        List<BlogTag> GetTags(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true);

        string GetAboutHtml();

        List<BlogArchive> GetArchives(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true);

        Task ReloadAssetsAsync(bool clone = true, bool updateTemplate = true, List<string> addedPosts = null, List<string> modifiedPosts = null);

        Task UpdateAssetsAsync();
    }
}