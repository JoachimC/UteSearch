module Scraper

open System

open canopy
open OpenQA.Selenium

type Search = { name :string; startPageUrl :string }

let tryParseDecimal (decimalStr :String) = 
   try
      let i = System.Decimal.Parse decimalStr
      Some i
   with _ -> None

let tryParseInt (intStr :String) = 
   try
      let i = System.Int32.Parse intStr
      Some i
   with _ -> None

let parseFromElementText regEx groupName (element: IWebElement option) = 
    element
    |> Option.map (fun e -> e.Text) 
    |> Option.map (fun e -> RegExExtensions.namedGroup regEx groupName e)
    |> Option.flatten

let parsePrice (result: IWebElement) :Decimal option = 
    (result |> someElementWithin "div.primary-price") 
    |> parseFromElementText @"\$(?<price>(\d*,)*\d*)" "price"
    |> Option.map tryParseDecimal
    |> Option.flatten

let parseDistance (result: IWebElement) :Decimal option= 
    (result |> someElementWithin "li.item-odometer") 
    |> parseFromElementText "(?<distance>(\d*,)*\d*)\skms" "distance"
    |> Option.map tryParseDecimal
    |> Option.flatten

let parseYear (description :String option) :int option = 
    description
    |> Option.map (fun d -> RegExExtensions.namedGroup "^(?<year>(\d*))\s" "year" d)
    |> Option.flatten
    |> Option.map tryParseInt
    |> Option.flatten

let parseTransmission (result: IWebElement) = 
    (result |> someElementWithin "li.item-transmission") 
    |> Option.map (fun p -> p.Text) 
    |> Option.map (fun x -> x.Trim())


let parseSearchResult name (result: IWebElement) : Scraper.Model.Result =
    let header = (result |> someElementWithin "div h2 a")    
    let url = header |> Option.map (fun h -> h.GetAttribute("href")) 
    let recordId = header |> Option.map (fun h -> h.GetAttribute("recordid"))
    let description = header |> Option.map (fun h -> h.GetAttribute("innerHTML"))
    let price = result |> parsePrice
    let distance = result |> parseDistance
    let year = description |> parseYear
    let transmission = result |> parseTransmission
    
    {
        name = name;
        recordId = recordId; 
        priceAUD = price; 
        distanceKm = distance; 
        year = year; 
        transmission = transmission; 
        url = url;
        scraped = DateTimeOffset.Now
    }

let href (element: IWebElement) = element |> someElementWithin "a" |> Option.map (fun a -> a.GetAttribute("href"))

let isValidNextLink (element: IWebElement) = 
        match (href element) with
        |None -> false
        |Some url -> not (url.Contains("javascript"))

let rec parseSearchPage (onResult : Scraper.Model.Result -> unit) (search :Search)  = 
    url search.startPageUrl

    let parseNamedSearchResult = parseSearchResult search.name    
    elements ".result-item"
    |> List.map parseNamedSearchResult
    |> List.iter onResult

    
    elements "li.next"
    |> List.filter isValidNextLink
    |> List.choose href
    |> List.distinct
    |> List.iter (fun result -> parseSearchPage onResult {search with startPageUrl = result})

    ()