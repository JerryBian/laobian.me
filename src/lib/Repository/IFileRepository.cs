﻿namespace Laobian.Lib.Repository
{
    public interface IFileRepository
    {
        Task AddAsync(string subFolder, string fileName, byte[] content);
    }
}