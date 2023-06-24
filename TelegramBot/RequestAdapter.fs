namespace VkontakteBot

open SharedImp.Enums

open SharedImp.Resources
open SharedImp.Services
open SharedImp.Tables
open Telegram.Bot
open TelegramBot.ButtonPayloadMessageHandlers
open TelegramBot.TextMessageHandlers

type RequestAdapter(tgClient:ITelegramBotClient) as this =
    inherit BaseRequestAdapter() do
        this.RegisterTextMessageHandler<StartTextMessageHandler>("/start", "start", "начать", "меню")
        this.RegisterTextMessageHandler<TeacherTextMessageHandler> "/teacher"

        this.RegisterButtonPayloadHandler<StartButtonPayloadHandler> ActionType.Start
        this.RegisterButtonPayloadHandler<SettingsButtonPayloadHandler> ActionType.Settings
        this.RegisterButtonPayloadHandler<SearchScheduleButtonPayloadHandler> ActionType.SearchSchedule
        this.RegisterButtonPayloadHandler<SetFacultyButtonPayloadHandler> ActionType.SetFaculty
        this.RegisterButtonPayloadHandler<BackToFacultyButtonPayloadHandler> ActionType.BackToFaculty
        this.RegisterButtonPayloadHandler<SetFormButtonPayloadHandler> ActionType.SetForm
        this.RegisterButtonPayloadHandler<BackToFormButtonPayloadHandler> ActionType.BackToForm
        this.RegisterButtonPayloadHandler<SetCourseButtonPayloadHandler> ActionType.SetCourse
        this.RegisterButtonPayloadHandler<BackToCourseButtonPayloadHandler> ActionType.BackToCourse
        this.RegisterButtonPayloadHandler<SetGroupButtonPayloadHandler> ActionType.SetGroup
        this.RegisterButtonPayloadHandler<SearchTeacherScheduleButtonPayloadHandler> ActionType.SearchTeacherSchedule
        this.RegisterButtonPayloadHandler<SpecificRangeButtonPayloadHandler> ActionType.SpecificScheduleRange
        this.RegisterButtonPayloadHandler<TodayScheduleButtonPayloadHandler> ActionType.TodaySchedule
        this.RegisterButtonPayloadHandler<TomorrowScheduleButtonPayloadHandler> ActionType.TomorrowSchedule
        this.RegisterButtonPayloadHandler<ResetUserButtonPayloadHandler> ActionType.ResetUser
        this.RegisterButtonPayloadHandler<RefreshScheduleButtonPayloadHandler> ActionType.RefreshSchedule

    override this.HandleUnknownButtonPayload(user:User) = async {
        do! tgClient.SendTextMessageAsync(user.Id, TextMessages.CallbackPayloadOutdated)
            |> Async.AwaitTask
            |> Async.Ignore
    }