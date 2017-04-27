open canopy
open Chiron

canopy.configuration.chromeDir <- @"."

let (searches : Scraper.Search array) = 
    [|
        {name = "Amarok 4x4"; startPageUrl = "http://www.carpoint.com.au/all-cars/results.aspx?q=((((Service%3d%5bCarsales%5d%26(Make%3d%5bVolkswagen%5d%26Model%3d%5bAmarok%5d))%26State%3d%5bSouth+Australia%5d)%26Drive%3d%5b4x4%5d)%26((((SiloType%3d%5bDealer+used+cars%5d)%7c(SiloType%3d%5bDemo+and+near+new+cars%5d))%7c(SiloType%3d%5bPrivate+seller+cars%5d))))&sortby=TopDeal"}
        {name = "hilux 4x4 4cyl"; startPageUrl = "http://www.carpoint.com.au/all-cars/results.aspx?q=(((((((Service%3d%5bCarsales%5d%26((SiloType%3d%5bDealer+used+cars%5d%7cSiloType%3d%5bDemo+and+near+new+cars%5d)%7cSiloType%3d%5bPrivate+seller+cars%5d))%26(Make%3d%5bToyota%5d%26Model%3d%5bHilux%5d))%26State%3d%5bSouth+Australia%5d)%26BodyStyle%3d%5bUte%5d)%26Drive%3d%5b4x4%5d)%26Doors%3d%5b4%5d)%26Cylinders%3d%5b4%5d)&sortby=TopDeal"}
    |]

let writeResult dataDir (result :Scraper.Result) = 
    let fileName = result.recordId |> Option.map (fun rId -> (sprintf "%s.json" rId))

    match fileName with 
    | Some f ->         
            let file = System.IO.Path.Combine(dataDir.ToString(), f)
            if (not (System.IO.FileInfo(file)).Exists) then 
                let json = Json.serialize result
                let content = Json.formatWith JsonFormattingOptions.Pretty json
                System.IO.File.WriteAllText(file.ToString(), content)
    | None -> ()

let search dataDir search = 
    start chrome

    let writeResultToDir = writeResult dataDir
    try
        Scraper.parseSearchPage search
        |> List.iter writeResultToDir

    finally
        quit()
    ()

    
[<EntryPoint>]
let main argv =     
    
    let dataPath =
        if ( (Array.length argv) = 1) then 
            argv.[0]
        else
            System.IO.Path.GetTempPath()

    let dataDir = System.IO.DirectoryInfo(dataPath)
    let searchToDataDir = search dataDir

    searches
    |> Array.iter searchToDataDir 
    
    0

