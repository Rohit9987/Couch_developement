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
        window.Height = 700; window.Width = 400;
        window.Title = "LotusMoon";
        m_context = context;

        if (m_context == null)
            return;
        

        // enable buttons depending on the couch insertion/position
        enableButtons();
    }

    /*****************************ENABLE BUTTONS ON THE UI*****************************************/

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

        double distance = couchCoarseDistance();        // need to calculate the offset.
        ui.displayDistanceToMove(distance);
        if (distance*distance > 10)
        {
            ui.enableShiftButton();
            return;
        }

        ui.enableAcquireButton();
        couchCollisionCheck(distance);
    }
    
    /*****************************COUCH COARSE DISTANCE CALCULATOR*****************************************/
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
                    peaks.Clear();
                    return Math.Round(measureFine(closestPoint, coarseDistance), 2);
            }
            else
            {
                imagePlane += 10;
            }
        }
        return -100;
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

    /*****************************COUCH FINE DISTANCE CALCULATOR*****************************************/

    private static double measureFine(VVector initialPoint, double coarseDistance)
    {
        
        VVector point1 = initialPoint;
        point1.y += coarseDistance;
        VVector finalPoint = point1;
        
        int epoch = 0;
        const double stepSize = 0.1;
        double loss = 10000000;
        double[] CT_couch_profile_fine = new double[10];

        while(epoch++ < 10)
        {
            VVector point2 = point1;
            point2.y += 15;

            m_context.StructureSet.Image.GetImageProfile(point1, point2, CT_couch_profile_fine);
            double epochLoss = lossFunction(CT_couch_profile_fine);

            VVector point1_stepped = point1;
            point1_stepped.y += stepSize;
            point2.y += stepSize;
            m_context.StructureSet.Image.GetImageProfile(point1_stepped, point2, CT_couch_profile_fine);

            double epochLoss2 = lossFunction(CT_couch_profile_fine);
            double gradient = (epochLoss2 - epochLoss) / stepSize;

            double deltaY = -1.0 / (500*(epoch+1)) * stepSize * gradient;                    // TODO: improve this formula 
            //MessageBox.Show("Delta: " + deltaY + "\nLoss:" + epochLoss);
            if (epochLoss < loss)
            {
                loss = epochLoss;
                finalPoint = point1;
            }
            point1.y += deltaY;
        }
            
        return finalPoint.y - initialPoint.y;
    }

    //TODO: read the reference from a saved CSV file
    private static int[] reference = new int[] { -520, -673, -815, -836, -840, -840, -837, -801, -628, -500 };
    private static double lossFunction(double[] profile)
    {
        double loss = 0;            
        for (int i = 0; i < 10; i++)
        {
            loss += ((reference[i] - profile[i]) * (reference[i] - profile[i]));
        }
        return loss / 10;
    }

    /*****************************COUCH MOVE FUNCTIONS*****************************************/
    public void moveCouchsurface()
    {

        double shift = 30;
        Structure couchSurface = m_context.StructureSet.Structures.FirstOrDefault(id => id.Id.Contains("CouchSurface"));
        
        int nPlanes = m_context.Image.ZSize;

        for (int i = 0; i < nPlanes; i++)
        {
            VVector[][] couchSurface_2D = couchSurface.GetContoursOnImagePlane(i);

            if (couchSurface_2D != null && couchSurface_2D.Length > 0)
            {
                VVector[] outer_2D = couchSurface_2D[0];
                VVector[] inner_2D = couchSurface_2D[1];

                VVector[] new_outer_2D = new VVector[outer_2D.Length];
                VVector[] new_inner_2D = new VVector[inner_2D.Length];

                for (int l = 0; l < outer_2D.Length; l++)
                {
                    double newcoordy = outer_2D[l].y + shift;
                    new_outer_2D[l] = new VVector(outer_2D[l].x, newcoordy, 0);

                    if (l < inner_2D.Length)
                    {
                        newcoordy = inner_2D[l].y + shift;
                        new_inner_2D[l] = new VVector(inner_2D[l].x, newcoordy, 0);
                    }
                }

                couchSurface.ClearAllContoursOnImagePlane(i);
                couchSurface.AddContourOnImagePlane(new_outer_2D, i);
                couchSurface.SubtractContourOnImagePlane(new_inner_2D, i);

            }
        }
    }

    /*****************************COUCH COLLISION DETECTOR*****************************************/

    private void couchCollisionCheck(double yDistance)
    {
        double x = couchInterior.CenterPoint.x;
        string message = "";
        message += "X offset: " + Math.Round(x, 2) + ", " + "Y offset: " + yDistance + "\n";

        if(Math.Abs(x) > 5)
        {
            message += "Couch table is misplaced laterally!";
        }

        if (Math.Abs(yDistance) > 5)
        {
            message += "Couch table is misplaced vertically!";
        }

        ExternalPlanSetup plan = m_context.ExternalPlanSetup;
        PlanSum ps = m_context.PlanSum;
        if (plan != null)
        {
                message += verifyCollisionForPlan(plan);               
        }
        else if(ps != null)
        {                
            foreach (var plan1 in ps.PlanSetups)
            {
                message += "PLAN: " + plan1.Id + "\n";
                message += verifyCollisionForPlan((ExternalPlanSetup) plan1);
            }
        }
        else
        {
            // in case no plan or plan sum
            message += "No plan or plan sum loaded!";
        }
        
        ui.updateCollisionMessage(message);
    }
    
    private static string verifyCollisionForPlan(ExternalPlanSetup plan)
    {
        VVector isocenter = plan.Beams.First().IsocenterPosition;
        VVector[][] couchSurface_2D = couchSurface.GetContoursOnImagePlane(20);
        double maxDistanceLeft=0, maxDistanceRight = 0;
        string outputMessage = "";
        int collisionRight = 0, collisionLeft = 0;

        foreach (VVector vec1 in couchSurface_2D[0])
        {
            double distance = (isocenter.x - vec1.x) * (isocenter.x - vec1.x) + (isocenter.y - vec1.y) * (isocenter.y - vec1.y);
            distance = Math.Sqrt(distance);

            // for collision check
            if (vec1.x > 0 && distance > maxDistanceRight)
                maxDistanceRight = distance;
            if (vec1.x < 0 && distance > maxDistanceLeft)
                maxDistanceLeft = distance;            
        }
        outputMessage += "Max distance left: " + Math.Round(maxDistanceLeft, 2).ToString() + "\n";
        outputMessage += "Max distance right: " + Math.Round(maxDistanceRight, 2).ToString() + "\n";

            foreach (Beam beam in plan.Beams)
        {
            ControlPointCollection cp = beam.ControlPoints;
            foreach (ControlPoint cp1 in cp)
            {
                if (cp1.GantryAngle > 100 && cp1.GantryAngle < 180)
                {
                    if (maxDistanceRight > 350)
                    {
                        collisionRight = 2;
                    }
                    else if (maxDistanceRight > 335)
                    {
                        collisionRight = 1;
                    }
                }
                else if (cp1.GantryAngle > 180 && cp1.GantryAngle < 260)
                {
                    if (maxDistanceLeft > 350)
                    {
                        collisionLeft = 2;
                    }
                    else if (maxDistanceRight > 335)
                    {
                        collisionLeft = 1;
                    }
                }
            }
        }

        if(collisionLeft == 1)
        {
            outputMessage += "Warning! Possible collision on left side!\n";
        }
        else if(collisionLeft == 2)
        {
            outputMessage += "Error! Collision on left side!\n";
        }

        if (collisionRight == 1)
        {
            outputMessage += "Warning! Possible collision on right side!\n";
        }
        else if (collisionRight == 2)
        {
            outputMessage += "Error! Collision on right side!\n";
        }

        if (collisionLeft == 0 && collisionRight == 0)
        {
            outputMessage += "Clear! No collision detected!\n";
        }

        return outputMessage;
    }

  }
}
