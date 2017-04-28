namespace Scraper.Model

open System
open Chiron

type Result = 
    {
        name :String;
        recordId :String option; 
        priceAUD :Decimal option; 
        distanceKm :Decimal option; 
        year :int option; 
        transmission :String option; 
        url :String option;
        scraped : DateTimeOffset
    } 
    
    static member ToJson (x :Result) = json { 
        do! Json.write "name" x.name
        do! Json.write "recordId" x.recordId
        do! Json.write "distanceKm" x.distanceKm
        do! Json.write "priceAUD" x.priceAUD
        do! Json.write "year" x.year
        do! Json.write "transmission" x.transmission
        do! Json.write "url" x.url
        do! Json.write "scraped" x.scraped
    }

    static member FromJson (_ :Result) = json {
        let! name = Json.read "name"
        let! recordId = Json.read "recordId"
        let! distanceKm = Json.read "distanceKm"
        let! priceAUD = Json.read "priceAUD"
        let! year = Json.read "year"
        let! transmission = Json.read "transmission"
        let! url = Json.read "url"
        let! scraped = Json.read "scraped"
        
        return { 
                name = name
                recordId = recordId 
                priceAUD =  priceAUD
                distanceKm = distanceKm
                year = year
                transmission = transmission
                url = url
                scraped = scraped
        }
    }