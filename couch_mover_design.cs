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
            // TODO: find a way to exit -    ask on reddit.
            return;
        }

        if(couchInterior == null && couchSurface == null)
        {
            ui.enableInsertButton();
            return;
        }

        double distance = couchCoarseDistance();
        ui.displayDistanceToMove(distance);
        if (distance*distance > 100)
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
        //return 50;
        int epoch = 0;

        StructureSet ss = m_context.StructureSet;
        int imagePlane = 20;
        double[] CT_couch_profile = new double[100];

        while (epoch++ < 10)
        {
            VVector[][] couchSurface_2D = couchSurface.GetContoursOnImagePlane(imagePlane);
            VVector closestPoint = new VVector(0, 0, 0);

            foreach (VVector vec1 in couchSurface_2D[0])     // 0, and 1 -  0 is outer and 1 is inner contour
            {
                if (closestPoint.y > vec1.y)
                {
                    closestPoint.y = vec1.y;
                    closestPoint.z = vec1.z;
                }
            }

            VVector farthestPoint = closestPoint;
            farthestPoint.y += 150;

            ss.Image.GetImageProfile(closestPoint, farthestPoint, CT_couch_profile);

            findPeaks(CT_couch_profile);
            double coarseDistance;
            if(verifyPeaks(out coarseDistance)==1)
            {
                    epoch = 10;
                    
                    // TODO: move to fine measurement, store closest_point + distancetomove
            }

            return coarseDistance;
        }


        return 100;
    }
    private static List<int> peaks = new List<int>();
    private static void findPeaks(double[] CT_couch_profile)
    {
        double previous = -1000;
        for (int i = 0; i < CT_couch_profile.Length; i++)
        {
            if (i != 0)
                previous = CT_couch_profile[i - 1];
            if (CT_couch_profile[i] > -700)
            {
                if (CT_couch_profile[i] > previous)
                {
                    if (CT_couch_profile[i] > CT_couch_profile[i + 1])
                    {
                        peaks.Add(i);
                    }
                }
            }
        }
    }

    private static int verifyPeaks(out double coarseDistance)
    {
        int coarseDetection = 0;
        coarseDistance = 0;
        for (int i = 0; i < peaks.Count - 1; i++)
        {
            double difference = peaks.ElementAt(i + 1) - peaks.ElementAt(i);
            if (difference > 8 && difference < 12)
            {
                coarseDetection++;
                coarseDistance = peaks.ElementAt(i) * 1.5;
            }
        }
        return coarseDetection;
    }


        private void couchCollisionCheck()
    {
        // TODO: check for distance
    }
  }
}
