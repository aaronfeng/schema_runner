#light
module LRF.SchemaRunner.Core
open System.Data.SqlClient
open System.Data.OracleClient

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

type Database(name : string, server : string, database_type : DatabaseType, user : string, password : string, schema_info : string) = class
   let mutable schema_version : string = ""
   
   member self.Name          with get() = name
   member self.Server        with get() = server
   member self.DatabaseType  with get() = database_type
   member self.User          with get() = user
   member self.Password      with get() = password
   member self.SchemaInfo    with get() = schema_info
   member self.SchemaVersion with get() = schema_version
   
   member self.ConnectionString() = 
        match self.DatabaseType with
        | MsSql  -> sprintf "SERVER=%A;Initial Catalog=%A;User Id=%A;PWD=%A" self.Server self.Name self.User self.Password
        | Oracle -> sprintf "Data Source=%A;User ID=%A;Password=%A;" self.Server self.User self.Password
        
   member self.LoadSchemaVersion() =
        // TODO: switch on database type
        let conn = new SqlConnection(self.ConnectionString())
        let cmd = new SqlCommand("SELECT * FROM " + self.SchemaInfo, conn)
        let disposable = { new System.IDisposable with Dispose() = conn.Close() }
        
        try
            use d = disposable
            conn.Open()
            schema_version <- cmd.ExecuteScalar().ToString()
        with
        | ex -> printfn "Oops, an exception has occured %s" ex.Message
                schema_version <- ""
end

type DatabaseGroup(name : string, databases : Database seq option) = class
   member self.Name with get() = name
   member self.Databases with get() = databases
end

type Directory(name : string, alias : string) = class
   member self.Name  with get() = name
   member self.Alias with get() = alias
end