#light
module LRF.SchemaRunner.Core
open System.Data.Common
open System.Data.SqlClient
open System.Data.OracleClient
open System.IO

type DatabaseType =
     | MsSql
     | Oracle
     override self.ToString() = 
        match self with
        | MsSql  -> "MsSql"
        | Oracle -> "Oracle"
     /// Convert a string into DatabaseType Union
     static member Convert (input : string) = 
        match input.ToLower() with
        | "mssql"  -> DatabaseType.MsSql
        | _        -> DatabaseType.Oracle

type Database(name : string, server : string, database_type : DatabaseType, user : string, password : string, schema_info : string) = 
    class
        let mutable schema_version : string = ""
   
        member self.Name          with get() = name
        member self.Server        with get() = server
        member self.DatabaseType  with get() = database_type
        member self.User          with get() = user
        member self.Password      with get() = password
        member self.SchemaInfo    with get() = schema_info
        member self.SchemaVersion with get() = schema_version

        member private self.GetConnection() : DbConnection =
             match self.DatabaseType with
             | MsSql -> new SqlConnection(self.ConnectionString()) :> DbConnection 
             | Oracle -> new OracleConnection(self.ConnectionString()) :> DbConnection 
            
        member private self.GetCommand(query, conn : DbConnection) : DbCommand =
             match self.DatabaseType with
             | MsSql -> new SqlCommand(query, (conn :?> SqlConnection)) :> DbCommand 
             | Oracle -> new OracleCommand(query, (conn :?> OracleConnection)) :> DbCommand 
            
        member self.ConnectionString() = 
             match self.DatabaseType with
             | MsSql  -> sprintf "SERVER=%A;Initial Catalog=%A;User Id=%A;PWD=%A" self.Server self.Name self.User self.Password
             | Oracle -> sprintf "Data Source=%A;User ID=%A;Password=%A;" self.Server self.User self.Password
        
        member self.LoadSchemaVersion() =
             let conn = self.GetConnection()
             let cmd = self.GetCommand("SELECT * FROM " + self.SchemaInfo, conn)
             let disposable = { new System.IDisposable with Dispose() = conn.Close() }
            
             try
                 use d = disposable
                 conn.Open()
                 schema_version <- cmd.ExecuteScalar().ToString()
             with
             | ex -> printfn "Oops, an exception has occured %s" ex.Message
                     schema_version <- ""
      end

type DatabaseGroup(name : string, databases : Database seq option) = 
    class
        member self.Name with get() = name
        member self.Databases with get() = databases    
    end