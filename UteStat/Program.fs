open Chiron

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
            "Name";
            "PriceAud";
            "DistanceKm"; 
            "AgeYears"; 
            "YearOfManufacture";
            "Transmission"; 
            "Url";
        |] 
        |> String.concat ","
    printf "%s\n" row

    let foldDecimalToStringOrEmpty = Option.fold (fun _ (x :decimal) -> x.ToString()) ""
    let foldOrEmpty = Option.fold (fun _ (x :string) -> x) ""

    dataDir.GetFiles("*.json")
    |> Seq.map (fun fileName -> System.IO.File.ReadAllText(fileName.FullName))
    |> Seq.map (fun resultText -> resultText |> Json.parse |> Json.deserialize)
    |> Seq.iter (fun (result :Scraper.Model.Result) -> 
        let row = 
            [| 
                result.name
                result.priceAUD |> foldDecimalToStringOrEmpty
                result.distanceKm |> Option.map (fun distKm -> if (distKm < 100m) then distKm * 1000m else distKm) |> foldDecimalToStringOrEmpty 
                result.year |> Option.fold (fun _ year -> (result.scraped.Year-year).ToString()) ""
                result.year |> Option.fold (fun _ year -> year.ToString()) ""
                result.transmission |> foldOrEmpty
                result.url |> foldOrEmpty
            |] 
            |> String.concat ","
        printf "%s\n" row)

    0
