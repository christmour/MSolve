using ISAAR.MSolve.PreProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ISAAR.MSolve.PreProcessor.Elements.SupportiveClasses;

namespace ISAAR.MSolve.PreProcessor
{
    public class nodalLoad
    {
        public Node Node { get; set; }
        public DOFType DOF { get; set; }
        public double Amount { get; set; }
    }

    public class DistributedLoadH8
    {
        public int ID { get; set; }
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public Node Node3 { get; set; }
        public Node Node4 { get; set; }
        public DOFType DOF { get; set; }
        public double AmountX { get; set; }
        public double AmountY { get; set; }
        public double AmountZ { get; set; }

        //public double[] Nodalloads;
        //public nodalLoad NodalLoad2 { get; set; }

        // public nodalLoad NodalLoad1 = new nodalLoad() { Amount = CalculateNodalForces[1], Node = model.Nodes[48], DOF = DOFType.Z };
        //public nodalLoad NodalLoad2 { get; set; }
        //public nodalLoad NodalLoad3 { get; set; }
        //public nodalLoad NodalLoad4 { get; set; }
        //public double[] CalculateNodalForces()
        //{
        //    return Nodalloads;
        //    NodalLoad2.Amount = Nodalloads[1];
        //}

        //int iInt = 2;
        //GaussLegendrePoint1D[] integrationPointsPerAxis =
        //        GaussQuadrature.GetGaussLegendrePoints(iInt);
        //int totalSamplingPoints = (int)Math.Pow(integrationPointsPerAxis.Length, 3);

        //GaussLegendrePoint3D[] integrationPoints = new GaussLegendrePoint3D[totalSamplingPoints];
        //private static int iInt;
    }




}
