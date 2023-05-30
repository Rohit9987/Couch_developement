using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
  public class Script
  {
    private static ScriptContext m_context;
    private static Structure couchSurface, couchInterior;
    private static couch_mover_design.couch_mover_UI ui;

    public Script()
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Execute(ScriptContext context , System.Windows.Window window /*, ScriptEnvironment environment*/)
    {
        // TODO : Add here the code that is called when the script is launched from Eclipse.
        ui = new couch_mover_design.couch_mover_UI();
        window.Content = ui;
        window.Height = 500; window.Width = 400;
        m_context = context;

        // enable buttons depending on the couch insertion/position
        enableButtons();
    }

    private void enableButtons()
    {
        StructureSet ss = m_context.StructureSet;
        couchSurface = ss.Structures.FirstOrDefault(id => id.Id.Contains("CouchSurface"));
        couchInterior = ss.Structures.FirstOrDefault(id => id.Id.Contains("CouchInterior"));

        if(couchInterior==null ^ couchSurface==null)
        {
            MessageBox.Show("Error!\nOne of the couch structure is missing!");
            // TODO: find a way to exit
            return;
        }

        if(couchInterior == null && couchSurface == null)
        {
            ui.enableInsertButton();
            return;
        }

        double distance = couchCoarseDistance();
        if(distance*distance > 100)
        {
            ui.enableShiftButton();
            return;
        }

        ui.enableAcquireButton();
        couchCollisionCheck();
    }

    private static double couchCoarseDistance()
    {
        //TODO: find coarse distance to move
        return 50;
    }

    private void couchCollisionCheck()
    {
        // TODO: check for distance
    }
  }
}
