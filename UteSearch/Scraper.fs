module Scraper

open System

open canopy
open OpenQA.Selenium

type result = {recordId :String option; price :Decimal option; distance :Decimal option; year :Decimal option; transmission :String option; url :String option }

let tryParseDecimal (decimalStr :String) = 
   try
      let i = System.Decimal.Parse decimalStr
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

let parseYear (description :String option) :Decimal option = 
    description
    |> Option.map (fun d -> RegExExtensions.namedGroup "^(?<year>(\d*))\s" "year" d)
    |> Option.flatten
    |> Option.map tryParseDecimal
    |> Option.flatten

let parseTransmission (result: IWebElement) = 
    (result |> someElementWithin "li.item-transmission") 
    |> Option.map (fun p -> p.Text) 
    |> Option.map (fun x -> x.Trim())


let parseSearchResult (result: IWebElement) =
    let header = (result |> someElementWithin "div h2 a")    
    let url = header |> Option.map (fun h -> h.GetAttribute("href")) 
    let recordId = header |> Option.map (fun h -> h.GetAttribute("recordid"))
    let description = header |> Option.map (fun h -> h.GetAttribute("innerHTML"))
    let price = result |> parsePrice
    let distance = result |> parseDistance
    let year = description |> parseYear
    let transmission = result |> parseTransmission
    
    {
        recordId = recordId; 
        price = price; 
        distance = distance; 
        year = year; 
        transmission = transmission; 
        url = url
    }

let href (element: IWebElement) = element |> someElementWithin "a" |> Option.map (fun a -> a.GetAttribute("href"))

let isValidNextLink (element: IWebElement) = 
        match (href element) with
        |None -> false
        |Some url -> not (url.Contains("javascript"))

let rec parseSearchPage searchPage = 
    url searchPage

    let thisPageResults = 
        elements "div.result-item"
        |> List.map parseSearchResult

    let otherPageResults = 
        elements "li.next"
        |> List.filter isValidNextLink
        |> List.choose href
        |> List.distinct
        |> List.map parseSearchPage
        |> List.concat

    List.append thisPageResults otherPageResults