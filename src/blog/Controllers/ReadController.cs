﻿using System.Collections.Generic;
using System.Linq;
using Laobian.Blog.Cache;
using Laobian.Blog.Models;
using Laobian.Blog.Service;
using Laobian.Share;
using Laobian.Share.Util;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers;

public class ReadController : Controller
{
    private readonly BlogOptions _blogOptions;
    private readonly IBlogService _blogService;
    private readonly ICacheClient _cacheClient;

    public ReadController(IBlogService blogService, ICacheClient cacheClient,
        IOptions<BlogOptions> blogOption)
    {
        _cacheClient = cacheClient;
        _blogService = blogService;
        _blogOptions = blogOption.Value;
    }

    [ResponseCache(CacheProfileName = Constants.CacheProfileName)]
    public IActionResult Index()
    {
        var authenticated = User.Identity?.IsAuthenticated ?? false;
        var model = _cacheClient.GetOrCreate(
            CacheKeyBuilder.Build(nameof(ReadController), nameof(Index), authenticated),
            () =>
            {
                var viewModels = new List<BookItemViewModel>();
                var posts = _blogService.GetAllPosts().Where(x => authenticated || x.Raw.IsPostPublished())
                    .ToList();
                foreach (var item in _blogService.GetBookItems().Where(x => authenticated || x.IsPublished)
                             .GroupBy(x => x.StartTime.Year))
                {
                    var viewModel = new BookItemViewModel
                    {
                        Count = item.Count(),
                        Id = $"year-{item.Key:D4}",
                        Title = $"{item.Key} 年"
                    };

                    foreach (var bookItem in item)
                    {
                        if (!string.IsNullOrEmpty(bookItem.BlogPostLink))
                        {
                            var post = posts.FirstOrDefault(x =>
                                StringUtil.EqualsIgnoreCase(bookItem.BlogPostLink, x.Raw.Link));
                            bookItem.BlogPostLink = post?.Raw.GetFullPath(_blogOptions.BlogRemoteEndpoint);
                            bookItem.BlogPostTitle = post?.Raw.Title;
                        }

                        if (!string.IsNullOrEmpty(bookItem.ShortComment))
                        {
                            bookItem.ShortCommentHtml = Markdown.ToHtml(bookItem.ShortComment);
                        }
                    }

                    viewModel.Items.AddRange(item);
                    viewModels.Add(viewModel);
                }

                return viewModels;
            });

        return View(model);
    }
}