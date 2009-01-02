﻿#light
module LRF.SchemaRunner.Configuration
open LRF.SchemaRunner.Core
open System.Xml

/// Generic translation function that translates XmlNodeList into Seq type of returned by fn
let translate fn (nodes : XmlNodeList) =
   nodes
   |> Seq.cast<XmlNode>
   |> Seq.map (fun node -> fn node)

/// Selects the DatabaseGroup nodes in XmlDocument via XPath    
let database_groups_nodes (doc : XmlDocument) =
   doc.SelectNodes "//DatabaseGroup"

/// Convert a XmlNode into a Database class
let node_to_database (node : XmlNode) =
    let name, server, database_type, user, password, schema_table = 
        match node.Attributes.["Name"], 
              node.Attributes.["Server"],
              node.Attributes.["Type"],
              node.Attributes.["User"],
              node.Attributes.["Password"],
              node.Attributes.["SchemaTable"] with
        | name, server, database_type, user, password, schema_table
            -> name.Value, server.Value, database_type.Value, user.Value, password.Value, schema_table.Value
    new Database(name, server,  DatabaseType.Convert database_type, user, password, schema_table)

/// Retrieve all the databases defined in the XmlNodeList
let databases (nodes : XmlNodeList) =
   nodes
   |> translate node_to_database
       
/// Convert a XmlNode into a DatabaseGroup class
let node_to_database_groups (node : XmlNode) =
   let name = match node.Attributes.["Name"] with
              | null -> null
              | name -> name.Value
   new DatabaseGroup(name, Some(databases node.ChildNodes))

/// Retrieve all the database groups defined in the XmlDocument
let database_groups (doc : XmlDocument) =
   database_groups_nodes doc
   |> translate node_to_database_groups

/// Selects the Directory nodes in XmlDocument via XPath 
let directories_nodes (doc : XmlDocument) =
   doc.SelectNodes "/Configuration/IncludeDirectories/*"

/// Convert a XmlNode into a Directory class
let node_to_directory (node : XmlNode) =
   let path = match node.Attributes.["Name"] with
              | null  -> null
              | name  -> name.Value
   new System.IO.DirectoryInfo(path)

/// Retrieve all the directories defined in the XmlDocument
let directories (doc : XmlDocument) =
   directories_nodes doc
   |> translate node_to_directory
   
let all_include_directories(config_file : string) =
    // TODO: clean this up
    let doc = new XmlDocument()
    let reader = new System.IO.StreamReader(config_file);
    doc.LoadXml(reader.ReadToEnd())
    directories doc
    |> Seq.to_list