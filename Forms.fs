#light

open System
open System.Drawing
open System.Windows.Forms
      
type MainForm() as self = 
    class 
        inherit Form(Text = "F# SchemaRunner", Width = 786, Height = 560, MinimumSize = new Size(500, 300)) 
        let directory_tree = new TreeView(Dock = DockStyle.Fill)
        let database_tree = new TreeView(Dock = DockStyle.Fill)

        let vertical_split as v = new SplitContainer()
        do v.Orientation <- Orientation.Vertical
        do v.Dock <- DockStyle.Fill
        do v.BorderStyle <- BorderStyle.Fixed3D
        do v.SplitterIncrement <- 1
        do v.SplitterDistance <- 100
        do v.BackColor <- Color.Aqua
        do v.Panel1.Controls.Add(directory_tree)
        do v.Panel2.Controls.Add(database_tree)

        let horizonal_split as h = new SplitContainer()
        do h.Orientation <- Orientation.Horizontal
        do h.Dock <- DockStyle.Fill
        do h.SplitterIncrement <- 1
        do h.SplitterDistance <- 400
        do h.Panel1.Controls.Add(vertical_split)
        do h.BackColor <- Color.Blue

        let master_container as m = new ToolStripContainer()
        do m.Dock <- DockStyle.Fill
        do m.ContentPanel.Controls.Add(horizonal_split)
        
        do self.Controls.Add(master_container)
        do self.Show()
        
    end 
    
let form = new MainForm()

#if INTERACTIVE
form.Show()
#endif

#if COMPILED
[<STAThread>]
do Application.Run(form)
#endif