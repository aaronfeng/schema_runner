#light
module LRF.SchemaRunner.Forms

open System
open System.Drawing
open System.Windows.Forms
open LRF.SchemaRunner.Core
          
type FileItem (f : File) = 
    inherit TreeNode(f.Name)

type DirectoryItem (d : Directory) = 
    inherit TreeNode(d.Name)
    
type Tree() as self =
    class
        inherit TreeView(Dock = DockStyle.Fill)
        do self.Nodes.Add("test") |> ignore
    end
      
type MainForm() as self = 
    class 
        inherit Form(Text = "F# SchemaRunner", Width = 780, Height = 560, MinimumSize = new Size(500, 300)) 
        let directory_tree = new Tree()
        let database_tree = new Tree()

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
        do self.Show()
    end 
    
//let form = new MainForm()
//
//#if INTERACTIVE
//form.Show()
//#endif
//
//#if COMPILED
//[<STAThread>]
//do Application.Run(form)
//#endif