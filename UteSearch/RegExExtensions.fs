module RegExExtensions

open System.Linq
open System.Text.RegularExpressions

let namedGroup pattern (name: string) input =
   let m = Regex.Match(input,pattern) 
   if (m.Success) then Some (m.Groups.Item(name)).Value else None

