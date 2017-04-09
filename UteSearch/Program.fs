open canopy

canopy.configuration.chromeDir <- @"."

let startPageUrl = "http://www.carpoint.com.au/all-cars/results.aspx?q=((((Service%3d%5bCarsales%5d%26(Make%3d%5bVolkswagen%5d%26Model%3d%5bAmarok%5d))%26State%3d%5bSouth+Australia%5d)%26Drive%3d%5b4x4%5d)%26((((SiloType%3d%5bDealer+used+cars%5d)%7c(SiloType%3d%5bDemo+and+near+new+cars%5d))%7c(SiloType%3d%5bPrivate+seller+cars%5d))))&sortby=TopDeal"

[<EntryPoint>]
let main argv =     

    start chrome
    try
        Scraper.parseSearchPage startPageUrl
        |> List.iter (fun r -> printfn "%A\n" r)

    finally
        quit()
    0
