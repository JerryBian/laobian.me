﻿using Laobian.Lib.Extension;
using Laobian.Lib.Model;
using Laobian.Lib.Service;
using System.Collections.Concurrent;

namespace Laobian.Lib.Worker
{
    public class BlogPostAccessWorker : IBlogPostAccessWorker
    {
        private readonly ConcurrentQueue<PostAccessItem> _posts;
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogPostAccessWorker> _logger;
        private readonly CancellationTokenSource _cts;

        public BlogPostAccessWorker(IBlogService blogService, ILogger<BlogPostAccessWorker> logger)
        {
            _logger = logger;
            _blogService = blogService;
            _posts = new ConcurrentQueue<PostAccessItem>();
            _cts = new CancellationTokenSource();
        }

        public async void Add(PostAccessItem item)
        {
            if (_cts.IsCancellationRequested)
            {
                _ = await _blogService.AddPostAccessAsync(item.Id, 1);
                return;
            }

            _posts.Enqueue(item);
        }

        public async Task ProcessAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                await ProcessInternalAsync();
                await Task.Delay(TimeSpan.FromHours(0.5)).OkForCancel();
            }
        }

        public async Task StopAsync()
        {
            _cts.Cancel();
            await ProcessInternalAsync();
        }

        private async Task ProcessInternalAsync()
        {
            try
            {
                List<PostAccessItem> items = new();
                while (_posts.TryDequeue(out var v))
                {
                    var lastTimestamp = items.Where(x => x.Id == v.Id && x.Ip == v.Ip).LastOrDefault().Timestamp;
                    if (v.Timestamp - lastTimestamp > TimeSpan.FromMinutes(1))
                    {
                        items.Add(v);
                    }
                    else
                    {
                        _logger.LogWarning($"Discard IP {v.Ip} access count for post {v.Id}. Timestamp {v.Timestamp}, last access {lastTimestamp}.");
                    }
                }

                foreach (var item in items.GroupBy(x => x.Id))
                {
                    _ = await _blogService.AddPostAccessAsync(item.Key, item.Count());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blog post access worker processing failed.");
            }
        }
    }
}
