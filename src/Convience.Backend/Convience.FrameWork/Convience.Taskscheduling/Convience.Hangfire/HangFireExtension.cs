﻿using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Convience.Hangfire
{
    public static class HangFireExtension
    {

        public static IServiceCollection AddHF(this IServiceCollection services,
            HangFireDataBaseType hangFireDataBaseType,
            string connectionString)
        {
            switch (hangFireDataBaseType)
            {
                case HangFireDataBaseType.PostgreSQL:
                    services.AddPostgreSQLHangFire(connectionString);
                    break;
                case HangFireDataBaseType.SqlServer:
                    services.AddSqlServerHangFire(connectionString);
                    break;
                case HangFireDataBaseType.InMemory:
                    services.AddInMemoryHangFire();
                    break;
            }
            return services;
        }

        public static IServiceCollection AddSqlServerHangFire(this IServiceCollection services,
            string connectionString)
        {
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));

            services.AddHangfireServer();

            return services;
        }

        public static IServiceCollection AddPostgreSQLHangFire(this IServiceCollection services,
            string connectionString)
        {
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(connectionString));

            services.AddHangfireServer();

            return services;
        }

        public static IServiceCollection AddInMemoryHangFire(this IServiceCollection services)
        {
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage());

            services.AddHangfireServer();

            return services;
        }

        public static IApplicationBuilder UseHFDashBoard(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new AllowAllAuthorizationFilter() },
                IsReadOnlyFunc = context => true
            });

            //// 创建一个新作业
            //BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget"));

            //// 延迟任务执行(Delayed jobs)
            //BackgroundJob.Schedule(() => Console.WriteLine("Delayed"), TimeSpan.FromSeconds(5));

            //// 定时任务执行(Recurring jobs)
            //RecurringJob.AddOrUpdate(() => Console.WriteLine("Hangfire is running Minutely."), Cron.Minutely());

            //// 延续性任务执行(Continuations)
            //var id = BackgroundJob.Enqueue(() => Console.WriteLine("Hello, "));
            //BackgroundJob.ContinueJobWith(id, () => Console.WriteLine("world!"));


            return app;
        }


        public static IApplicationBuilder UseHFDashBoard(this IApplicationBuilder app, string path)
        {
            app.UseHangfireDashboard(path);
            return app;
        }
    }
}
