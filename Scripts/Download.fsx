#r "System.dll"
open System.Net

let https = @"https://raw.githubusercontent.com/fsharp/FAKE/f4024fd5b790485828c1cfc002190716eee97597/modules/Octokit/Octokit.fsx";
let http = @"http://stackoverflow.com/"

let download (url:string) =
    printfn "%s" url;
    try
      use client = new WebClient()
      client.DownloadString url |> ignore
      printfn "Success!";
    with
    | ex -> printfn "Exception! %s " (ex.Message);

download http;
download https;