namespace VkontakteBot
#nowarn "20"
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open SharedImp.Interfaces
open SharedImp.Services
open VkNet
open VkNet.Abstractions
open VkNet.Model
open Serilog
open VkontakteBot.Middlewares
open Quartz
open SharedImp
open SharedImp.Utils

module Program =

    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)

        Log.Logger <-
            LoggerConfiguration()
                .Enrich.With<UserEnricher>()
                .Destructure.With<MitsoDestructuringPolicy>()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger()
        try
            try
                Dapper.FSharp.SQLite.OptionTypes.register()
                
                builder.Host.UseSerilog()
                
                builder.Services.AddScoped<IUsersService, UsersService>()
                builder.Services.AddScoped<IMitsoApiService, MitsoApiService>()
                builder.Services.AddScoped<IMitsoService, MitsoService>()
                builder.Services.AddScoped<IScheduleService, ScheduleService>()
                
                builder.Services.AddControllers().AddNewtonsoftJson()
                builder.Services.AddHttpClient()

                builder.Services.AddSingleton<IVkApi, VkApi>(fun _ -> 
                    let api = new VkApi()
                    api.Authorize(ApiAuthParams(AccessToken = builder.Configuration["Token"]))
                    
                    api
                )

                let requestAdapter = RequestAdapter <| builder.Services.BuildServiceProvider().GetRequiredService<IVkApi>()
                builder.Services.RegisterHandlersForRequestAdapter requestAdapter
                builder.Services.AddSingleton<IRequestAdapter> requestAdapter

                builder.Services.AddQuartz(fun x -> (
                    x.AddJob<CleanDbJob>(fun x -> x.WithIdentity "CleanDbJob" |> ignore)
                    x.AddTrigger(fun x ->
                        x.ForJob("CleanDbJob").WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(06, 00)) |> ignore)
                    ()
                ))

                builder.Services.AddQuartzHostedService(fun x -> x.WaitForJobsToComplete <- false)

                DbSetup.CreateUsersTable()
                DbSetup.CreateFacultiesTable()
                DbSetup.CreateFormsTable()
                DbSetup.CreateCoursesTable()
                DbSetup.CreateGroupsTable()
                DbSetup.CreateScheduleTable()
                DbSetup.CreateTeacherScheduleTable()

                let app = builder.Build()
                
                app.UseMiddleware<ErrorHandlerMiddleware>()
                app.UseAuthorization()
                app.MapControllers()

                app.Run()
                0
            with ex ->
                Log.ForContext("SourceContext", "Program").Error(ex, "Application terminated unexpectedly")
                0
        finally
            Log.CloseAndFlush()