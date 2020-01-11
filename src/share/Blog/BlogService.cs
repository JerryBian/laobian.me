﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog.Alert;
using Laobian.Share.Blog.Asset;
using Laobian.Share.Blog.Model;
using Laobian.Share.Helper;
using Microsoft.Extensions.Logging;

namespace Laobian.Share.Blog
{
    public class BlogService : IBlogService
    {
        private readonly IBlogAlertService _blogAlertService;
        private readonly IBlogAssetManager _blogAssetManager;
        private readonly ILogger<BlogService> _logger;
        private readonly SemaphoreSlim _semaphoreSlim1;
        private readonly SemaphoreSlim _semaphoreSlim2;

        public BlogService(ILogger<BlogService> logger, IBlogAssetManager blogAssetManager,
            IBlogAlertService blogAlertService)
        {
            _logger = logger;
            _blogAssetManager = blogAssetManager;
            _blogAlertService = blogAlertService;
            _semaphoreSlim1 = new SemaphoreSlim(1, 1);
            _semaphoreSlim2 = new SemaphoreSlim(1, 1);
        }

        public List<BlogPost> GetPosts(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true)
        {
            IEnumerable<BlogPost> posts = _blogAssetManager.GetAllPosts();
            posts = onlyPublic ? posts.Where(p => p.IsPublic) : posts;
            posts = publishTimeDesc
                ? posts.OrderByDescending(p => p.PublishTime)
                : posts.OrderBy(p => p.Metadata.PublishTime);
            posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
            return posts.ToList();
        }

        public BlogPost GetPost(int year, int month, string link, bool onlyPublic = true)
        {
            IEnumerable<BlogPost> posts = _blogAssetManager.GetAllPosts();
            var post = onlyPublic
                ? posts.FirstOrDefault(p =>
                    p.IsPublic && p.PublishTime.Year == year && p.PublishTime.Month == month &&
                    CompareHelper.IgnoreCase(p.Link, link))
                : posts.FirstOrDefault(p =>
                    p.PublishTime.Year == year && p.PublishTime.Month == month &&
                    CompareHelper.IgnoreCase(p.Link, link));
            return post;
        }

        public List<BlogCategory> GetCategories(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true)
        {
            var categories = new List<BlogCategory>();
            foreach (var blogCategory in _blogAssetManager.GetAllCategories())
            {
                var category = new BlogCategory
                {
                    Link = blogCategory.Link,
                    Name = blogCategory.Name
                };

                IEnumerable<BlogPost> posts = new List<BlogPost>(blogCategory.Posts);
                if (onlyPublic)
                {
                    posts = posts.Where(p => p.IsPublic);
                }

                posts = publishTimeDesc
                    ? posts.OrderByDescending(p => p.PublishTime)
                    : posts.OrderBy(p => p.PublishTime);

                posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
                category.Posts.AddRange(posts);

                if (category.Posts.Any())
                {
                    categories.Add(category);
                }
            }

            return categories;
        }

        public List<BlogTag> GetTags(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true)
        {
            var tags = new List<BlogTag>();
            foreach (var blogTag in _blogAssetManager.GetAllTags())
            {
                var tag = new BlogTag
                {
                    Link = blogTag.Link,
                    Name = blogTag.Name
                };

                IEnumerable<BlogPost> posts = new List<BlogPost>(blogTag.Posts);
                if (onlyPublic)
                {
                    posts = posts.Where(p => p.IsPublic);
                }

                posts = publishTimeDesc
                    ? posts.OrderByDescending(p => p.PublishTime)
                    : posts.OrderBy(p => p.PublishTime);

                posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
                tag.Posts.AddRange(posts);

                if (tag.Posts.Any())
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }

        public string GetAboutHtml()
        {
            return _blogAssetManager.GetAboutHtml();
        }

        public List<BlogArchive> GetArchives(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true)
        {
            var archives = new List<BlogArchive>();
            foreach (var item in _blogAssetManager.GetAllPosts().ToLookup(p => p.PublishTime.Year))
            {
                var archive = new BlogArchive(item.Key);
                IEnumerable<BlogPost> posts = new List<BlogPost>(item);
                if (onlyPublic)
                {
                    posts = posts.Where(p => p.IsPublic);
                }

                posts = publishTimeDesc
                    ? posts.OrderByDescending(p => p.PublishTime)
                    : posts.OrderBy(p => p.PublishTime);

                posts = toppingPostsFirst ? posts.OrderBy(p => p.IsTopping ? 0 : 1) : posts;
                archive.Posts.AddRange(posts);

                if (archive.Posts.Any())
                {
                    archives.Add(archive);
                }
            }

            return archives;
        }

        public async Task ReloadLocalAssetsAsync(
            bool clone = true,
            List<string> addedPosts = null,
            List<string> modifiedPosts = null)
        {
            try
            {
                await _semaphoreSlim1.WaitAsync();
                if (clone)
                {
                    await _blogAssetManager.RemoteGitToLocalFileAsync();
                }

                var existingPosts = _blogAssetManager.GetAllPosts().ToList();
                var reloadResult = await _blogAssetManager.LocalFileToLocalMemoryAsync();
                if (reloadResult)
                {
                    BlogState.AssetLastUpdate = DateTime.Now;
                    foreach (var item in _blogAssetManager.GetAllPosts())
                    {
                        var existingPost =
                            existingPosts.FirstOrDefault(p => CompareHelper.IgnoreCase(p.Link, item.Link));
                        if (existingPost != null)
                        {
                            item.AccessCount = Math.Max(item.AccessCount,
                                existingPost.AccessCount);
                        }
                    }

                    var postsPublishTime = new List<DateTime>();
                    foreach (var blogPost in _blogAssetManager.GetAllPosts())
                    {
                        var rawPublishTime = blogPost.GetRawPublishTime();
                        if (rawPublishTime.HasValue && rawPublishTime != default(DateTime))
                        {
                            postsPublishTime.Add(rawPublishTime.Value);
                        }
                    }

                    BlogState.PostsPublishTime = postsPublishTime.OrderBy(p => p);
                }

                if (addedPosts != null && addedPosts.Any() || modifiedPosts != null && modifiedPosts.Any())
                {
                    if (modifiedPosts != null)
                    {
                        var posts = _blogAssetManager.GetAllPosts().Where(p =>
                            modifiedPosts.FirstOrDefault(mp => CompareHelper.IgnoreCase(mp, p.GitPath)) != null);
                        foreach (var blogPost in posts)
                        {
                            blogPost.Metadata.LastUpdateTime = DateTime.Now;
                        }
                    }

                    await UpdateRemoteAssetsAsync();
                }

                _logger.LogInformation("Reload local and memory assets successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reload local and memory assets throws error.");
                await _blogAlertService.AlertEventAsync("Reload local and memory assets throws error.", ex);
            }
            finally
            {
                _semaphoreSlim1.Release();
            }
        }

        public async Task UpdateRemoteAssetsAsync()
        {
            try
            {
                await _semaphoreSlim2.WaitAsync();
                await _blogAssetManager.LocalMemoryToLocalFileAsync();
                await _blogAssetManager.LocalFileToRemoteGitAsync();
                _logger.LogInformation("Update remote store successfully.");
            }
            finally
            {
                _semaphoreSlim2.Release();
            }
        }
    }
}