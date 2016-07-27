#r "System.dll"
open System
open System.Net

let https = @"https://raw.githubusercontent.com/fsharp/FAKE/f4024fd5b790485828c1cfc002190716eee97597/modules/Octokit/Octokit.fsx";
let http = @"http://stackoverflow.com/"

let envProxies () =
    let getEnvValue (name:string) =
        let v = Environment.GetEnvironmentVariable(name.ToUpperInvariant())
        // under mono, env vars are case sensitive
        if isNull v then Environment.GetEnvironmentVariable(name.ToLowerInvariant()) else v
    let bypassList =
        let noproxy = getEnvValue "NO_PROXY"
        if String.IsNullOrEmpty noproxy then [||] else
        noproxy.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
    let getCredentials (uri:Uri) =
        let userPass = uri.UserInfo.Split([| ':' |], 2)
        if userPass.Length <> 2 || userPass.[0].Length = 0 then None else
        let credentials = NetworkCredential(Uri.UnescapeDataString userPass.[0], Uri.UnescapeDataString userPass.[1])
        Some credentials

    let getProxy (scheme:string) =
        let envVarName = sprintf "%s_PROXY" (scheme.ToUpperInvariant())
        let envVarValue = getEnvValue envVarName
        if isNull envVarValue then None else
        match Uri.TryCreate(envVarValue, UriKind.Absolute) with
        | true, envUri ->
            let proxy = WebProxy (Uri (sprintf "http://%s:%d" envUri.Host envUri.Port))
            proxy.Credentials <- Option.toObj <| getCredentials envUri
            proxy.BypassProxyOnLocal <- true
            proxy.BypassList <- bypassList
            Some proxy
        | _ -> None

    let addProxy (map:Map<string, WebProxy>) scheme =
        match getProxy scheme with
        | Some p -> Map.add scheme p map
        | _ -> map

    [ "http"; "https" ]
    |> List.fold addProxy Map.empty

let getDefaultProxyFor (url:string)=
    let uri = Uri url
    let getDefault () =
        let result = WebRequest.GetSystemWebProxy()
        let address = result.GetProxy uri 

        if address = uri then null else
        let proxy = WebProxy address
        proxy.Credentials <- CredentialCache.DefaultCredentials
        proxy.BypassProxyOnLocal <- true
        proxy

    match envProxies().TryFind uri.Scheme with
    | Some p -> if p.GetProxy uri <> uri then p else getDefault()
    | None -> getDefault()

let download (url:string) =
    printfn "%s" url;
    try
      use client = new WebClient()
      let proxy = WebRequest.DefaultWebProxy
      proxy.Credentials <- CredentialCache.DefaultCredentials
      client.Proxy <- proxy
      client.DownloadString url |> ignore
      printfn "Success!";
    with
    | ex -> printfn "Exception! %s " (ex.Message);

download http;
download https;