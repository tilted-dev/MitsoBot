namespace SharedImp.Services

open System
open System.Net
open System.Net.Http
open Microsoft.Extensions.Configuration
open Microsoft.FSharp.Control
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Serilog
open SharedImp.Interfaces
open SharedImp.Models
open SharedImp.MonadModels.MitsoApiResults

type MitsoApiService(httpClient: HttpClient, config: IConfiguration) =
    let key = config["key"]
    let mutable logger = Log.ForContext<MitsoApiService>()
    do
        httpClient.BaseAddress <- Uri "https://student.mitso.by/api/"
    interface IMitsoApiService with
        member this.GetFacultiesAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let uri = $"get_faculties.php?key={key}"
                let! response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                let! result = async {
                    match response.StatusCode with
                    | HttpStatusCode.OK ->
                        logger
                            .Information("Successfully received faculties from mitso API.")
                        return Ok <| JsonConvert.DeserializeObject<string list> content
                    | HttpStatusCode.NoContent ->
                        logger
                            .Warning("[NoContent] ==> Failed to receive faculties from mitso API.")
                        return Error <| NoContent
                    | _ ->
                        logger
                            .ForContext("StatusCode", response.StatusCode)
                            .ForContext("ResponseMsg", JObject.Parse(content).["message"].ToString())
                            .Error("[ApiError] ==> Failed to process GET request. Received not succeeds status-code: {StatusCode}. Message: {ResponseMsg}")
                        return Error <| ApiError
                }
                return result
            with ex ->
                logger.Error(ex, "[UnexpectedMitsoApiServiceError] ==> Failed to process GET request. An unexpected exception was thrown")
                return Error <| UnexpectedMitsoApiServiceError
        }
        
        member this.GetCoursesAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let uri = $"get_courses.php?faculty={user.Faculty}&form_edu={user.Form}&key={key}"
                let! response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                let! result = async {
                    match response.StatusCode with
                    | HttpStatusCode.OK ->
                        logger
                            .Information("Successfully received courses from mitso API.")
                        return Ok <| ((JsonConvert.DeserializeObject<string list> content) |> Seq.sort |> Seq.toList)
                    | HttpStatusCode.NoContent ->
                        logger
                            .Warning("[NoContent] ==> Failed to receive courses from mitso API.")
                        return Error <| NoContent
                    | _ ->
                        logger
                            .ForContext("StatusCode", response.StatusCode)
                            .ForContext("ResponseMsg", JObject.Parse(content).["message"].ToString())
                            .Error("[ApiError] ==> Failed to process GET request. Received not succeeds status-code: {StatusCode}. Message: {ResponseMsg}")
                        return Error <| ApiError
                }
                return result
            with ex ->
                logger.Error(ex, "[UnexpectedMitsoApiServiceError] ==> Failed to process GET request. An unexpected exception was thrown")
                return Error <| UnexpectedMitsoApiServiceError
        }
        
        member this.GetFormsAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let uri = $"get_forms.php?faculty={user.Faculty}&key={key}"
                let! response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                let! result = async {
                    match response.StatusCode with
                    | HttpStatusCode.OK ->
                        logger
                            .Information("Successfully received forms from mitso API.")
                        return Ok <| JsonConvert.DeserializeObject<string list> content
                    | HttpStatusCode.NoContent ->
                        logger
                            .Warning("[NoContent] ==> Failed to receive forms from mitso API.")
                        return Error <| NoContent
                    | _ ->
                        logger
                            .ForContext("StatusCode", response.StatusCode)
                            .ForContext("ResponseMsg", JObject.Parse(content).["message"].ToString())
                            .Error("[ApiError] ==> Failed to process GET request. Received not succeeds status-code: {StatusCode}. Message: {ResponseMsg}")
                        return Error <| ApiError
                }
                return result
            with ex ->
                logger.Error(ex, "[UnexpectedMitsoApiServiceError] ==> Failed to process GET request. An unexpected exception was thrown")
                return Error <| UnexpectedMitsoApiServiceError
        }
        
        member this.GetGroupsAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let uri = $"get_groups.php?faculty={user.Faculty}&form_edu={user.Form}&course={user.Course}&key={key}"
                let! response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                let! result = async {
                    match response.StatusCode with
                    | HttpStatusCode.OK ->
                        logger
                            .Information("Successfully received groups from mitso API.")
                        return Ok <| JsonConvert.DeserializeObject<string list> content
                    | HttpStatusCode.NoContent ->
                        logger
                            .Warning("[NoContent] ==> Failed to receive groups from mitso API.")
                        return Error <| NoContent
                    | _ ->
                        logger
                            .ForContext("StatusCode", response.StatusCode)
                            .ForContext("ResponseMsg", JObject.Parse(content).["message"].ToString())
                            .Error("[ApiError] ==> Failed to process GET request. Received not succeeds status-code: {StatusCode}. Message: {ResponseMsg}")
                        return Error <| ApiError
                }
                return result
            with ex ->
                logger.Error(ex, "[UnexpectedMitsoApiServiceError] ==> Failed to process GET request. An unexpected exception was thrown")
                return Error <| UnexpectedMitsoApiServiceError
        }
        
        member this.GetScheduleAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let uri = $"get_schedule.php?faculty={user.Faculty}&form_edu={user.Form}&course={user.Course}&group={user.GroupName}&key={key}&deep=true"
                let! response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                let! result = async {
                    match response.StatusCode with
                    | HttpStatusCode.OK ->
                        logger
                            .Information("Successfully received schedule from mitso API.")
                        return Ok <| JsonConvert.DeserializeObject<Schedule> content
                    | HttpStatusCode.NoContent ->
                        logger
                            .Warning("[NoContent] ==> Failed to receive schedule from mitso API.")
                        return Error <| NoContent
                    | _ ->
                        logger
                            .ForContext("StatusCode", response.StatusCode)
                            .ForContext("ResponseMsg", JObject.Parse(content).["message"].ToString())
                            .Error("[ApiError] ==> Failed to process GET request. Received not succeeds status-code: {StatusCode}. Message: {ResponseMsg}")
                        return Error <| ApiError
                }
                return result
            with ex ->
                logger.Error(ex, "[UnexpectedMitsoApiServiceError] ==> Failed to process GET request. An unexpected exception was thrown")
                return Error <| UnexpectedMitsoApiServiceError
        }
        
        member this.GetTeacherScheduleAsync(user, surname) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let uri = $"get_teacher.php?teacher={surname}&key={key}&deep=true"
                let! response = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri)) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                let! result = async {
                    match response.StatusCode with
                    | HttpStatusCode.OK ->
                        logger
                            .Information("Successfully received teacher schedule from mitso API.")
                        return Ok <| JsonConvert.DeserializeObject<Schedule> content
                    | HttpStatusCode.NoContent ->
                        logger
                            .Warning("[NoContent] ==> Failed to receive teacher schedule from mitso API.")
                        return Error <| NoContent
                    | _ ->
                        logger
                            .ForContext("StatusCode", response.StatusCode)
                            .ForContext("ResponseMsg", JObject.Parse(content).["message"].ToString())
                            .Error("[ApiError] ==> Failed to process GET request. Received not succeeds status-code: {StatusCode}. Message: {ResponseMsg}")
                        return Error <| ApiError
                }
                return result
            with ex ->
                logger.Error(ex, "[UnexpectedMitsoApiServiceError] ==> Failed to process GET request. An unexpected exception was thrown")
                return Error <| UnexpectedMitsoApiServiceError
        }