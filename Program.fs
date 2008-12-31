﻿#light
module LRF.SchemaRunner.Program
open LRF.SchemaRunner.Core
open LRF.SchemaRunner.Configuration
open System.Xml

let xml = "
<Configuration>
  <IncludeDirectories>
    <Directory Name='C:\Temp\Archive'/>
    <Directory Name='C:\Temp\SchemaVersions'/>
    <Directory Name='C:\Temp\UpgradeScripts'/>
  </IncludeDirectories>

  <DatabaseGroup Name='sql'>
    <Database Name='Test1' Server='localhost' Type='MsSql' User='sa' Password='sa' SchemaTable='SchemaInfo'/>
  </DatabaseGroup>
  
  <DatabaseGroup Name='oracle'>
    <Database Name='Test1' Server='localhost' Type='Oracle' User='Test1' Password='test1' SchemaTable='SchemaInfo'/>
  </DatabaseGroup>
</Configuration>"

let parse (xml : string) =
   let doc = new XmlDocument()
   doc.LoadXml(xml)
   let directories = directories doc
   Seq.iter (fun (d : Directory) -> printfn "Path is %s" d.Name) directories
   let database_groups = database_groups doc
   Seq.iter (fun (g : DatabaseGroup) -> 
                printfn "Database Group Name:%s" g.Name
                Seq.iter (fun (d : Database) -> 
                            printfn "Database Name: %s" d.Name
                            printfn "Database Server: %s" d.Server
                            printfn "Database Type: %s" (d.DatabaseType.ToString())
                            printfn "Database User: %s" d.User
                            printfn "Database Password: %s" d.Password
                            printfn "Database SchemaInfo: %s" d.SchemaInfo) 
                         g.Databases.Value)
            database_groups
   ()

parse xml

//let db = new Database("LarryTest", "phiv5dbdev", DatabaseType.Oracle, "LarryTest", "changeme", "SchemaInfo")
//db.LoadSchemaVersion() 
//db.SchemaVersion |> printfn "YO: %s" 

//System.Console.ReadLine() |> ignore