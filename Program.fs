#light
module LRF.SchemaRunner.Program
open LRF.SchemaRunner.Core
open LRF.SchemaRunner.Configuration
open LRF.SchemaRunner.Forms
open System.Xml
open System.Windows.Forms

let form = new MainForm()

#if INTERACTIVE
form.Show()
#endif

#if COMPILED
do Application.Run(form)
#endif