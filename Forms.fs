#light

open System
open System.Drawing
open System.Windows.Forms

let directory_tree = new TreeView(Dock = DockStyle.Fill)

let database_tree = new TreeView(Dock = DockStyle.Fill)

let vertical_split = new SplitContainer()
vertical_split.Orientation <- Orientation.Vertical
vertical_split.Dock <- DockStyle.Fill
vertical_split.BorderStyle <- BorderStyle.Fixed3D
vertical_split.SplitterIncrement <- 1
vertical_split.SplitterDistance <- 100
vertical_split.BackColor <- Color.Aqua
vertical_split.Panel1.Controls.Add(directory_tree)
vertical_split.Panel2.Controls.Add(database_tree)

let horizonal_split = new SplitContainer()
horizonal_split.Orientation <- Orientation.Horizontal
horizonal_split.Dock <- DockStyle.Fill
horizonal_split.SplitterIncrement <- 1
horizonal_split.SplitterDistance <- 400
horizonal_split.Panel1.Controls.Add(vertical_split)
horizonal_split.BackColor <- Color.Blue

let master_container = new ToolStripContainer()
master_container.Dock <- DockStyle.Fill
master_container.ContentPanel.Controls.Add(horizonal_split)
      
        
let form = new Form()
form.Text <- "F# SchemaRunner"
form.Height <- 560
form.Width <- 786
form.MinimumSize <- new Size(500, 300)
form.Controls.Add(master_container)

#if INTERACTIVE
form.Show()
#endif

#if COMPILED
[<STAThread>]
do Application.Run(form)
#endif