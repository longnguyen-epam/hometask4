using CatalogService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Tests.Integration
{
    public sealed class CatalogDbFixture : IAsyncLifetime
    {
        private readonly string _connectionString =
            $@"Server=(localdb)\MSSQLLocalDB;Database=CatalogSimpleTests_{Guid.NewGuid():N};Trusted_Connection=True;TrustServerCertificate=True";

        public CatalogDbContext CreateContext() =>
            new(new DbContextOptionsBuilder<CatalogDbContext>().UseSqlServer(_connectionString).Options);

        public async Task InitializeAsync()
        {
            await using var db = CreateContext();
            await db.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await using var db = CreateContext();
            await db.Database.EnsureDeletedAsync();
        }
    }
}
