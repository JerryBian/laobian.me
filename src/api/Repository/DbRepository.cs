﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.SourceProvider;
using Laobian.Api.Store;
using Laobian.Share.Util;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Repository
{
    public class DbRepository : IDbRepository
    {
        private readonly ISourceProvider _sourceProvider;
        private BlogAccessStore _blogAccessStore;
        private BlogMetadataStore _blogMetadataStore;

        private BlogTagStore _blogTagStore;


        public DbRepository(ISourceProviderFactory sourceProviderFactory, IOptions<ApiOption> apiConfig)
        {
            _sourceProvider = sourceProviderFactory.Get(apiConfig.Value.Source);
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.LoadAsync(true, cancellationToken);

            var tags = await _sourceProvider.GetTagsAsync(cancellationToken);
            _blogTagStore = new BlogTagStore(tags);

            var postMetadata = await _sourceProvider.GetPostMetadataAsync(cancellationToken);
            _blogMetadataStore = new BlogMetadataStore(postMetadata);

            var postAccess = await _sourceProvider.GetPostAccessAsync(cancellationToken);
            _blogAccessStore = new BlogAccessStore(postAccess);
        }

        public async Task<BlogTagStore> GetBlogTagStoreAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogTagStore);
        }

        public async Task<BlogMetadataStore> GetBlogMetadataStoreAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult(_blogMetadataStore);
        }

        public async Task<BlogAccessStore> GetBlogAccessStoreAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_blogAccessStore);
        }

        public async Task PersistentAsync(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(
                PersistentBlogAccessStoreAsync(cancellationToken),
                PersistentBlogMetadataAsync(cancellationToken),
                PersistentBlogTagStoreAsync(cancellationToken)
            );
            await _sourceProvider.PersistentAsync(cancellationToken);
        }

        private async Task PersistentBlogTagStoreAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.SaveTagsAsync(
                JsonUtil.Serialize(_blogTagStore.GetAll().OrderByDescending(x => x.LastUpdatedAt), true),
                cancellationToken);
        }

        private async Task PersistentBlogMetadataAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.SavePostMetadataAsync(
                JsonUtil.Serialize(_blogMetadataStore.GetAll().OrderByDescending(x => x.LastUpdateTime), true),
                cancellationToken);
        }

        private async Task PersistentBlogAccessStoreAsync(CancellationToken cancellationToken = default)
        {
            await _sourceProvider.SavePostAccessAsync(
                _blogAccessStore.GetAll().ToDictionary(x => x.Key,
                    x => JsonUtil.Serialize(x.Value.OrderByDescending(y => y.Date), true)), cancellationToken);
        }
    }
}