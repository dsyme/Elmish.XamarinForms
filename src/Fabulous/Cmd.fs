// Copyright 2018-2019 Elmish and Fabulous contributors. See LICENSE.md for license.
namespace Fabulous

/// Dispatch - feed new message into the processing loop
type Dispatch<'msg> = 'msg -> unit

/// Subscription - return immediately, but may schedule dispatch of a message at any time
type Sub<'msg> = Dispatch<'msg> -> unit

/// Cmd - container for subscriptions that may produce messages
type Cmd<'msg> = Sub<'msg> list

/// Cmd module for creating and manipulating commands
[<RequireQualifiedAccess>]
module Cmd =
    /// None - no commands, also known as `[]`
    let none : Cmd<'msg> =
        []

    /// Command to issue a specific message
    let ofMsg (msg:'msg) : Cmd<'msg> =
        [fun dispatch -> dispatch msg]

    /// Command to issue a specific message, only when Option.IsSome = true
    let ofMsgOption (msg:'msg option) : Cmd<'msg> =
        [ fun dispatch ->
             match msg with
             | None -> ()
             | Some msg -> dispatch msg ]

    /// When emitting the message, map to another type
    let map (f: 'a -> 'msg) (cmd: Cmd<'a>) : Cmd<'msg> =
        cmd |> List.map (fun g -> (fun dispatch -> f >> dispatch) >> g)

    /// Aggregate multiple commands
    let batch (cmds: #seq<Cmd<'msg>>) : Cmd<'msg> =
        cmds |> List.concat

    /// Command to call the subscriber
    let ofSub (sub: Sub<'msg>) : Cmd<'msg> =
        [sub]

    let dispatch d (cmd: Cmd<_>) = for sub in cmd do sub d

    /// Command to issue a message at the end of an asynchronous task
    let ofAsyncMsg (p: Async<'msg>) : Cmd<'msg> =
        [ fun dispatch -> async { let! msg = p in dispatch msg } |> Async.StartImmediate ]

    /// Command to issue a message at the end of an asynchronous task, only when Option.IsSome = true
    let ofAsyncMsgOption (p: Async<'msg option>) : Cmd<'msg> =
        [ fun dispatch -> async { 
            let! msg = p
            match msg with
            | None -> ()
            | Some msg -> dispatch msg } |> Async.StartImmediate ]

