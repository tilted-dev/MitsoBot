namespace TelegramBot
#nowarn "20"
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open SharedImp
open SharedImp.Interfaces
open SharedImp.Services
open Telegram.Bot
open TelegramBot.Middlewares
open VkontakteBot
open Quartz
open SharedImp.Utils
module Program =
    let exitCode = 0

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

                builder.Services.AddHttpClient("telegram_bot_client")
                    .AddTypedClient<ITelegramBotClient>(fun httpClient sp -> (
                        let config = sp.GetRequiredService<IConfiguration>()
                        TelegramBotClient(config["Token"], httpClient) :> ITelegramBotClient
                    ))
                    
                let requestAdapter = RequestAdapter <| builder.Services.BuildServiceProvider().GetRequiredService<ITelegramBotClient>()
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