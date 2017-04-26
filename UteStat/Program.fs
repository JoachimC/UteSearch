open FSharp.Data

type Result = JsonProvider<""" {
  "distanceKm": 90054,
  "priceAUD": 27981,
  "recordId": "OAG-AD-14370273",
  "scraped": "2017-04-20T23:27:24.3767257+01:00",
  "transmission": "Manual",
  "url": "http://www.carpoint.com.au/all-cars/dealer/details.aspx?R=OAG-AD-14370273&Cr=0",
  "year": 2011
} """>

[<EntryPoint>]
let main argv = 
    let dataPath =
        if ( (Array.length argv) = 1) then 
            argv.[0]
        else
            System.IO.Path.GetTempPath()

    let dataDir = System.IO.DirectoryInfo(dataPath)

    let row = 
        [| 
            "PriceAud";
            "DistanceKm"; 
            "AgeYears"; 
            "YearOfManufacture";
            "Transmission"; 
            "Url";
        |] 
        |> String.concat ","
    printf "%s\n" row

    dataDir.GetFiles("*.json")
    |> Seq.map (fun fileName -> System.IO.File.ReadAllText(fileName.FullName))
    |> Seq.map (fun resultText -> Result.Parse(resultText))
    |> Seq.iter (fun result -> 
        let row = 
            [| 
                result.PriceAud.ToString();
                result.DistanceKm.ToString(); 
                (result.Scraped.Year-result.Year).ToString(); 
                result.Scraped.Year.ToString();
                result.Transmission; 
                result.Url
            |] 
            |> String.concat ","
        printf "%s\n" row)

    0
