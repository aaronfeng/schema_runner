#light
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
   doc.SelectNodes "/IncludeDirectories/*"

/// Convert a XmlNode into a Directory class
let node_to_directory (node : XmlNode) =
   let path, alias = match node.Attributes.["Name"],
                           node.Attributes.["Alias"] with
                     | null, null  -> null, null
                     | name, null  -> name.Value, null
                     | name, alias -> name.Value, alias.Value
   new Directory(path, alias)

/// Retrieve all the directories defined in the XmlDocument
let directories (doc : XmlDocument) =
   directories_nodes doc
   |> translate node_to_directory