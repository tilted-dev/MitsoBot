namespace SharedImp.Models

open System
open System.Collections.Generic
open Microsoft.FSharp.Collections

type Schedule() = inherit Dictionary<string, Dictionary<DateTime, Lesson list>>()