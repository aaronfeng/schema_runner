#light
module LRF.SchemaRunner.Forms
open LRF.SchemaRunner.Core
open LRF.SchemaRunner.Configuration

open System
open System.Drawing
open System.Windows.Forms
open System.IO

type SchemaNode =
     | FileNode of FileInfo
     | DirectoryNode of DirectoryInfo
         
type SchemaItem  = 
    class 
        inherit TreeNode
        new(schema_type : SchemaNode) = { 
             inherit TreeNode(Text = (match schema_type with
                                      | FileNode(file) -> file.Name
                                      | DirectoryNode(dir) -> dir.Name),
                              Tag  = (match schema_type with
                                      | FileNode(file) -> file :> Object
                                      | DirectoryNode(dir) -> dir :> Object))
        }
        
        member self.Refresh() = 
            let dir = self.Tag :?> DirectoryInfo
            let files = dir.GetFiles("*.sql") |> Array.to_list
            List.iter (fun (file_info : FileInfo) -> 
                        let make_file(file_info : FileInfo) = new SchemaItem(FileNode(file_info))
                        let schema_file = make_file file_info
                        self.Nodes.Add(schema_file)
                        |> ignore)
                      files
                      
            let directories = dir.GetDirectories() 
                              |> Array.to_list 
                              |> List.filter (fun dir -> (dir.Attributes &&& FileAttributes.Hidden) <> FileAttributes.Hidden)
                
            List.iter (fun dir ->
                        let directory_node = new SchemaItem(DirectoryNode(dir))
                        directory_node.Refresh()
                        self.Nodes.Add(directory_node)
                        |> ignore) 
                      directories 
    end
 
type SchemaTree(directories : DirectoryInfo list option) as self =
    class
        inherit TreeView(Dock = DockStyle.Fill, CheckBoxes = true)
        let dirs = match directories with
                   | Some(d) -> d
                   | None    -> List.empty
                   
        do List.iter (fun d ->
                        let schema_item = new SchemaItem(DirectoryNode(d))
                        schema_item.Refresh()
                        self.Nodes.Add(schema_item) 
                        |> ignore)
                     dirs
    end
    
type DatabaseTree(database_groups : DatabaseGroup list option) as self = 
    class
         inherit TreeView(Dock = DockStyle.Fill, CheckBoxes = true)
         let groups = match database_groups with
                      | Some(g) -> g
                      | None    -> List.empty
                      
         let x = groups.Head.Name
         
         do List.iter (fun (g : DatabaseGroup) ->
                        let parent = new TreeNode(Text = g.Name, Tag = g)
                        self.Nodes.Add(parent) |> ignore
                        g.Databases.Value |> Seq.iter (fun (d : Database) -> 
                                                        d.LoadSchemaVersion() 
                                                        match d.SchemaVersion with
                                                        | "" | null -> new TreeNode(Text = "[" + d.Name + "]", Tag = d) |> parent.Nodes.Add |> ignore
                                                        | _ -> new TreeNode(Text = d.SchemaVersion + " " + d.Name, Tag = d) |> parent.Nodes.Add |> ignore))
                      groups
    end
      
type MainForm() as self = 
    class 
        inherit Form(Text = "F# SchemaRunner", Width = 780, Height = 560, MinimumSize = new Size(500, 300)) 
        
        let config = get_config  "SchemaInfo.xml"
        let directory_tree = match config.Directories with
                             | Some(d) -> new SchemaTree(Some(config.Directories.Value))
                             | None    -> new SchemaTree(None)
                             
        let database_tree = match config.DatabaseGroups with
                            | Some(d) -> new DatabaseTree(Some(config.DatabaseGroups.Value))
                            | None    -> new DatabaseTree(None)

        let vertical_split as v = new SplitContainer(Orientation = Orientation.Vertical,
                                                     Dock = DockStyle.Fill,
                                                     BorderStyle = BorderStyle.Fixed3D,
                                                     SplitterIncrement = 1,
                                                     SplitterDistance = 100)
        do v.Panel1.Controls.Add(directory_tree)
        do v.Panel2.Controls.Add(database_tree)

        let horizonal_split as h = new SplitContainer(Orientation = Orientation.Horizontal,
                                                      Dock = DockStyle.Fill,
                                                      SplitterIncrement = 1,
                                                      SplitterDistance = 400)
        do h.Panel1.Controls.Add(vertical_split)

        let master_container as m = new ToolStripContainer(Dock = DockStyle.Fill)
        do m.ContentPanel.Controls.Add(horizonal_split)
        
        do self.Controls.Add(master_container)        
    end 