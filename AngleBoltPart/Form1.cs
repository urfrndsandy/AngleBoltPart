using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using T3D = Tekla.Structures.Geometry3d;
using Tekla.Structures;
using Tekla.Structures.Model;

namespace AngleBoltPart
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Model _model = new Model();
            ArrayList Poinlist = new ArrayList();

            int condition = 0;
            ModelInfo _minfo = _model.GetInfo();
            string repPath = _minfo.ModelPath + "\\Reports\\BoltPartCheck.xsr";
            //StreamWriter sw = new StreamWriter(repPath);
            //sw.WriteLine("Bolt Part Error's Report..");
            //sw.WriteLine("Part ID, Bolt ID");
            Tekla.Structures.Model.UI.ModelObjectSelector mos = new Tekla.Structures.Model.UI.ModelObjectSelector();
            TransformationPlane currplane = _model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            ModelObjectEnumerator moe = mos.GetSelectedObjects();
            Tekla.Structures.Model.ModelObjectSelector mosb = _model.GetModelObjectSelector();
            
            while (moe.MoveNext())
            {
                Beam angle = moe.Current as Beam;

                if (angle.StartPoint.Z > angle.EndPoint.Z)
                {
                    condition = 1;
                }

                ModelObjectEnumerator anglebolts = angle.GetBolts();
                _model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(angle.GetCoordinateSystem()));
                angle.Select();

                while(anglebolts.MoveNext())
                {
                    BoltArray Bolt = anglebolts.Current as BoltArray;

                    foreach (T3D.Point p in Bolt.BoltPositions)
                    {
                        Poinlist.Add(p);
                    }
                }
                T3D.Point Point1 = new T3D.Point();
                T3D.Point Point2 = new T3D.Point();
              

                T3D.Point pa = Poinlist[0] as T3D.Point;


                Point1.Z = 0;
                Point2.Z = 0;

                if (pa.X < 150)
                {
                    Point1 = pa;
                    Point2 = Poinlist[1] as T3D.Point;
                }

                else
                {
                    Point1 = Poinlist[1] as T3D.Point;
                    Point2 = pa;
                }

                angle.StartPoint = Point1;
                angle.StartPoint.X = angle.StartPoint.X - 50;
                angle.EndPoint = Point2;
                angle.EndPoint.X = angle.EndPoint.X + 50;

                angle.Modify();

                /*  switch (condition)
                  {
                      case 1:

                          angle.StartPoint = new T3D.Point(Poinlist[1] as T3D.Point);
                          angle.StartPoint.X = angle.StartPoint.X - 50;
                          angle.EndPoint = new T3D.Point(Poinlist[0] as T3D.Point);
                          angle.EndPoint.X = angle.EndPoint.X + 50;

                          angle.Modify();

                          break;

                      case 0:
                  angle.StartPoint =new T3D.Point(Poinlist[0] as T3D.Point);
                  angle.StartPoint.X = angle.StartPoint.X - 50;
                  angle.EndPoint = new T3D.Point(Poinlist[1] as T3D.Point);
                  angle.EndPoint.X = angle.EndPoint.X + 50;

                  angle.Modify();
                          break;*/


                Poinlist.Clear();


            }
            _model.GetWorkPlaneHandler().SetCurrentTransformationPlane(currplane);
            _model.CommitChanges();
            }

        private void button2_Click(object sender, EventArgs e)
        {
            Model _model = new Model();
          
            Tekla.Structures.Model.UI.ModelObjectSelector mos = new Tekla.Structures.Model.UI.ModelObjectSelector();
         
            ModelObjectEnumerator moe = mos.GetSelectedObjects();
           

            while (moe.MoveNext())
            {
                Part angle = moe.Current as Part;
                if (angle != null)
                {
                    ModelObjectEnumerator cuts = angle.GetBooleans();

                    while (cuts.MoveNext())
                    {

                        BooleanPart cutpart = cuts.Current as BooleanPart;
                        if (cutpart != null)
                        {
                            Part cutpt = cutpart.OperativePart;

                            if (cutpt.Profile.ProfileString.StartsWith("SPD"))
                            {
                                string prof = cutpt.Profile.ProfileString;

                              prof=  prof.Replace("SPD", "PIPE");
                                cutpt.Profile.ProfileString = prof;
                                cutpt.Modify();
                               
                            }

                            //string prof = cutpart.OperativePar .Profile.ProfileString;

                            //prof.Replace("SPD", "PIPE");
                            //cutpart.Profile.ProfileString = prof;
                            //cutpart.Modify();
                        }
                    }
                }
            }

            _model.CommitChanges();
            }
        }
}
