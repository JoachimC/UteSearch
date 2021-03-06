﻿open canopy
open Chiron

canopy.configuration.chromeDir <- @"."
// let adBlockExtensionPath = @"C:\Users\joachim\AppData\Local\Google\Chrome\User Data\Profile 1\Extensions\gighmmpiobklfepjocnamgkkbiglidom"
let adBlockExtensionPath = @"C:\Users\joachim\AppData\Local\Google\Chrome\User Data\Profile 1\Extensions\gighmmpiobklfepjocnamgkkbiglidom\3.12.0_0"

let chromeOptions = OpenQA.Selenium.Chrome.ChromeOptions()
chromeOptions.AddArgument("load-extension=" + adBlockExtensionPath)
// ChromeOptions options = new ChromeOptions();
// options.addArguments("load-extension=/path/to/extension");
// DesiredCapabilities capabilities = new DesiredCapabilities();
// capabilities.setCapability(ChromeOptions.CAPABILITY, options);
// ChromeDriver driver = new ChromeDriver(capabilities);

let (searches : Scraper.Search array) = 
    [|
        {name = "rubicon 2dr"; startPageUrl = "http://www.carpoint.com.au/all-cars/results.aspx?q=((Service%3d%5bCarsales%5d%26((SiloType%3d%5bDealer+used+cars%5d%7cSiloType%3d%5bDemo+and+near+new+cars%5d)%7cSiloType%3d%5bPrivate+seller+cars%5d))%26(Make%3d%5bJeep%5d%26(Model%3d%5bWrangler%5d%26Badge%3d%5bRubicon%5d)))&sortby=TopDeal"}
        {name = "rubicon 4dr"; startPageUrl = "http://www.carpoint.com.au/all-cars/results.aspx?q=((Service%3d%5bCarsales%5d%26((SiloType%3d%5bDealer+used+cars%5d%7cSiloType%3d%5bDemo+and+near+new+cars%5d)%7cSiloType%3d%5bPrivate+seller+cars%5d))%26(Make%3d%5bJeep%5d%26(Model%3d%5bWrangler%5d%26Badge%3d%5bUnlimited+Rubicon%5d)))&sortby=TopDeal"}
        {name = "Amarok 4x4 4cyl"; startPageUrl = "http://www.carpoint.com.au/all-cars/results.aspx?q=(((((Service%3d%5bCarsales%5d%26(Make%3d%5bVolkswagen%5d%26Model%3d%5bAmarok%5d))%26State%3d%5bSouth+Australia%5d)%26Drive%3d%5b4x4%5d)%26((SiloType%3d%5bDealer+used+cars%5d%7cSiloType%3d%5bDemo+and+near+new+cars%5d)%7cSiloType%3d%5bPrivate+seller+cars%5d))%26Cylinders%3d%5b4%5d)&sortby=TopDeal"}
        {name = "hilux 4x4 4cyl"; startPageUrl = "http://www.carpoint.com.au/all-cars/results.aspx?q=(((((((Service%3d%5bCarsales%5d%26((SiloType%3d%5bDealer+used+cars%5d%7cSiloType%3d%5bDemo+and+near+new+cars%5d)%7cSiloType%3d%5bPrivate+seller+cars%5d))%26(Make%3d%5bToyota%5d%26Model%3d%5bHilux%5d))%26State%3d%5bSouth+Australia%5d)%26BodyStyle%3d%5bUte%5d)%26Drive%3d%5b4x4%5d)%26Doors%3d%5b4%5d)%26Cylinders%3d%5b4%5d)&sortby=TopDeal"}
        {name = "ford ranger 2.2L"; startPageUrl = "http://www.carpoint.com.au/all-cars/results.aspx?q=((((((((Service%3d%5bCarsales%5d%26((SiloType%3d%5bDealer+used+cars%5d%7cSiloType%3d%5bDemo+and+near+new+cars%5d)%7cSiloType%3d%5bPrivate+seller+cars%5d))%26(Make%3d%5bFord%5d%26Model%3d%5bRanger%5d))%26State%3d%5bSouth+Australia%5d)%26BodyStyle%3d%5bUte%5d)%26Drive%3d%5b4x4%5d)%26Doors%3d%5b4%5d)%26Cylinders%3d%5b4%5d))%26(EngineSize%3drange%5b..2500%5d)&sortby=TopDeal"}
        {name = "ford ranger 3.0L"; startPageUrl = "http://www.carpoint.com.au/all-cars/results.aspx?q=((((((((Service%3d%5bCarsales%5d%26((SiloType%3d%5bDealer+used+cars%5d%7cSiloType%3d%5bDemo+and+near+new+cars%5d)%7cSiloType%3d%5bPrivate+seller+cars%5d))%26(Make%3d%5bFord%5d%26Model%3d%5bRanger%5d))%26State%3d%5bSouth+Australia%5d)%26BodyStyle%3d%5bUte%5d)%26Drive%3d%5b4x4%5d)%26Doors%3d%5b4%5d)%26Cylinders%3d%5b4%5d))%26(EngineSize%3drange%5b2500..3500%5d)&sortby=TopDeal"}
    |]

let writeResult dataDir (result :Scraper.Model.Result) = 
    let fileName = result.recordId |> Option.map (fun rId -> (sprintf "%s.json" rId))

    match fileName with 
    | Some f ->         
            let file = System.IO.Path.Combine(dataDir.ToString(), f)
            if (not (System.IO.FileInfo(file)).Exists) then 
                let content = result |> Json.serialize |> Json.formatWith JsonFormattingOptions.Pretty
                System.IO.File.WriteAllText(file.ToString(), content)
    | None -> ()

let search onResult search = 
    // start chrome
    start <| ChromeWithOptions chromeOptions

    try
        Scraper.parseSearchPage onResult search
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
    let writeResultToDir = writeResult dataDir
    let searchAndWriteToDir = search writeResultToDir

    searches
    |> Array.iter searchAndWriteToDir
    
    0

