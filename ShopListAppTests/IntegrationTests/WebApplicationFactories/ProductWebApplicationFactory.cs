﻿using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ShopListApp;
using ShopListApp.Database;
using ShopListApp.DataProviders;
using ShopListApp.Interfaces;
using ShopListAppTests.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopListAppTests.IntegrationTests.WebApplicationFactories
{
    public class ProductWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<ShopListDbContext>>();
                services.RemoveAll<ShopListDbContext>();

                services.AddDbContext<TestDbContext>(options =>
                                   options.UseInMemoryDatabase("ProductTestDb"));

                services.AddScoped<ShopListDbContext, TestDbContext>();
                services.AddTransient<IHtmlFetcher<HtmlNode, HtmlDocument>, BiedronkaHtmlFetcherStub>();

            });
        }
    }
}
