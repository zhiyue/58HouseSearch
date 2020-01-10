﻿using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using HouseMap.Common;
using HouseMap.Dao;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HouseMapConsumer.Dao;
using System.IO;
using StackExchange.Redis;

namespace HouseMap.Crawler
{
    class Program
    {
        public static IConfiguration Configuration;

        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var services = new ServiceCollection();
            InitConfiguration(environmentName, services);
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var refreshHouseVersionV2 = Environment.GetEnvironmentVariable("REFRESH_CACHE_V2");
            if (!string.IsNullOrEmpty(refreshHouseVersionV2))
            {
                var houseService = serviceProvider.GetServices<HouseService>().FirstOrDefault();
                houseService.RefreshHouseV2();
                return;
            }

            var refreshHouseVersionV3 = Environment.GetEnvironmentVariable("REFRESH_CACHE_V3");
            if (!string.IsNullOrEmpty(refreshHouseVersionV3))
            {
                var houseService = serviceProvider.GetServices<HouseService>().FirstOrDefault();
                houseService.RefreshHouseV3();
                return;
            }
            var crawlName = Environment.GetEnvironmentVariable("CRAWL_NAME");
            var crawler = serviceProvider.GetServices<IHouseCrawler>().FirstOrDefault(c => c.GetSource().GetSourceName().ToUpper() == crawlName?.ToUpper());
            if (crawler == null)
            {
                Console.WriteLine($"crawler:{crawlName} not found!");
                return;
            }
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IS_ANALYZE")))
            {
                crawler.AnalyzeData();
            }
            else
            {
                crawler.Run();
            }

        }

        private static void InitConfiguration(string environmentName, ServiceCollection services)
        {
            Configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
                        .AddEnvironmentVariables().Build();
            services.AddOptions().Configure<AppSettings>(x => Configuration.Bind(x));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        private static void ConfigureServices(IServiceCollection services)
        {
            InitRedis(services);
            InitDI(services);
            InitDB(services);
        }

        private static void InitRedis(IServiceCollection services)
        {
            services.AddSingleton<ConnectionMultiplexer, ConnectionMultiplexer>(factory =>
            {
                ConfigurationOptions options = ConfigurationOptions.Parse(Configuration["RedisConnectionString"]);
                options.SyncTimeout = 10 * 10000;
                return ConnectionMultiplexer.Connect(options);
            });
        }


        private static void InitDB(IServiceCollection services)
        {
            services.AddDbContextPool<HouseMapContext>(options =>
           {
               options.UseMySql(Configuration["QCloudMySQL"].ToString());
           });
        }



        private static void InitDI(IServiceCollection services)
        {
            #region Mapper
            services.AddScoped<ConfigDapper, ConfigDapper>();
            services.AddScoped<BaseDapper, BaseDapper>();
            services.AddScoped<BaseDapper, BaseDapper>();
            services.AddScoped<HouseDapper, HouseDapper>();
            services.AddScoped<HouseMongoMapper, HouseMongoMapper>();
            services.AddScoped<MongoDBMapper, MongoDBMapper>();

            #endregion Service

            #region Service
            services.AddScoped<RedisTool, RedisTool>();
            services.AddScoped<HouseService, HouseService>();
            services.AddScoped<ElasticService, ElasticService>();
            services.AddScoped<ConfigService, ConfigService>();

            #endregion

            #region Crawler


            services.AddScoped<BaseCrawler, BaseCrawler>();
            services.AddScoped<IHouseCrawler, Zuber>();
            services.AddScoped<IHouseCrawler, Beike>();
            services.AddScoped<IHouseCrawler, DoubanWechat>();
            services.AddScoped<IHouseCrawler, Huzhu>();
            services.AddScoped<IHouseCrawler, Mogu>();
            services.AddScoped<IHouseCrawler, BaixingWechat>();
            services.AddScoped<IHouseCrawler, Douban>();
            services.AddScoped<IHouseCrawler, CCBHouse>();
            services.AddScoped<IHouseCrawler, PinPaiGongYu>();
            //services.AddScoped<INewCrawler, Xianyu>();
            services.AddScoped<IHouseCrawler, Fangduoduo>();
            services.AddScoped<IHouseCrawler, Fangtianxia>();
            services.AddScoped<IHouseCrawler, Hizhu>();
            services.AddScoped<IHouseCrawler, V2ex>();
            services.AddScoped<IHouseCrawler, Pinshiyou>();
            services.AddScoped<IHouseCrawler, Hezuzhaoshiyou>();
            services.AddScoped<IHouseCrawler, Baletu>();
            services.AddScoped<IHouseCrawler, Anjuke>();
            services.AddScoped<IHouseCrawler, ZiRoom>();
            services.AddScoped<IHouseCrawler, Qingke>();
            services.AddScoped<IHouseCrawler, CJia>();
            services.AddScoped<IHouseCrawler, Hangzhouzhufang>();
            services.AddScoped<IHouseCrawler, AnXuan>();
            services.AddScoped<IHouseCrawler, Nuan>();
            services.AddScoped<IHouseCrawler, Xhj>();

            #endregion

        }

    }
}
