module HCup.RequestCounter

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Juraff.HttpHandlers
open Juraff.Tasks

let outstandingRequestCount = ref 0

let private getRequestInfo (ctx : HttpContext) =
    (ctx.Request.Protocol,
     ctx.Request.Method,
     ctx.Request.Path.ToString())
|||> sprintf "%s %s %s"

type RequestCounterMiddleware (next : RequestDelegate,
                               handler : unit-> unit) =
    do if isNull next then raise (ArgumentNullException("next"))

    member __.Invoke (ctx : HttpContext) =
        task {
            let! result = next.Invoke ctx

            Interlocked.Increment(outstandingRequestCount)
            |> (fun reqCount -> 
                                // if (reqCount = 150154 || reqCount = 190154)
                                // then GC.Collect(1)
                                if (reqCount % 10000 = 0)
                                then
                                    handler()
                                    Console.Write(("Result {0} {1}; Threads {2}; "),
                                        reqCount,
                                        DateTime.Now.ToString("HH:mm:ss.ffff"),
                                        Process.GetCurrentProcess().Threads.Count)
                                    Console.WriteLine("Gen0={0} Gen1={1} Gen2={2}",
                                        GC.CollectionCount(0),
                                        GC.CollectionCount(1),
                                        GC.CollectionCount(2)))



            
        } :> Task


type IApplicationBuilder with
    member this.UseRequestCounter (handler : unit -> unit) =
        this.UseMiddleware<RequestCounterMiddleware> handler
        |> ignore