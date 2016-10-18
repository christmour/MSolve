using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Embedding;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;
using System.Runtime.InteropServices;
using ISAAR.MSolve.Matrices;
using ISAAR.MSolve.PreProcessor.Elements.SupportiveClasses;

namespace ISAAR.MSolve.PreProcessor.Elements
{
    public class Hexa20 : IStructuralFiniteElement, IEmbeddedHostElement
    {
        protected static double determinantTolerance = 0.00000001;
        protected static int iInt = 3;
        protected static int iInt2 = iInt * iInt;
        protected static int iInt3 = iInt2 * iInt;
        protected readonly static DOFType[] nodalDOFTypes = new DOFType[] { DOFType.X, DOFType.Y, DOFType.Z };
        protected readonly static DOFType[][] dofTypes = new DOFType[][] { nodalDOFTypes, nodalDOFTypes, nodalDOFTypes,
            nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes,
            nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes, nodalDOFTypes,
            nodalDOFTypes };
        protected readonly IFiniteElementMaterial3D[] materialsAtGaussPoints;
        protected IFiniteElementDOFEnumerator dofEnumerator = new GenericDOFEnumerator();

        #region Fortran imports
        [DllImport("femelements.dll",
            EntryPoint = "CALCH20GAUSSMATRICES",
            CallingConvention = CallingConvention.Cdecl)]
        protected static extern void CalcH20GaussMatrices(ref int iInt, [MarshalAs(UnmanagedType.LPArray)]double[,] faXYZ,
            [MarshalAs(UnmanagedType.LPArray)]double[] faWeight, [MarshalAs(UnmanagedType.LPArray)]double[,] faS,
            [MarshalAs(UnmanagedType.LPArray)]double[,] faDS, [MarshalAs(UnmanagedType.LPArray)]double[,,] faJ,
            [MarshalAs(UnmanagedType.LPArray)]double[] faDetJ, [MarshalAs(UnmanagedType.LPArray)]double[,,] faB);

        [DllImport("femelements.dll",
            EntryPoint = "CALCH20STRAINS",
            CallingConvention = CallingConvention.Cdecl)]
        protected static extern void CalcH20Strains(ref int iInt,
            [MarshalAs(UnmanagedType.LPArray)]double[,,] faB, [MarshalAs(UnmanagedType.LPArray)]double[] fau,
            [MarshalAs(UnmanagedType.LPArray)]double[,] faStrains);

        [DllImport("femelements.dll",
            EntryPoint = "CALCH20FORCES",
            CallingConvention = CallingConvention.Cdecl)]
        protected static extern void CalcH20Forces(ref int iInt,
            [MarshalAs(UnmanagedType.LPArray)]double[,,] faB, [MarshalAs(UnmanagedType.LPArray)]double[] faWeight,
            [MarshalAs(UnmanagedType.LPArray)]double[,] faStresses,
            [MarshalAs(UnmanagedType.LPArray)]double[] faForces);

        [DllImport("femelements.dll",
            EntryPoint = "CALCH20K",
            CallingConvention = CallingConvention.Cdecl)]
        protected static extern void CalcH20K(ref int iInt, [MarshalAs(UnmanagedType.LPArray)]double[,,] faE,
            [MarshalAs(UnmanagedType.LPArray)]double[,,] faB, [MarshalAs(UnmanagedType.LPArray)]double[] faWeight,
            [MarshalAs(UnmanagedType.LPArray)]double[] faK);

        [DllImport("femelements.dll",
            EntryPoint = "CALCH20MLUMPED",
            CallingConvention = CallingConvention.Cdecl)]
        protected static extern void CalcH20MLumped(ref int iInt, ref double fDensity,
            [MarshalAs(UnmanagedType.LPArray)]double[] faWeight, [MarshalAs(UnmanagedType.LPArray)]double[] faM);
        #endregion



        //public double[] CalcH20GaussMatrices(ref int iInt, double[,] faXYZ, double[] faWeight, double[,] faS, double[,] faDS, double[,,] faJ, double[] faDetJ, double[,,] faB)
        //{
        //    double[,] GP = new double[4, 4];
        //    GP[0, 0] = 0.0; GP[0, 1] = -0.5773502691896; GP[0, 2] = -0.7745966692415; GP[0, 3] = -0.8611363115941;
        //    GP[1, 0] = 0.0; GP[1, 1] = 0.5773502691896; GP[1, 2] = 0.0; GP[1, 3] = -0.3399810435849;
        //    GP[2, 0] = 0.0; GP[2, 1] = 0.0; GP[2, 2] = 0.7745966692415; GP[2, 3] = 0.3399810435849;
        //    GP[3, 0] = 0.0; GP[3, 1] = 0.0; GP[3, 2] = 0.0; GP[3, 3] = 0.8611363115941;

        //    double[,] GW = new double[4, 4];
        //    GW[0, 0] = 0.0; GW[0, 1] = 1.0; GW[0, 2] = 0.5555555555556; GP[0, 3] = 0.3478548451375;
        //    GW[1, 0] = 0.0; GW[1, 1] = 1.0; GW[1, 2] = 0.8888888888889; GP[1, 3] = 0.6521451548625;
        //    GW[2, 0] = 0.0; GW[2, 1] = 0.0; GW[2, 2] = 0.5555555555556; GP[2, 3] = 0.6521451548625;
        //    GW[3, 0] = 0.0; GW[3, 1] = 0.0; GW[3, 2] = 0.0; GP[3, 3] = 0.3478548451375;

        //    int M = 0;
        //    for (int iX = 0; iX < iInt; iX++)
        //    {
        //        double fXi = GP[iX, iInt];
        //        for (int iY = 0; iY < iInt; iY++)
        //        {
        //            double fEta = GP[iY, iInt];
        //            for (int iZ = 0; iZ < iInt; iZ++)
        //            {
        //                double fZeta = GP[iZ, iInt];
        //                M = M + 1;
        //                CalcH20NablaShape(fXi, fEta, fZeta);
        //                CalcH20Shape(fXi, fEta, fZeta);
        //                CalcH20JDetJ(faXYZ, faDS);
        //                CalculateDeformationMatrix(faJ, faDS);
        //                faWeight(M) = GW(iX, iInt) * GW(iY, iInt) * GW(iZ, iInt) * jacobian[M];
        //            }

        //        }
        //    }
        //    return faWeight;
        //}
        //public double[,] CalcH20Strains(ref int iInt, double[,,] faB, double[] fau, double[,] faStrains)
        //{
        //    for (int i = 0; i < Math.Pow(iInt,3); i++)
        //    {
        //        for (int k = 0; k < 6; k++)
        //        {
        //            faStrains[k, i] = 0.0;
        //            for (int j = 0; j < 60; j++)
        //            {
        //                faStrains[k, i] = faStrains[k, i] + faB[k, j, i] * fau[j];
        //            }
        //        }

        //    }
        //  return faStrains;
        //}

        //public double[] CalcH20Forces(ref int iInt,double[,,] faB,double[] faWeight,double[,] faStresses,double[] faForces)
        //{
        //    for (int i = 0; i < Math.Pow(iInt, 3); i++)
        //    {
        //        for (int k = 0; k < 6; k++)
        //        {
        //            faForces[k] = 0.0;
        //            for (int j = 0; j < 60; j++)
        //            {
        //                faForces[k] = faForces[k] + faB[j, k, i] * faStresses[j,i] * faWeight[i];
        //            }
        //        }

        //    }
        //    return faForces;
        //}

        protected Hexa20()
        {
        }

        public Hexa20(IFiniteElementMaterial3D material)
        {
            materialsAtGaussPoints = new IFiniteElementMaterial3D[iInt3];
            for (int i = 0; i < iInt3; i++)
                materialsAtGaussPoints[i] = (IFiniteElementMaterial3D)material.Clone();
        }

        public Hexa20(IFiniteElementMaterial3D material, IFiniteElementDOFEnumerator dofEnumerator)
            : this(material)
        {
            this.dofEnumerator = dofEnumerator;
        }

        public IFiniteElementDOFEnumerator DOFEnumerator
        {
            get { return dofEnumerator; }
            set { dofEnumerator = value; }
        }

        public double Density { get; set; }
        public double RayleighAlpha { get; set; }
        public double RayleighBeta { get; set; }

        protected double[,] GetCoordinates(Element element)
        {
            double[,] faXYZ = new double[dofTypes.Length, 3];
            for (int i = 0; i < dofTypes.Length; i++)
            {
                faXYZ[i, 0] = element.Nodes[i].X;
                faXYZ[i, 1] = element.Nodes[i].Y;
                faXYZ[i, 2] = element.Nodes[i].Z;
            }
            return faXYZ;
        }

        protected double[,] GetCoordinatesTranspose(Element element)
        {
            double[,] faXYZ = new double[3, dofTypes.Length];
            for (int i = 0; i < dofTypes.Length; i++)
            {
                faXYZ[0, i] = element.Nodes[i].X;
                faXYZ[1, i] = element.Nodes[i].Y;
                faXYZ[2, i] = element.Nodes[i].Z;
            }
            return faXYZ;
        }

        #region IElementType Members

        public int ID
        {
            get { return 11; }
        }

        public ElementDimensions ElementDimensions
        {
            get { return ElementDimensions.ThreeD; }
        }

        public virtual IList<IList<DOFType>> GetElementDOFTypes(Element element)
        {
            return dofTypes;
        }

        public IList<Node> GetNodesForMatrixAssembly(Element element)
        {
            return element.Nodes;
        }

        private double[] CalcH20Shape(double fXi, double fEta, double fZeta)
        {
            const double fSqC125 = 0.5;
            double fXiP = (1.0 + fXi) * fSqC125;
            double fEtaP = (1.0 + fEta) * fSqC125;
            double fZetaP = (1.0 + fZeta) * fSqC125;
            double fXiM = (1.0 - fXi) * fSqC125;
            double fEtaM = (1.0 - fEta) * fSqC125;
            double fZetaM = (1.0 - fZeta) * fSqC125;
            double fXi2 = (1.0 - fXi * fXi);
            double fEta2 = (1.0 - fEta * fEta);
            double fZeta2 = (1.0 - fZeta * fZeta);

            return new double[]
            {
                //fXiP  * fEtaP * fZetaP * ( fXi + fEta + fZeta - 2.0),
                //fXiM  * fEtaP * fZetaP * (-fXi + fEta + fZeta - 2.0),
                //fXiM  * fEtaM * fZetaP * (-fXi - fEta + fZeta - 2.0),
                //fXiP  * fEtaM * fZetaP * (fXi - fEta + fZeta - 2.0),
                //fXiP  * fEtaP * fZetaM * (fXi + fEta - fZeta - 2.0),
                //fXiM  * fEtaP * fZetaM * (-fXi + fEta - fZeta - 2.0),
                //fXiM  * fEtaM * fZetaM * (-fXi - fEta - fZeta - 2.0),
                //fXiP  * fEtaM * fZetaM * (fXi - fEta - fZeta - 2.0),

                //fXi2   * fEtaP * fZetaP,
                //fEta2  * fXiM  * fZetaP,
                //fXi2   * fEtaM * fZetaP,
                //fEta2  * fXiP  * fZetaP,
                //fXi2   * fEtaP * fZetaM,
                //fEta2  * fXiM  * fZetaM,
                //fXi2   * fEtaM * fZetaM,
                //fEta2  * fXiP  * fZetaM,
                //fZeta2 * fXiP  * fEtaP,
                //fZeta2 * fXiM  * fEtaP,
                //fZeta2 * fXiM  * fEtaM,
                //fZeta2 * fXiP  * fEtaM,
                fXiM * fEtaM * fZetaM * (-fXi - fEta - fZeta - 2.0),

                fXiP * fEtaM * fZetaM * (fXi - fEta - fZeta - 2.0),

                fXiP * fEtaP * fZetaM * (fXi + fEta - fZeta - 2.0),

                fXiM * fEtaP * fZetaM * (-fXi + fEta - fZeta - 2.0),

                fXiM * fEtaM * fZetaP * (-fXi - fEta + fZeta - 2.0),

                fXiP * fEtaM * fZetaP * (fXi - fEta + fZeta - 2.0),

                fXiP * fEtaP * fZetaP * (fXi + fEta + fZeta - 2.0),

                fXiM * fEtaP * fZetaP * (-fXi + fEta + fZeta - 2.0),

                fXi2 * fEtaM * fZetaM,

                fEta2 * fXiP * fZetaM,

                fXi2 * fEtaP * fZetaM,

                fEta2 * fXiM * fZetaM,

                fXi2 * fEtaM * fZetaP,

                fEta2 * fXiP * fZetaP,

                fXi2 * fEtaP * fZetaP,

                fEta2 * fXiM * fZetaP,

                fZeta2 * fXiM * fEtaM,

                fZeta2 * fXiP * fEtaM,

                fZeta2 * fXiP * fEtaP,

                fZeta2 * fXiM * fEtaP,
            };
        }

        private double[] CalcH20NablaShape(double fXi, double fEta, double fZeta)
        {
            //double f1 = fXi;
            //double f2 = fEta;
            //double f3 = fZeta;

            //double[,] fadhval = new double[20,3];
            double fXiP = (1.0 + fXi);
            double fEtaP = (1.0 + fEta);
            double fZetaP = (1.0 + fZeta);
            double fXiM = (1.0 - fXi);
            double fEtaM = (1.0 - fEta);
            double fZetaM = (1.0 - fZeta);

            double[] faDS = new double[60];
            //// node number 20
            //fadhval[19,0] = (1.0 - f2) * (1.0 - f3 * f3) * 0.25;
            //fadhval[19, 1] = -(1.0 + f1) * (1.0 - f3 * f3) * 0.25;
            //fadhval[19, 2] = -(1.0 + f1) * (1.0 - f2) * f3 * 0.50;
            //// node number 19
            //fadhval[18, 0] = -(1.0 - f2) * (1.0 - f3 * f3) * 0.25;
            //fadhval[18, 1] = -(1.0 - f1) * (1.0 - f3 * f3) * 0.25;
            //fadhval[18, 2] = -(1.0 - f1) * (1.0 - f2) * f3 * 0.50;
            //// node number 18
            //fadhval[17, 0] = -(1.0 + f2) * (1.0 - f3 * f3) * 0.25;
            //fadhval[17, 1] = (1.0 - f1) * (1.0 - f3 * f3) * 0.25;
            //fadhval[17, 2] = -(1.0 - f1) * (1.0 + f2) * f3 * 0.50;
            //// node number 17
            //fadhval[16, 0] = (1.0 + f2) * (1.0 - f3 * f3) * 0.25;
            //fadhval[16, 1] = (1.0 + f1) * (1.0 - f3 * f3) * 0.25;
            //fadhval[16, 2] = -(1.0 + f1) * (1.0 + f2) * f3 * 0.50;

            //// node number 16
            //fadhval[15, 0] = (1.0 - f2 * f2) * (1.0 - f3) * 0.25;
            //fadhval[15, 1] = -(1.0 + f1) * f2 * (1.0 - f3) * 0.50;
            //fadhval[15, 2] = -(1.0 + f1) * (1.0 - f2 * f2) * 0.25;
            //// node number 15
            //fadhval[14, 0] = -f1 * (1.0 - f2) * (1.0 - f3) * 0.50;
            //fadhval[14, 1] = -(1.0 - f1 * f1) * (1.0 - f3) * 0.25;
            //fadhval[14, 2] = -(1.0 - f1 * f1) * (1.0 - f2) * 0.25;
            //// node number 14
            //fadhval[13, 0] = -(1.0 - f2 * f2) * (1.0 - f3) * 0.25;
            //fadhval[13, 1] = -(1.0 - f1) * f2 * (1.0 - f3) * 0.50;
            //fadhval[13, 2] = -(1.0 - f1) * (1.0 - f2 * f2) * 0.25;
            //// node number 13
            //fadhval[12, 0] = -f1 * (1.0 + f2) * (1.0 - f3) * 0.50;
            //fadhval[12, 1] = (1.0 - f1 * f1) * (1.0 - f3) * 0.25;
            //fadhval[12, 2] = -(1.0 - f1 * f1) * (1.0 + f2) * 0.25;

            //// node number 12
            //fadhval[11, 0] = (1.0 - f2 * f2) * (1.0 + f3) * 0.25;
            //fadhval[11, 1] = -(1.0 + f1) * f2 * (1.0 + f3) * 0.50;
            //fadhval[11, 2] = (1.0 + f1) * (1.0 - f2 * f2) * 0.25;
            ////node number 11
            //fadhval[10, 0] = -f1 * (1.0 - f2) * (1.0 + f3) * 0.50;
            //fadhval[10, 1] = -(1.0 - f1 * f1) * (1.0 + f3) * 0.25;
            //fadhval[10, 2] = (1.0 - f1 * f1) * (1.0 - f2) * 0.25;
            //// node number 10
            //fadhval[9, 0] = -(1.0 - f2 * f2) * (1.0 + f3) * 0.25;
            //fadhval[9, 1] = -(1.0 - f1) * f2 * (1.0 + f3) * 0.50;
            //fadhval[9, 2] = (1.0 - f1) * (1.0 - f2 * f2) * 0.25;
            //// node number 9
            //fadhval[8, 0] = -f1 * (1.0 + f2) * (1.0 + f3) * 0.50;
            //fadhval[8, 2] = (1.0 - f1 * f1) * (1.0 + f3) * 0.25;
            //fadhval[8, 3] = (1.0 - f1 * f1) * (1.0 + f2) * 0.25;

            //// node number 8
            //fadhval[7, 0] = (1.0 - f2) * (1.0 - f3) * 0.125 - (fadhval[15, 1] + fadhval[16, 1] + fadhval[20, 1]) * 0.50;
            //fadhval[7, 1] = -(1.0 + f1) * (1.0 - f3) * 0.125 - (fadhval[15, 2] + fadhval[16, 2] + fadhval[20, 2]) * 0.50;
            //fadhval[7, 2] = -(1.0 + f1) * (1.0 - f2) * 0.125 - (fadhval[15, 3] + fadhval[16, 3] + fadhval[20, 3]) * 0.50;
            //// node number 7
            //fadhval[6, 0] = -(1.0 - f2) * (1.0 - f3) * 0.125 - (fadhval[14, 1] + fadhval[15, 1] + fadhval[19, 1]) * 0.50;
            //fadhval[6, 1] = -(1.0 - f1) * (1.0 - f3) * 0.125 - (fadhval[14, 2] + fadhval[15, 2] + fadhval[19, 2]) * 0.50;
            //fadhval[6, 2] = -(1.0 - f1) * (1.0 - f2) * 0.125 - (fadhval[14, 3] + fadhval[15, 3] + fadhval[19, 3]) * 0.50;
            //// node number 6
            //fadhval[5, 0] = -(1.0 + f2) * (1.0 - f3) * 0.125 - (fadhval[13, 1] + fadhval[14, 1] + fadhval[18, 1]) * 0.50;
            //fadhval[5, 1] = (1.0 - f1) * (1.0 - f3) * 0.125 - (fadhval[13, 2] + fadhval[14, 2] + fadhval[18, 2]) * 0.50;
            //fadhval[5, 2] = -(1.0 - f1) * (1.0 + f2) * 0.125 - (fadhval[13, 3] + fadhval[14, 3] + fadhval[18, 3]) * 0.50;
            //// node number 5
            //fadhval[4, 0] = (1.0 + f2) * (1.0 - f3) * 0.125 - (fadhval[13, 1] + fadhval[16, 1] + fadhval[17, 1]) * 0.50;
            //fadhval[4, 1] = (1.0 + f1) * (1.0 - f3) * 0.125 - (fadhval[13, 2] + fadhval[16, 2] + fadhval[17, 2]) * 0.50;
            //fadhval[4, 2] = -(1.0 + f1) * (1.0 + f2) * 0.125 - (fadhval[13, 3] + fadhval[16, 3] + fadhval[17, 3]) * 0.50;

            //// node number 4
            //fadhval[3, 0] = (1.0 - f2) * (1.0 + f3) * 0.125 - (fadhval[11, 1] + fadhval[12, 1] + fadhval[20, 1]) * 0.50;
            //fadhval[3, 1] = -(1.0 + f1) * (1.0 + f3) * 0.125 - (fadhval[11, 2] + fadhval[12, 2] + fadhval[20, 2]) * 0.50;
            //fadhval[3, 2] = (1.0 + f1) * (1.0 - f2) * 0.125 - (fadhval[11, 3] + fadhval[12, 3] + fadhval[20, 3]) * 0.50;
            //// node number 3
            //fadhval[2, 0] = -(1.0 - f2) * (1.0 + f3) * 0.125 - (fadhval[10, 1] + fadhval[11, 1] + fadhval[19, 1]) * 0.50;
            //fadhval[2, 1] = -(1.0 - f1) * (1.0 + f3) * 0.125 - (fadhval[10, 2] + fadhval[11, 2] + fadhval[19, 2]) * 0.50;
            //fadhval[2, 2] = (1.0 - f1) * (1.0 - f2) * 0.125 - (fadhval[10, 3] + fadhval[11, 3] + fadhval[19, 3]) * 0.50;
            //// number 2
            //fadhval[1, 0] = -(1.0 + f2) * (1.0 + f3) * 0.125 - (fadhval[10, 1] + fadhval[18, 1] + fadhval[9, 1]) * 0.50;
            //fadhval[1, 1] = (1.0 - f1) * (1.0 + f3) * 0.125 - (fadhval[10, 2] + fadhval[18, 2] + fadhval[9, 2]) * 0.50;
            //fadhval[1, 2] = (1.0 - f1) * (1.0 + f2) * 0.125 - (fadhval[10, 3] + fadhval[18, 3] + fadhval[9, 3]) * 0.50;
            //// node number 1
            //fadhval[0, 0] = (1.0 + f2) * (1.0 + f3) * 0.125 - (fadhval[12, 1] + fadhval[17, 1] + fadhval[9, 1]) * 0.50;
            //fadhval[0, 1] = (1.0 + f1) * (1.0 + f3) * 0.125 - (fadhval[12, 2] + fadhval[17, 2] + fadhval[9, 2]) * 0.50;
            //fadhval[0, 2] = (1.0 + f1) * (1.0 + f2) * 0.125 - (fadhval[12, 3] + fadhval[17, 3] + fadhval[9, 3]) * 0.50;

            //double[] faDS = new double[60];
            //for (int i = 0; i < 20; i++)
            //{
            //    faDS[i] = fadhval[i, 1];
            //    faDS[20+i] = fadhval[i, 2];
            //    faDS[40+i] = fadhval[i, 3];
            //}
            // Calculation of natural coordinate derivatives of the shape functions
            // Corresponding to fXi

            faDS[0] = 0.125 * fEtaM * fZetaM * (2.0 * fXi + fEta + fZeta + 1.0);
            faDS[1] = 0.125 * fEtaM * fZetaM * (2.0 * fXi - fEta - fZeta - 1.0);
            faDS[2] = 0.125 * fEtaP * fZetaM * (2.0 * fXi + fEta - fZeta - 1.0);
            faDS[3] = 0.125 * fEtaP * fZetaM * (2.0 * fXi - fEta + fZeta + 1.0);
            faDS[4] = 0.125 * fEtaM * fZetaP * (2.0 * fXi + fEta - fZeta + 1.0);
            faDS[5] = 0.125 * fEtaM * fZetaP * (2.0 * fXi - fEta + fZeta - 1.0);
            faDS[6] = 0.125 * fEtaP * fZetaP * (2.0 * fXi + fEta + fZeta - 1.0);
            faDS[7] = 0.125 * fEtaP * fZetaP * (2.0 * fXi - fEta - fZeta + 1.0);
            faDS[8] = -0.5 * fXi * fEtaM * fZetaM;

            faDS[9] = 0.25 * fEtaM * fEtaP * fZetaM;

            faDS[10] = -0.5 * fXi * fEtaP * fZetaM;

            faDS[11] = -0.25 * fEtaM * fEtaP * fZetaM;

            faDS[12] = -0.5 * fXi * fEtaM * fZetaP;

            faDS[13] = 0.25 * fEtaM * fEtaP * fZetaP;

            faDS[14] = -0.5 * fXi * fEtaP * fZetaP;

            faDS[15] = -0.25 * fEtaM * fEtaP * fZetaP;

            faDS[16] = -0.25 * fEtaM * fZetaP * fZetaM;

            faDS[17] = 0.25 * fEtaM * fZetaP * fZetaM;

            faDS[18] = 0.25 * fEtaP * fZetaP * fZetaM;

            faDS[19] = -0.25 * fEtaP * fZetaP * fZetaM;

            // Corresponding to fEta

            faDS[20] = 0.125 * fXiM * fZetaM * (fXi + 2.0 * fEta + fZeta + 1.0);
            faDS[21] = 0.125 * fXiP * fZetaM * (-fXi + 2.0 * fEta + fZeta + 1.0);
            faDS[22] = 0.125 * fXiP * fZetaM * (fXi + 2.0 * fEta - fZeta - 1.0);
            faDS[23] = 0.125 * fXiM * fZetaM * (-fXi + 2.0 * fEta - fZeta - 1.0);
            faDS[24] = 0.125 * fXiM * fZetaP * (fXi + 2.0 * fEta - fZeta + 1.0);
            faDS[25] = 0.125 * fXiP * fZetaP * (-fXi + 2.0 * fEta - fZeta + 1.0);
            faDS[26] = 0.125 * fXiP * fZetaP * (fXi + 2.0 * fEta + fZeta - 1.0);
            faDS[27] = 0.125 * fXiM * fZetaP * (-fXi + 2.0 * fEta + fZeta - 1.0);
            faDS[28] = -0.25 * fXiP * fXiM * fZetaM;

            faDS[29] = -0.5 * fXiP * fEta * fZetaM;

            faDS[30] = 0.25 * fXiM * fXiP * fZetaM;

            faDS[31] = -0.5 * fXiM * fEta * fZetaM;

            faDS[32] = -0.25 * fXiP * fXiM * fZetaP;

            faDS[33] = -0.5 * fEta * fXiP * fZetaP;

            faDS[34] = 0.25 * fXiM * fXiP * fZetaP;

            faDS[35] = -0.5 * fXiM * fEta * fZetaP;

            faDS[36] = -0.25 * fXiM * fZetaP * fZetaM;

            faDS[37] = -0.25 * fXiP * fZetaP * fZetaM;

            faDS[38] = 0.25 * fXiP * fZetaP * fZetaM;

            faDS[39] = 0.25 * fXiM * fZetaP * fZetaM;

            // Corresponding to fZeta

            faDS[40] = 0.125 * fXiM * fEtaM * (fXi + fEta + 2.0 * fZeta + 1.0);
            faDS[41] = 0.125 * fXiP * fEtaM * (-fXi + fEta + 2.0 * fZeta + 1.0);
            faDS[42] = 0.125 * fXiP * fEtaP * (-fXi - fEta + 2.0 * fZeta + 1.0);
            faDS[43] = 0.125 * fXiM * fEtaP * (fXi - fEta + 2.0 * fZeta + 1.0);
            faDS[44] = 0.125 * fXiM * fEtaM * (-fXi - fEta + 2.0 * fZeta - 1.0);
            faDS[45] = 0.125 * fXiP * fEtaM * (fXi - fEta + 2.0 * fZeta - 1.0);
            faDS[46] = 0.125 * fXiP * fEtaP * (fXi + fEta + 2.0 * fZeta - 1.0);
            faDS[47] = 0.125 * fXiM * fEtaP * (-fXi + fEta + 2.0 * fZeta - 1.0);
            faDS[48] = -0.25 * fXiP * fXiM * fEtaM;

            faDS[49] = -0.25 * fXiP * fEtaP * fEtaM;

            faDS[50] = -0.25 * fXiM * fXiP * fEtaP;

            faDS[51] = -0.25 * fXiM * fEtaP * fEtaM;

            faDS[52] = 0.25 * fXiP * fXiM * fEtaM;

            faDS[53] = 0.25 * fXiP * fEtaP * fEtaM;

            faDS[54] = 0.25 * fXiM * fXiP * fEtaP;

            faDS[55] = 0.25 * fXiM * fEtaM * fEtaP;

            faDS[56] = -0.5 * fXiM * fEtaM * fZeta;

            faDS[57] = -0.5 * fXiP * fEtaM * fZeta;

            faDS[58] = -0.5 * fXiP * fEtaP * fZeta;

            faDS[59] = -0.5 * fXiM * fEtaP * fZeta;


            return faDS;
        }

        private Tuple<double[,], double[,], double> CalcH20JDetJ(double[,] faXYZ, double[] faDS)
        {
            double[,] faJ = new double[3, 3];
            faJ[0, 0] = faDS[0] * faXYZ[0, 0] + faDS[1] * faXYZ[0, 1] + faDS[2] * faXYZ[0, 2] + faDS[3] * faXYZ[0, 3] + faDS[4] * faXYZ[0, 4] + faDS[5] * faXYZ[0, 5] + faDS[6] * faXYZ[0, 6] + faDS[7] * faXYZ[0, 7]
               + faDS[8] * faXYZ[0, 8] + faDS[9] * faXYZ[0, 9] + faDS[10] * faXYZ[0, 10] + faDS[11] * faXYZ[0, 11] + faDS[12] * faXYZ[0, 12] + faDS[13] * faXYZ[0, 13] + faDS[14] * faXYZ[0, 14] + faDS[15] * faXYZ[0, 15]
               + faDS[16] * faXYZ[0, 16] + faDS[17] * faXYZ[0, 17] + faDS[18] * faXYZ[0, 18] + faDS[19] * faXYZ[0, 19];
            faJ[0, 1] = faDS[0] * faXYZ[1, 0] + faDS[1] * faXYZ[1, 1] + faDS[2] * faXYZ[1, 2] + faDS[3] * faXYZ[1, 3] + faDS[4] * faXYZ[1, 4] + faDS[5] * faXYZ[1, 5] + faDS[6] * faXYZ[1, 6] + faDS[7] * faXYZ[1, 7]
               + faDS[8] * faXYZ[1, 8] + faDS[9] * faXYZ[1, 9] + faDS[10] * faXYZ[1, 10] + faDS[11] * faXYZ[1, 11] + faDS[12] * faXYZ[1, 12] + faDS[13] * faXYZ[1, 13] + faDS[14] * faXYZ[1, 14] + faDS[15] * faXYZ[1, 15]
               + faDS[16] * faXYZ[1, 16] + faDS[17] * faXYZ[1, 17] + faDS[18] * faXYZ[1, 18] + faDS[19] * faXYZ[1, 19];
            faJ[0, 2] = faDS[0] * faXYZ[2, 0] + faDS[1] * faXYZ[2, 1] + faDS[2] * faXYZ[2, 2] + faDS[3] * faXYZ[2, 3] + faDS[4] * faXYZ[2, 4] + faDS[5] * faXYZ[2, 5] + faDS[6] * faXYZ[2, 6] + faDS[7] * faXYZ[2, 7]
               + faDS[8] * faXYZ[2, 8] + faDS[9] * faXYZ[2, 9] + faDS[10] * faXYZ[2, 10] + faDS[11] * faXYZ[2, 11] + faDS[12] * faXYZ[2, 12] + faDS[13] * faXYZ[2, 13] + faDS[14] * faXYZ[2, 14] + faDS[15] * faXYZ[2, 15]
               + faDS[16] * faXYZ[2, 16] + faDS[17] * faXYZ[2, 17] + faDS[18] * faXYZ[2, 18] + faDS[19] * faXYZ[2, 19];

            faJ[1, 0] = faDS[20] * faXYZ[0, 0] + faDS[21] * faXYZ[0, 1] + faDS[22] * faXYZ[0, 2] + faDS[23] * faXYZ[0, 3] + faDS[24] * faXYZ[0, 4] + faDS[25] * faXYZ[0, 5] + faDS[26] * faXYZ[0, 6] + faDS[27] * faXYZ[0, 7]
               + faDS[28] * faXYZ[0, 8] + faDS[29] * faXYZ[0, 9] + faDS[30] * faXYZ[0, 10] + faDS[31] * faXYZ[0, 11] + faDS[32] * faXYZ[0, 12] + faDS[33] * faXYZ[0, 13] + faDS[34] * faXYZ[0, 14] + faDS[35] * faXYZ[0, 15]
               + faDS[36] * faXYZ[0, 16] + faDS[37] * faXYZ[0, 17] + faDS[38] * faXYZ[0, 18] + faDS[39] * faXYZ[0, 19];
            faJ[1, 1] = faDS[20] * faXYZ[1, 0] + faDS[21] * faXYZ[1, 1] + faDS[22] * faXYZ[1, 2] + faDS[23] * faXYZ[1, 3] + faDS[24] * faXYZ[1, 4] + faDS[25] * faXYZ[1, 5] + faDS[26] * faXYZ[1, 6] + faDS[27] * faXYZ[1, 7]
                + faDS[28] * faXYZ[1, 8] + faDS[29] * faXYZ[1, 9] + faDS[30] * faXYZ[1, 10] + faDS[31] * faXYZ[1, 11] + faDS[32] * faXYZ[1, 12] + faDS[33] * faXYZ[1, 13] + faDS[34] * faXYZ[1, 14] + faDS[35] * faXYZ[1, 15]
                + faDS[36] * faXYZ[1, 16] + faDS[37] * faXYZ[1, 17] + faDS[38] * faXYZ[1, 18] + faDS[39] * faXYZ[1, 19];
            faJ[1, 2] = faDS[20] * faXYZ[2, 0] + faDS[21] * faXYZ[2, 1] + faDS[22] * faXYZ[2, 2] + faDS[23] * faXYZ[2, 3] + faDS[24] * faXYZ[2, 4] + faDS[25] * faXYZ[2, 5] + faDS[26] * faXYZ[2, 6] + faDS[27] * faXYZ[2, 7]
                + faDS[28] * faXYZ[2, 8] + faDS[29] * faXYZ[2, 9] + faDS[30] * faXYZ[2, 10] + faDS[31] * faXYZ[2, 11] + faDS[32] * faXYZ[2, 12] + faDS[33] * faXYZ[2, 13] + faDS[34] * faXYZ[2, 14] + faDS[35] * faXYZ[2, 15]
                + faDS[36] * faXYZ[2, 16] + faDS[37] * faXYZ[2, 17] + faDS[38] * faXYZ[2, 18] + faDS[39] * faXYZ[2, 19];

            faJ[2, 0] = faDS[40] * faXYZ[0, 0] + faDS[41] * faXYZ[0, 1] + faDS[42] * faXYZ[0, 2] + faDS[43] * faXYZ[0, 3] + faDS[44] * faXYZ[0, 4] + faDS[45] * faXYZ[0, 5] + faDS[46] * faXYZ[0, 6] + faDS[47] * faXYZ[0, 7]
                + faDS[48] * faXYZ[0, 8] + faDS[49] * faXYZ[0, 9] + faDS[50] * faXYZ[0, 10] + faDS[51] * faXYZ[0, 11] + faDS[52] * faXYZ[0, 12] + faDS[53] * faXYZ[0, 13] + faDS[54] * faXYZ[0, 14] + faDS[55] * faXYZ[0, 15]
                + faDS[56] * faXYZ[0, 16] + faDS[57] * faXYZ[0, 17] + faDS[58] * faXYZ[0, 18] + faDS[59] * faXYZ[0, 19];
            faJ[2, 1] = faDS[40] * faXYZ[1, 0] + faDS[41] * faXYZ[1, 1] + faDS[42] * faXYZ[1, 2] + faDS[43] * faXYZ[1, 3] + faDS[44] * faXYZ[1, 4] + faDS[45] * faXYZ[1, 5] + faDS[46] * faXYZ[1, 6] + faDS[47] * faXYZ[1, 7]
                + faDS[48] * faXYZ[1, 8] + faDS[49] * faXYZ[1, 9] + faDS[50] * faXYZ[1, 10] + faDS[51] * faXYZ[1, 11] + faDS[52] * faXYZ[1, 12] + faDS[53] * faXYZ[1, 13] + faDS[54] * faXYZ[1, 14] + faDS[55] * faXYZ[1, 15]
                + faDS[56] * faXYZ[1, 16] + faDS[57] * faXYZ[1, 17] + faDS[58] * faXYZ[1, 18] + faDS[59] * faXYZ[1, 19];
            faJ[2, 2] = faDS[40] * faXYZ[2, 0] + faDS[41] * faXYZ[2, 1] + faDS[42] * faXYZ[2, 2] + faDS[43] * faXYZ[2, 3] + faDS[44] * faXYZ[2, 4] + faDS[45] * faXYZ[2, 5] + faDS[46] * faXYZ[2, 6] + faDS[47] * faXYZ[2, 7]
                 + faDS[48] * faXYZ[2, 8] + faDS[49] * faXYZ[2, 9] + faDS[50] * faXYZ[2, 10] + faDS[51] * faXYZ[2, 11] + faDS[52] * faXYZ[2, 12] + faDS[53] * faXYZ[2, 13] + faDS[54] * faXYZ[2, 14] + faDS[55] * faXYZ[2, 15]
                 + faDS[56] * faXYZ[2, 16] + faDS[57] * faXYZ[2, 17] + faDS[58] * faXYZ[2, 18] + faDS[59] * faXYZ[2, 19];


            double fDet1 = faJ[0, 0] * (faJ[1, 1] * faJ[2, 2] - faJ[2, 1] * faJ[1, 2]);
            double fDet2 = -faJ[0, 1] * (faJ[1, 0] * faJ[2, 2] - faJ[2, 0] * faJ[1, 2]);
            double fDet3 = faJ[0, 2] * (faJ[1, 0] * faJ[2, 1] - faJ[2, 0] * faJ[1, 1]);
            double fDetJ = fDet1 + fDet2 + fDet3;
            if (fDetJ < determinantTolerance)
                throw new ArgumentException(String.Format("Jacobian determinant is negative or under tolerance ({0} < {1}). Check the order of nodes or the element geometry.", fDetJ, determinantTolerance));

            double fDetInv = 1.0 / fDetJ;
            double[,] faJInv = new double[3, 3];
            faJInv[0, 0] = (faJ[1, 1] * faJ[2, 2] - faJ[2, 1] * faJ[1, 2]) * fDetInv;
            faJInv[1, 0] = (faJ[2, 0] * faJ[1, 2] - faJ[1, 0] * faJ[2, 2]) * fDetInv;
            faJInv[2, 0] = (faJ[1, 0] * faJ[2, 1] - faJ[2, 0] * faJ[1, 1]) * fDetInv;
            faJInv[0, 1] = (faJ[2, 1] * faJ[0, 2] - faJ[0, 1] * faJ[2, 2]) * fDetInv;
            faJInv[1, 1] = (faJ[0, 0] * faJ[2, 2] - faJ[2, 0] * faJ[0, 2]) * fDetInv;
            faJInv[2, 1] = (faJ[2, 0] * faJ[0, 1] - faJ[2, 1] * faJ[0, 0]) * fDetInv;
            faJInv[0, 2] = (faJ[0, 1] * faJ[1, 2] - faJ[1, 1] * faJ[0, 2]) * fDetInv;
            faJInv[1, 2] = (faJ[1, 0] * faJ[0, 2] - faJ[0, 0] * faJ[1, 2]) * fDetInv;
            faJInv[2, 2] = (faJ[0, 0] * faJ[1, 1] - faJ[1, 0] * faJ[0, 1]) * fDetInv;

            return new Tuple<double[,], double[,], double>(faJ, faJInv, fDetJ);
        }

        private double[,] CalculateDeformationMatrix(
            Jacobian3D jacobian, ShapeFunctionNaturalDerivatives3D[] shapeFunctionDerivatives)
        {
            double[,] jacobianInverse = jacobian.CalculateJacobianInverse();
            double[,] b = new double[8, 60];

            for (int shapeFunction = 0; shapeFunction < 20; shapeFunction++)
            {
                b[0, (3 * shapeFunction) + 0] = (jacobianInverse[0, 0] * shapeFunctionDerivatives[shapeFunction].Xi) +
                                                (jacobianInverse[0, 1] * shapeFunctionDerivatives[shapeFunction].Eta) +
                                                (jacobianInverse[0, 2] * shapeFunctionDerivatives[shapeFunction].Zeta);
                b[1, (3 * shapeFunction) + 1] = (jacobianInverse[1, 0] * shapeFunctionDerivatives[shapeFunction].Xi) +
                                                (jacobianInverse[1, 1] * shapeFunctionDerivatives[shapeFunction].Eta) +
                                                (jacobianInverse[1, 2] * shapeFunctionDerivatives[shapeFunction].Zeta);
                b[2, (3 * shapeFunction) + 2] = (jacobianInverse[2, 0] * shapeFunctionDerivatives[shapeFunction].Xi) +
                                                (jacobianInverse[2, 1] * shapeFunctionDerivatives[shapeFunction].Eta) +
                                                (jacobianInverse[2, 2] * shapeFunctionDerivatives[shapeFunction].Zeta);
                b[3, (3 * shapeFunction) + 0] = b[1, (3 * shapeFunction) + 1];
                b[3, (3 * shapeFunction) + 1] = b[0, (3 * shapeFunction) + 0];
                b[4, (3 * shapeFunction) + 1] = b[2, (3 * shapeFunction) + 2];
                b[4, (3 * shapeFunction) + 2] = b[1, (3 * shapeFunction) + 1];
                b[5, (3 * shapeFunction) + 0] = b[2, (3 * shapeFunction) + 2];
                b[5, (3 * shapeFunction) + 2] = b[0, (3 * shapeFunction) + 0];
            }

            return b;
        }


        private ShapeFunctionNaturalDerivatives3D[] CalculateShapeDerivativeValues(
            double xi, double eta, double zeta)
        {
            ShapeFunctionNaturalDerivatives3D[] shapeFunctionDerivatives =
                new ShapeFunctionNaturalDerivatives3D[20];
            for (int shapeFunction = 0; shapeFunction < 20; shapeFunction++)
            {
                shapeFunctionDerivatives[shapeFunction] = new ShapeFunctionNaturalDerivatives3D();
            }


            double xiP = (1.0 + xi);
            double etaP = (1.0 + eta);
            double zetaP = (1.0 + zeta);
            double xiM = (1.0 - xi);
            double etaM = (1.0 - eta);
            double zetaM = (1.0 - zeta);

            //// node number 20
            //shapeFunctionDerivatives[19].Xi = (1.0 - f2) * (1.0 - f3 * f3) * 0.25;
            //shapeFunctionDerivatives[19].Eta = -(1.0 + f1) * (1.0 - f3 * f3) * 0.25;
            //shapeFunctionDerivatives[19].Zeta = -(1.0 + f1) * (1.0 - f2) * f3 * 0.50;
            //// node number 19
            //shapeFunctionDerivatives[18].Xi = -(1.0 - f2) * (1.0 - f3 * f3) * 0.25;
            //shapeFunctionDerivatives[18].Eta = -(1.0 - f1) * (1.0 - f3 * f3) * 0.25;
            //shapeFunctionDerivatives[18].Zeta = -(1.0 - f1) * (1.0 - f2) * f3 * 0.50;
            //// node number 18
            //shapeFunctionDerivatives[17].Xi = -(1.0 + f2) * (1.0 - f3 * f3) * 0.25;
            //shapeFunctionDerivatives[17].Eta = (1.0 - f1) * (1.0 - f3 * f3) * 0.25;
            //shapeFunctionDerivatives[17].Zeta = -(1.0 - f1) * (1.0 + f2) * f3 * 0.50;
            //// node number 17
            //shapeFunctionDerivatives[16].Xi = (1.0 + f2) * (1.0 - f3 * f3) * 0.25;
            //shapeFunctionDerivatives[16].Eta = (1.0 + f1) * (1.0 - f3 * f3) * 0.25;
            //shapeFunctionDerivatives[16].Zeta = -(1.0 + f1) * (1.0 + f2) * f3 * 0.50;

            //// node number 16
            //shapeFunctionDerivatives[15].Xi = (1.0 - f2 * f2) * (1.0 - f3) * 0.25;
            //shapeFunctionDerivatives[15].Eta = -(1.0 + f1) * f2 * (1.0 - f3) * 0.50;
            //shapeFunctionDerivatives[15].Zeta = -(1.0 + f1) * (1.0 - f2 * f2) * 0.25;
            //// node number 15
            //shapeFunctionDerivatives[14].Xi = -f1 * (1.0 - f2) * (1.0 - f3) * 0.50;
            //shapeFunctionDerivatives[14].Eta = -(1.0 - f1 * f1) * (1.0 - f3) * 0.25;
            //shapeFunctionDerivatives[14].Zeta = -(1.0 - f1 * f1) * (1.0 - f2) * 0.25;
            //// node number 14
            //shapeFunctionDerivatives[13].Xi = -(1.0 - f2 * f2) * (1.0 - f3) * 0.25;
            //shapeFunctionDerivatives[13].Eta = -(1.0 - f1) * f2 * (1.0 - f3) * 0.50;
            //shapeFunctionDerivatives[13].Zeta = -(1.0 - f1) * (1.0 - f2 * f2) * 0.25;
            //// node number 13
            //shapeFunctionDerivatives[12].Xi = -f1 * (1.0 + f2) * (1.0 - f3) * 0.50;
            //shapeFunctionDerivatives[12].Eta = (1.0 - f1 * f1) * (1.0 - f3) * 0.25;
            //shapeFunctionDerivatives[12].Zeta = -(1.0 - f1 * f1) * (1.0 + f2) * 0.25;

            //// node number 12
            //shapeFunctionDerivatives[11].Xi = (1.0 - f2 * f2) * (1.0 + f3) * 0.25;
            //shapeFunctionDerivatives[11].Eta = -(1.0 + f1) * f2 * (1.0 + f3) * 0.50;
            //shapeFunctionDerivatives[11].Zeta = (1.0 + f1) * (1.0 - f2 * f2) * 0.25;
            ////node number 11
            //shapeFunctionDerivatives[10].Xi = -f1 * (1.0 - f2) * (1.0 + f3) * 0.50;
            //shapeFunctionDerivatives[10].Eta = -(1.0 - f1 * f1) * (1.0 + f3) * 0.25;
            //shapeFunctionDerivatives[10].Zeta = (1.0 - f1 * f1) * (1.0 - f2) * 0.25;
            //// node number 10
            //shapeFunctionDerivatives[9].Xi = -(1.0 - f2 * f2) * (1.0 + f3) * 0.25;
            //shapeFunctionDerivatives[9].Eta = -(1.0 - f1) * f2 * (1.0 + f3) * 0.50;
            //shapeFunctionDerivatives[9].Zeta = (1.0 - f1) * (1.0 - f2 * f2) * 0.25;
            //// node number 9
            //shapeFunctionDerivatives[8].Xi = -f1 * (1.0 + f2) * (1.0 + f3) * 0.50;
            //shapeFunctionDerivatives[8].Eta = (1.0 - f1 * f1) * (1.0 + f3) * 0.25;
            //shapeFunctionDerivatives[8].Zeta = (1.0 - f1 * f1) * (1.0 + f2) * 0.25;

            //// node number 8
            //shapeFunctionDerivatives[7].Xi = (1.0 - f2) * (1.0 - f3) * 0.125 - (shapeFunctionDerivatives[14].Xi + shapeFunctionDerivatives[15].Xi + shapeFunctionDerivatives[19].Xi) * 0.50;
            //shapeFunctionDerivatives[7].Eta = -(1.0 + f1) * (1.0 - f3) * 0.125 - (shapeFunctionDerivatives[14].Eta + shapeFunctionDerivatives[15].Eta + shapeFunctionDerivatives[19].Eta) * 0.50;
            //shapeFunctionDerivatives[7].Zeta = -(1.0 + f1) * (1.0 - f2) * 0.125 - (shapeFunctionDerivatives[14].Zeta + shapeFunctionDerivatives[15].Zeta + shapeFunctionDerivatives[19].Zeta) * 0.50;
            //// node number 7
            //shapeFunctionDerivatives[6].Xi = -(1.0 - f2) * (1.0 - f3) * 0.125 - (shapeFunctionDerivatives[13].Xi + shapeFunctionDerivatives[14].Xi + shapeFunctionDerivatives[18].Xi) * 0.50;
            //shapeFunctionDerivatives[6].Eta = -(1.0 - f1) * (1.0 - f3) * 0.125 - (shapeFunctionDerivatives[13].Eta + shapeFunctionDerivatives[14].Eta + shapeFunctionDerivatives[18].Eta) * 0.50;
            //shapeFunctionDerivatives[6].Zeta = -(1.0 - f1) * (1.0 - f2) * 0.125 - (shapeFunctionDerivatives[13].Zeta + shapeFunctionDerivatives[14].Zeta + shapeFunctionDerivatives[18].Zeta) * 0.50;
            //// node number 6
            //shapeFunctionDerivatives[5].Xi = -(1.0 + f2) * (1.0 - f3) * 0.125 - (shapeFunctionDerivatives[12].Xi + shapeFunctionDerivatives[13].Xi + shapeFunctionDerivatives[17].Xi) * 0.50;
            //shapeFunctionDerivatives[5].Eta = (1.0 - f1) * (1.0 - f3) * 0.125 - (shapeFunctionDerivatives[12].Eta + shapeFunctionDerivatives[13].Eta + shapeFunctionDerivatives[17].Eta) * 0.50;
            //shapeFunctionDerivatives[5].Zeta = -(1.0 - f1) * (1.0 + f2) * 0.125 - (shapeFunctionDerivatives[12].Zeta + shapeFunctionDerivatives[13].Zeta + shapeFunctionDerivatives[17].Zeta) * 0.50;
            //// node number 5
            //shapeFunctionDerivatives[4].Xi = (1.0 + f2) * (1.0 - f3) * 0.125 - (shapeFunctionDerivatives[12].Xi + shapeFunctionDerivatives[15].Xi + shapeFunctionDerivatives[16].Xi) * 0.50;
            //shapeFunctionDerivatives[4].Eta = (1.0 + f1) * (1.0 - f3) * 0.125 - (shapeFunctionDerivatives[12].Eta + shapeFunctionDerivatives[15].Eta + shapeFunctionDerivatives[16].Eta) * 0.50;
            //shapeFunctionDerivatives[4].Zeta = -(1.0 + f1) * (1.0 + f2) * 0.125 - (shapeFunctionDerivatives[12].Zeta + shapeFunctionDerivatives[15].Zeta + shapeFunctionDerivatives[16].Zeta) * 0.50;

            //// node number 4
            //shapeFunctionDerivatives[3].Xi = (1.0 - f2) * (1.0 + f3) * 0.125 - (shapeFunctionDerivatives[10].Xi + shapeFunctionDerivatives[11].Xi + shapeFunctionDerivatives[19].Xi) * 0.50;
            //shapeFunctionDerivatives[3].Eta = -(1.0 + f1) * (1.0 + f3) * 0.125 - (shapeFunctionDerivatives[10].Eta + shapeFunctionDerivatives[11].Eta + shapeFunctionDerivatives[19].Eta) * 0.50;
            //shapeFunctionDerivatives[3].Zeta = (1.0 + f1) * (1.0 - f2) * 0.125 - (shapeFunctionDerivatives[10].Zeta + shapeFunctionDerivatives[11].Zeta + shapeFunctionDerivatives[19].Zeta) * 0.50;
            //// node number 3
            //shapeFunctionDerivatives[2].Xi = -(1.0 - f2) * (1.0 + f3) * 0.125 - (shapeFunctionDerivatives[9].Xi + shapeFunctionDerivatives[10].Xi + shapeFunctionDerivatives[18].Xi) * 0.50;
            //shapeFunctionDerivatives[2].Eta = -(1.0 - f1) * (1.0 + f3) * 0.125 - (shapeFunctionDerivatives[9].Eta + shapeFunctionDerivatives[10].Eta + shapeFunctionDerivatives[18].Eta) * 0.50;
            //shapeFunctionDerivatives[2].Zeta = (1.0 - f1) * (1.0 - f2) * 0.125 - (shapeFunctionDerivatives[9].Zeta + shapeFunctionDerivatives[10].Zeta + shapeFunctionDerivatives[18].Zeta) * 0.50;
            //// number 2
            //shapeFunctionDerivatives[1].Xi = -(1.0 + f2) * (1.0 + f3) * 0.125 - (shapeFunctionDerivatives[9].Xi + shapeFunctionDerivatives[17].Xi + shapeFunctionDerivatives[8].Xi) * 0.50;
            //shapeFunctionDerivatives[1].Eta = (1.0 - f1) * (1.0 + f3) * 0.125 - (shapeFunctionDerivatives[9].Eta + shapeFunctionDerivatives[17].Eta + shapeFunctionDerivatives[8].Eta) * 0.50;
            //shapeFunctionDerivatives[1].Zeta = (1.0 - f1) * (1.0 + f2) * 0.125 - (shapeFunctionDerivatives[9].Zeta + shapeFunctionDerivatives[17].Zeta + shapeFunctionDerivatives[8].Zeta) * 0.50;
            //// node number 1
            //shapeFunctionDerivatives[0].Xi = (1.0 + f2) * (1.0 + f3) * 0.125 - (shapeFunctionDerivatives[11].Xi + shapeFunctionDerivatives[16].Xi + shapeFunctionDerivatives[8].Xi) * 0.50;
            //shapeFunctionDerivatives[0].Eta = (1.0 + f1) * (1.0 + f3) * 0.125 - (shapeFunctionDerivatives[11].Eta + shapeFunctionDerivatives[16].Eta + shapeFunctionDerivatives[8].Eta) * 0.50;
            //shapeFunctionDerivatives[0].Zeta = (1.0 + f1) * (1.0 + f2) * 0.125 - (shapeFunctionDerivatives[11].Zeta + shapeFunctionDerivatives[16].Zeta + shapeFunctionDerivatives[8].Zeta) * 0.50;

            // node number 1
            //shapeFunctionDerivatives[0].Xi = 0.125 * etaM * zetaM * (2.0 * xi + eta + zeta + 1.0);
            //shapeFunctionDerivatives[0].Eta = 0.125 * xiM * zetaM * (xi + 2.0 * eta + zeta + 1.0);
            //shapeFunctionDerivatives[0].Zeta = 0.125 * xiM * etaM * (xi + eta + 2.0 * zeta + 1.0);
            //// node number 2
            //shapeFunctionDerivatives[1].Xi = 0.125 * etaM * zetaM * (2.0 * xi - eta - zeta - 1.0);
            //shapeFunctionDerivatives[1].Eta = 0.125 * xiP * zetaM * (-xi + 2.0 * eta + zeta + 1.0);
            //shapeFunctionDerivatives[1].Zeta = 0.125 * xiP * etaM * (-xi + eta + 2.0 * zeta + 1.0);
            //// node number 3
            //shapeFunctionDerivatives[2].Xi = 0.125 * etaP * zetaM * (2.0 * xi + eta - zeta - 1.0);
            //shapeFunctionDerivatives[2].Eta = 0.125 * xiP * zetaM * (xi + 2.0 * eta - zeta - 1.0);
            //shapeFunctionDerivatives[2].Zeta = 0.125 * xiP * etaP * (-xi - eta + 2.0 * zeta + 1.0);
            //// node number 4
            //shapeFunctionDerivatives[3].Xi = 0.125 * etaP * zetaM * (2.0 * xi - eta + zeta + 1.0);
            //shapeFunctionDerivatives[3].Eta = 0.125 * xiM * zetaM * (-xi + 2.0 * eta - zeta - 1.0);
            //shapeFunctionDerivatives[3].Zeta = 0.125 * xiM * etaP * (xi - eta + 2.0 * zeta + 1.0);

            //// node number 5
            //shapeFunctionDerivatives[4].Xi = 0.125 * etaM * zetaP * (2.0 * xi + eta - zeta + 1.0);
            //shapeFunctionDerivatives[4].Eta = 0.125 * xiM * zetaP * (xi + 2.0 * eta - zeta + 1.0);
            //shapeFunctionDerivatives[4].Zeta = 0.125 * xiM * etaM * (-xi - eta + 2.0 * zeta - 1.0);
            //// node number 6
            //shapeFunctionDerivatives[5].Xi = 0.125 * etaM * zetaP * (2.0 * xi - eta + zeta - 1.0);
            //shapeFunctionDerivatives[5].Eta = 0.125 * xiP * zetaP * (-xi + 2.0 * eta - zeta + 1.0);
            //shapeFunctionDerivatives[5].Zeta = 0.125 * xiP * etaM * (xi - eta + 2.0 * zeta - 1.0); 
            //// node number 7
            //shapeFunctionDerivatives[6].Xi = 0.125 * etaP * zetaP * (2.0 * xi + eta + zeta - 1.0);
            //shapeFunctionDerivatives[6].Eta = 0.125 * xiP * zetaP * (xi + 2.0 * eta + zeta - 1.0);
            //shapeFunctionDerivatives[6].Zeta = 0.125 * xiP * etaP * (xi + eta + 2.0 * zeta - 1.0);
            //// node number 8
            //shapeFunctionDerivatives[7].Xi = 0.125 * etaP * zetaP * (2.0 * xi - eta - zeta + 1.0);
            //shapeFunctionDerivatives[7].Eta = 0.125 * xiM * zetaP * (-xi + 2.0 * eta + zeta - 1.0);
            //shapeFunctionDerivatives[7].Zeta = 0.125 * xiM * etaP * (-xi + eta + 2.0 * zeta - 1.0);

            //// node number 9
            //shapeFunctionDerivatives[8].Xi = -0.5 * xi * etaM * zetaM;
            //shapeFunctionDerivatives[8].Eta = -0.25 * xiP * xiM * zetaM;
            //shapeFunctionDerivatives[8].Zeta = -0.25 * xiP * xiM * etaM;
            ////node number 10
            //shapeFunctionDerivatives[9].Xi = 0.25 * etaM * etaP * zetaM;
            //shapeFunctionDerivatives[9].Eta = -0.5 * xiP * eta * zetaM;
            //shapeFunctionDerivatives[9].Zeta = -0.25 * xiP * etaP * etaM;
            //// node number 11
            //shapeFunctionDerivatives[10].Xi = -0.5 * xi * etaP * zetaM;
            //shapeFunctionDerivatives[10].Eta = 0.25 * xiM * xiP * zetaM;
            //shapeFunctionDerivatives[10].Zeta = -0.25 * xiM * xiP * etaP;
            //// node number 12
            //shapeFunctionDerivatives[11].Xi = -0.25 * etaM * etaP * zetaM;
            //shapeFunctionDerivatives[11].Eta = -0.5 * xiM * eta * zetaM;
            //shapeFunctionDerivatives[11].Zeta = -0.25 * xiM * etaP * etaM;

            //// node number 13
            //shapeFunctionDerivatives[12].Xi = -0.5 * xi * etaM * zetaP;
            //shapeFunctionDerivatives[12].Eta = -0.25 * xiP * xiM * zetaP;
            //shapeFunctionDerivatives[12].Zeta = 0.25 * xiP * xiM * etaM;
            //// node number 14
            //shapeFunctionDerivatives[13].Xi = 0.25 * etaM * etaP * zetaP;
            //shapeFunctionDerivatives[13].Eta = -0.5 * eta * xiP * zetaP;
            //shapeFunctionDerivatives[13].Zeta = 0.25 * xiP * etaP * etaM;
            //// node number 15
            //shapeFunctionDerivatives[14].Xi = -0.5 * xi * etaP * zetaP;
            //shapeFunctionDerivatives[14].Eta = 0.25 * xiM * xiP * zetaP;
            //shapeFunctionDerivatives[14].Zeta = 0.25 * xiM * xiP * etaP;
            //// node number 16
            //shapeFunctionDerivatives[15].Xi = -0.25 * etaM * etaP * zetaP;
            //shapeFunctionDerivatives[15].Eta = -0.5 * xiM * eta * zetaP;
            //shapeFunctionDerivatives[15].Zeta = 0.25 * xiM * etaM * etaP;

            //// node number 17
            //shapeFunctionDerivatives[16].Xi = -0.25 * etaM * zetaP * zetaM;
            //shapeFunctionDerivatives[16].Eta = -0.25 * xiM * zetaP * zetaM;
            //shapeFunctionDerivatives[16].Zeta = -0.5 * xiM * etaM * zeta;
            //// node number 18
            //shapeFunctionDerivatives[17].Xi = 0.25 * etaM * zetaP * zetaM;
            //shapeFunctionDerivatives[17].Eta = -0.25 * xiP * zetaP * zetaM;
            //shapeFunctionDerivatives[17].Zeta = -0.5 * xiP * etaM * zeta;
            //// number 19
            //shapeFunctionDerivatives[18].Xi = 0.25 * etaP * zetaP * zetaM;
            //shapeFunctionDerivatives[18].Eta = 0.25 * xiP * zetaP * zetaM;
            //shapeFunctionDerivatives[18].Zeta = -0.5 * xiP * etaP * zeta;
            //// node number 20
            //shapeFunctionDerivatives[19].Xi = -0.25 * etaP * zetaP * zetaM;
            //shapeFunctionDerivatives[19].Eta = 0.25 * xiM * zetaP * zetaM; 
            //shapeFunctionDerivatives[19].Zeta = -0.5 * xiM * etaP * zeta;


            // Corresponding to xi
            shapeFunctionDerivatives[0].Xi = 0.125 * etaM * zetaM * (2 * xi + eta + zeta + 1);
            shapeFunctionDerivatives[1].Xi = 0.125 * etaM * zetaM * (2 * xi - eta - zeta - 1);
            shapeFunctionDerivatives[2].Xi = 0.125 * etaP * zetaM * (2 * xi + eta - zeta - 1);
            shapeFunctionDerivatives[3].Xi = 0.125 * etaP * zetaM * (2 * xi - eta + zeta + 1);
            shapeFunctionDerivatives[4].Xi = 0.125 * etaM * zetaP * (2 * xi + eta - zeta + 1);
            shapeFunctionDerivatives[5].Xi = 0.125 * etaM * zetaP * (2 * xi - eta + zeta - 1);
            shapeFunctionDerivatives[6].Xi = 0.125 * etaP * zetaP * (2 * xi + eta + zeta - 1);
            shapeFunctionDerivatives[7].Xi = 0.125 * etaP * zetaP * (2 * xi - eta - zeta + 1);
            shapeFunctionDerivatives[8].Xi = -0.5 * xi * etaM * zetaM;
            shapeFunctionDerivatives[9].Xi = 0.25 * etaM * etaP * zetaM;
            shapeFunctionDerivatives[10].Xi = -0.5 * xi * etaP * zetaM;
            shapeFunctionDerivatives[11].Xi = -0.25 * etaM * etaP * zetaM;
            shapeFunctionDerivatives[12].Xi = -0.5 * xi * etaM * zetaP;
            shapeFunctionDerivatives[13].Xi = 0.25 * etaM * etaP * zetaP;
            shapeFunctionDerivatives[14].Xi = -0.5 * xi * etaP * zetaP;
            shapeFunctionDerivatives[15].Xi = -0.25 * etaM * etaP * zetaP;
            shapeFunctionDerivatives[16].Xi = -0.25 * etaM * zetaP * zetaM;
            shapeFunctionDerivatives[17].Xi = 0.25 * etaM * zetaP * zetaM;
            shapeFunctionDerivatives[18].Xi = 0.25 * etaP * zetaP * zetaM;
            shapeFunctionDerivatives[19].Xi = -0.25 * etaP * zetaP * zetaM;

            // Corresponding to eta
            shapeFunctionDerivatives[0].Eta = 0.125 * xiM * zetaM * (xi + 2 * eta + zeta + 1);
            shapeFunctionDerivatives[1].Eta = 0.125 * xiP * zetaM * (-xi + 2 * eta + zeta + 1);
            shapeFunctionDerivatives[2].Eta = 0.125 * xiP * zetaM * (xi + 2 * eta - zeta - 1);
            shapeFunctionDerivatives[3].Eta = 0.125 * xiM * zetaM * (-xi + 2 * eta - zeta - 1);
            shapeFunctionDerivatives[4].Eta = 0.125 * xiM * zetaP * (xi + 2 * eta - zeta + 1);
            shapeFunctionDerivatives[5].Eta = 0.125 * xiP * zetaP * (-xi + 2 * eta - zeta + 1);
            shapeFunctionDerivatives[6].Eta = 0.125 * xiP * zetaP * (xi + 2 * eta + zeta - 1);
            shapeFunctionDerivatives[7].Eta = 0.125 * xiM * zetaP * (-xi + 2 * eta + zeta - 1);
            shapeFunctionDerivatives[8].Eta = -0.25 * xiP * xiM * zetaM;
            shapeFunctionDerivatives[9].Eta = -0.5 * xiP * eta * zetaM;
            shapeFunctionDerivatives[10].Eta = 0.25 * xiM * xiP * zetaM;
            shapeFunctionDerivatives[11].Eta = -0.5 * xiM * eta * zetaM;
            shapeFunctionDerivatives[12].Eta = -0.25 * xiP * xiM * zetaP;
            shapeFunctionDerivatives[13].Eta = -0.5 * eta * xiP * zetaP;
            shapeFunctionDerivatives[14].Eta = 0.25 * xiM * xiP * zetaP;
            shapeFunctionDerivatives[15].Eta = -0.5 * xiM * eta * zetaP;
            shapeFunctionDerivatives[16].Eta = -0.25 * xiM * zetaP * zetaM;
            shapeFunctionDerivatives[17].Eta = -0.25 * xiP * zetaP * zetaM;
            shapeFunctionDerivatives[18].Eta = 0.25 * xiP * zetaP * zetaM;
            shapeFunctionDerivatives[19].Eta = 0.25 * xiM * zetaP * zetaM;

            // Corresponding to zeta
            shapeFunctionDerivatives[0].Zeta = 0.125 * xiM * etaM * (xi + eta + 2 * zeta + 1);
            shapeFunctionDerivatives[1].Zeta = 0.125 * xiP * etaM * (-xi + eta + 2 * zeta + 1);
            shapeFunctionDerivatives[2].Zeta = 0.125 * xiP * etaP * (-xi - eta + 2 * zeta + 1);
            shapeFunctionDerivatives[3].Zeta = 0.125 * xiM * etaP * (xi - eta + 2 * zeta + 1);
            shapeFunctionDerivatives[4].Zeta = 0.125 * xiM * etaM * (-xi - eta + 2 * zeta - 1);
            shapeFunctionDerivatives[5].Zeta = 0.125 * xiP * etaM * (xi - eta + 2 * zeta - 1);
            shapeFunctionDerivatives[6].Zeta = 0.125 * xiP * etaP * (xi + eta + 2 * zeta - 1);
            shapeFunctionDerivatives[7].Zeta = 0.125 * xiM * etaP * (-xi + eta + 2 * zeta - 1);
            shapeFunctionDerivatives[8].Zeta = -0.25 * xiP * xiM * etaM;
            shapeFunctionDerivatives[9].Zeta = -0.25 * xiP * etaP * etaM;
            shapeFunctionDerivatives[10].Zeta = -0.25 * xiM * xiP * etaP;
            shapeFunctionDerivatives[11].Zeta = -0.25 * xiM * etaP * etaM;
            shapeFunctionDerivatives[12].Zeta = 0.25 * xiP * xiM * etaM;
            shapeFunctionDerivatives[13].Zeta = 0.25 * xiP * etaP * etaM;
            shapeFunctionDerivatives[14].Zeta = 0.25 * xiM * xiP * etaP;
            shapeFunctionDerivatives[15].Zeta = 0.25 * xiM * etaM * etaP;
            shapeFunctionDerivatives[16].Zeta = -0.5 * xiM * etaM * zeta;
            shapeFunctionDerivatives[17].Zeta = -0.5 * xiP * etaM * zeta;
            shapeFunctionDerivatives[18].Zeta = -0.5 * xiP * etaP * zeta;
            shapeFunctionDerivatives[19].Zeta = -0.5 * xiM * etaP * zeta;

            return shapeFunctionDerivatives;
        }

        private GaussLegendrePoint3D[] CalculateGaussMatrices(double[,] nodeCoordinates)
        {
            GaussLegendrePoint1D[] integrationPointsPerAxis =
                GaussQuadrature.GetGaussLegendrePoints(iInt);
            int totalSamplingPoints = (int)Math.Pow(integrationPointsPerAxis.Length, 3);

            GaussLegendrePoint3D[] integrationPoints = new GaussLegendrePoint3D[totalSamplingPoints];

            int counter = -1;
            foreach (GaussLegendrePoint1D pointXi in integrationPointsPerAxis)
            {
                foreach (GaussLegendrePoint1D pointEta in integrationPointsPerAxis)
                {
                    foreach (GaussLegendrePoint1D pointZeta in integrationPointsPerAxis)
                    {
                        counter += 1;
                        double xi = pointXi.Coordinate;
                        double eta = pointEta.Coordinate;
                        double zeta = pointZeta.Coordinate;

                        ShapeFunctionNaturalDerivatives3D[] shapeDerivativeValues =
                            this.CalculateShapeDerivativeValues(xi, eta, zeta);
                        Jacobian3D jacobian = new Jacobian3D(nodeCoordinates, shapeDerivativeValues);
                        double[,] deformationMatrix = this.CalculateDeformationMatrix(jacobian, shapeDerivativeValues);
                        double weightFactor = pointXi.WeightFactor * pointEta.WeightFactor * pointZeta.WeightFactor *
                                              jacobian.Determinant;

                        integrationPoints[counter] = new GaussLegendrePoint3D(
                            xi, eta, zeta, deformationMatrix, weightFactor);
                    }
                }
            }

            return integrationPoints;
        }

        public virtual IMatrix2D<double> StiffnessMatrix(Element element)
        {
            double[,] coordinates = this.GetCoordinates(element);
            GaussLegendrePoint3D[] integrationPoints = this.CalculateGaussMatrices(coordinates);

            SymmetricMatrix2D<double> stiffnessMatrix = new SymmetricMatrix2D<double>(60);

            int pointId = -1;
            foreach (GaussLegendrePoint3D intPoint in integrationPoints)
            {
                pointId++;
                IMatrix2D<double> constitutiveMatrix = materialsAtGaussPoints[pointId].ConstitutiveMatrix;
                double[,] b = intPoint.DeformationMatrix;
                for (int i = 0; i < 60; i++)
                {
                    double[] eb = new double[60];
                    for (int iE = 0; iE < 6; iE++)
                    {
                        eb[iE] = (constitutiveMatrix[iE, 0] * b[0, i]) + (constitutiveMatrix[iE, 1] * b[1, i]) +
                                 (constitutiveMatrix[iE, 2] * b[2, i]) + (constitutiveMatrix[iE, 3] * b[3, i]) +
                                 (constitutiveMatrix[iE, 4] * b[4, i]) + (constitutiveMatrix[iE, 5] * b[5, i]);
                    }

                    for (int j = i; j < 60; j++)
                    {
                        double stiffness = (b[0, j] * eb[0]) + (b[1, j] * eb[1]) + (b[2, j] * eb[2]) + (b[3, j] * eb[3]) +
                                           (b[4, j] * eb[4]) + (b[5, j] * eb[5]);
                        stiffnessMatrix[i, j] += stiffness * intPoint.WeightFactor;
                    }
                }
            }

            return stiffnessMatrix;
        }
        public IMatrix2D<double> CalculateConsistentMass(Element element)
        {
            double[,] coordinates = this.GetCoordinates(element);
            GaussLegendrePoint3D[] integrationPoints = this.CalculateGaussMatrices(coordinates);

            SymmetricMatrix2D<double> consistentMass = new SymmetricMatrix2D<double>(60);

            foreach (GaussLegendrePoint3D intPoint in integrationPoints)
            {
                double[] shapeFunctionValues = this.CalcH20Shape(intPoint.Xi, intPoint.Eta, intPoint.Zeta);
                double weightDensity = intPoint.WeightFactor * this.Density;
                for (int shapeFunctionI = 0; shapeFunctionI < shapeFunctionValues.Length; shapeFunctionI++)
                {
                    for (int shapeFunctionJ = shapeFunctionI; shapeFunctionJ < shapeFunctionValues.Length; shapeFunctionJ++)
                    {
                        consistentMass[3 * shapeFunctionI, 3 * shapeFunctionJ] += shapeFunctionValues[shapeFunctionI] *
                                                                                  shapeFunctionValues[shapeFunctionJ] *
                                                                                  weightDensity;
                    }

                    for (int shapeFunctionJ = shapeFunctionI; shapeFunctionJ < shapeFunctionValues.Length; shapeFunctionJ++)
                    {
                        consistentMass[(3 * shapeFunctionI) + 1, (3 * shapeFunctionJ) + 1] =
                            consistentMass[3 * shapeFunctionI, 3 * shapeFunctionJ];

                        consistentMass[(3 * shapeFunctionI) + 2, (3 * shapeFunctionJ) + 2] =
                            consistentMass[3 * shapeFunctionI, 3 * shapeFunctionJ];
                    }
                }
            }

            return consistentMass;
        }

        public virtual IMatrix2D<double> MassMatrix(Element element)
        {
            return CalculateConsistentMass(element);
        }

        public virtual IMatrix2D<double> DampingMatrix(Element element)
        {
            var m = MassMatrix(element);
            m.LinearCombination(new double[] { RayleighAlpha, RayleighBeta }, new IMatrix2D<double>[] { MassMatrix(element), StiffnessMatrix(element) });
            return m;
        }
        public Tuple<double[], double[]> CalculateStresses(Element element, double[] localDisplacements, double[] localdDisplacements)
        {
            double[,] faXYZ = GetCoordinates(element);
            double[,] faDS = new double[iInt3, 60];
            double[,] faS = new double[iInt3, 20];
            double[,,] faB = new double[iInt3, 60, 6];
            double[] faDetJ = new double[iInt3];
            double[,,] faJ = new double[iInt3, 3, 3];
            double[] faWeight = new double[iInt3];
            double[,] fadStrains = new double[iInt3, 6];
            double[,] faStrains = new double[iInt3, 6];

            //CalculateGaussMatrices(faXYZ);
            CalcH20GaussMatrices(ref iInt, faXYZ, faWeight, faS, faDS, faJ, faDetJ, faB);
            CalcH20Strains(ref iInt, faB, localDisplacements, faStrains);
            CalcH20Strains(ref iInt, faB, localdDisplacements, fadStrains);

            double[] dStrains = new double[6];
            double[] strains = new double[6];
            for (int i = 0; i < materialsAtGaussPoints.Length; i++)
            {
                for (int j = 0; j < 6; j++) dStrains[j] = fadStrains[i, j];
                for (int j = 0; j < 6; j++) strains[j] = faStrains[i, j];
                materialsAtGaussPoints[i].UpdateMaterial(dStrains);
            }

            return new Tuple<double[], double[]>(strains, materialsAtGaussPoints[materialsAtGaussPoints.Length - 1].Stresses);
        }

        public double[] CalculateForcesForLogging(Element element, double[] localDisplacements)
        {
            return CalculateForces(element, localDisplacements, new double[localDisplacements.Length]);
        }

        public double[] CalculateForces(Element element, double[] localTotalDisplacements, double[] localdDisplacements)
        {

            double[,] faStresses = new double[iInt3, 6];
            for (int i = 0; i < materialsAtGaussPoints.Length; i++)
                for (int j = 0; j < 6; j++) faStresses[i, j] = materialsAtGaussPoints[i].Stresses[j];

            double[,] faXYZ = GetCoordinates(element);
            double[,] faDS = new double[iInt3, 60];
            double[,] faS = new double[iInt3, 20];
            double[,,] faB = new double[iInt3, 60, 6];
            double[] faDetJ = new double[iInt3];
            double[,,] faJ = new double[iInt3, 3, 3];
            double[] faWeight = new double[iInt3];
            double[] faForces = new double[60];

            //CalculateGaussMatrices(faXYZ);
            CalcH20GaussMatrices(ref iInt, faXYZ, faWeight, faS, faDS, faJ, faDetJ, faB);
            CalcH20Forces(ref iInt, faB, faWeight, faStresses, faForces);

            return faForces;
        }

        public double[] CalculateAccelerationForces(Element element, IList<MassAccelerationLoad> loads)
        {
            Vector<double> accelerations = new Vector<double>(60);
            IMatrix2D<double> massMatrix = MassMatrix(element);

            foreach (MassAccelerationLoad load in loads)
            {
                int index = 0;
                foreach (DOFType[] nodalDOFTypes in dofTypes)
                    foreach (DOFType dofType in nodalDOFTypes)
                    {
                        if (dofType == load.DOF) accelerations[index] += load.Amount;
                        index++;
                    }
            }
            double[] forces = new double[60];
            massMatrix.Multiply(accelerations, forces);
            return forces;
        }

        public void ClearMaterialState()
        {
            foreach (IFiniteElementMaterial3D m in materialsAtGaussPoints) m.ClearState();
        }

        public void SaveMaterialState()
        {
            foreach (IFiniteElementMaterial3D m in materialsAtGaussPoints) m.SaveState();
        }

        public void ClearMaterialStresses()
        {
            foreach (IFiniteElementMaterial3D m in materialsAtGaussPoints) m.ClearStresses();
        }
        #endregion
        #region IStructuralFiniteElement Members

        public bool MaterialModified
        {
            get
            {
                foreach (IFiniteElementMaterial3D material in materialsAtGaussPoints)
                    if (material.Modified) return true;
                return false;
            }
        }

        public void ResetMaterialModified()
        {
            foreach (IFiniteElementMaterial3D material in materialsAtGaussPoints) material.ResetModified();
        }

        #endregion

        #region IEmbeddedHostElement Members

        public EmbeddedNode BuildHostElementEmbeddedNode(Element element, Node node, IEmbeddedDOFInHostTransformationVector transformationVector)
        {
            var points = GetNaturalCoordinates(element, node);
            if (points.Length == 0) return null;

            element.EmbeddedNodes.Add(node);
            var embeddedNode = new EmbeddedNode(node, element, transformationVector.GetDependentDOFTypes);
            for (int i = 0; i < points.Length; i++)
                embeddedNode.Coordinates.Add(points[i]);
            return embeddedNode;
        }
        public double[] GetShapeFunctionsForNode(Element element, EmbeddedNode node)
        {
            double[,] elementCoordinates = GetCoordinatesTranspose(element);
            var shapeFunctions = CalcH20Shape(node.Coordinates[0], node.Coordinates[1], node.Coordinates[2]);
            var nablaShapeFunctions = CalcH20NablaShape(node.Coordinates[0], node.Coordinates[1], node.Coordinates[2]);
            var jacobian = CalcH20JDetJ(elementCoordinates, nablaShapeFunctions);

            return new double[]
            {
                shapeFunctions[0], shapeFunctions[1], shapeFunctions[2], shapeFunctions[3], shapeFunctions[4], shapeFunctions[5], shapeFunctions[6], shapeFunctions[7], shapeFunctions[8], shapeFunctions[9],
                shapeFunctions[10], shapeFunctions[11], shapeFunctions[12], shapeFunctions[13], shapeFunctions[14], shapeFunctions[15],
                nablaShapeFunctions[0], nablaShapeFunctions[1], nablaShapeFunctions[2], nablaShapeFunctions[3], nablaShapeFunctions[4], nablaShapeFunctions[5], nablaShapeFunctions[6], nablaShapeFunctions[7],
                nablaShapeFunctions[8], nablaShapeFunctions[9], nablaShapeFunctions[10], nablaShapeFunctions[11], nablaShapeFunctions[12], nablaShapeFunctions[13], nablaShapeFunctions[14], nablaShapeFunctions[15],
                nablaShapeFunctions[16], nablaShapeFunctions[17], nablaShapeFunctions[18], nablaShapeFunctions[19], nablaShapeFunctions[20], nablaShapeFunctions[21], nablaShapeFunctions[22], nablaShapeFunctions[23],
                nablaShapeFunctions[32], nablaShapeFunctions[33], nablaShapeFunctions[34], nablaShapeFunctions[35], nablaShapeFunctions[36], nablaShapeFunctions[37], nablaShapeFunctions[38], nablaShapeFunctions[39],
                nablaShapeFunctions[40], nablaShapeFunctions[41], nablaShapeFunctions[42], nablaShapeFunctions[43], nablaShapeFunctions[44], nablaShapeFunctions[45], nablaShapeFunctions[46], nablaShapeFunctions[47],
                nablaShapeFunctions[48], nablaShapeFunctions[49], nablaShapeFunctions[50], nablaShapeFunctions[51], nablaShapeFunctions[52], nablaShapeFunctions[53], nablaShapeFunctions[54], nablaShapeFunctions[55],
                nablaShapeFunctions[56], nablaShapeFunctions[57], nablaShapeFunctions[58], nablaShapeFunctions[59],
                jacobian.Item1[0, 0], jacobian.Item1[0, 1], jacobian.Item1[0, 2], jacobian.Item1[1, 0], jacobian.Item1[1, 1], jacobian.Item1[1, 2], jacobian.Item1[2, 0], jacobian.Item1[2, 1], jacobian.Item1[2, 2],
                jacobian.Item2[0, 0], jacobian.Item2[0, 1], jacobian.Item2[0, 2], jacobian.Item2[1, 0], jacobian.Item2[1, 1], jacobian.Item2[1, 2], jacobian.Item2[2, 0], jacobian.Item2[2, 1], jacobian.Item2[2, 2]
            };
        }

        private double[] GetNaturalCoordinates(Element element, Node node)
        {
            double[] mins = new double[] { element.Nodes[0].X, element.Nodes[0].Y, element.Nodes[0].Z };
            double[] maxes = new double[] { element.Nodes[0].X, element.Nodes[0].Y, element.Nodes[0].Z };
            for (int i = 0; i < element.Nodes.Count; i++)
            {
                mins[0] = mins[0] > element.Nodes[i].X ? element.Nodes[i].X : mins[0];
                mins[1] = mins[1] > element.Nodes[i].Y ? element.Nodes[i].Y : mins[1];
                mins[2] = mins[2] > element.Nodes[i].Z ? element.Nodes[i].Z : mins[2];
                maxes[0] = maxes[0] < element.Nodes[i].X ? element.Nodes[i].X : maxes[0];
                maxes[1] = maxes[1] < element.Nodes[i].Y ? element.Nodes[i].Y : maxes[1];
                maxes[2] = maxes[2] < element.Nodes[i].Z ? element.Nodes[i].Z : maxes[2];
            }


            bool maybeInsideElement = node.X <= maxes[0] && node.X >= mins[0] &&
                node.Y <= maxes[1] && node.Y >= mins[1] &&
                node.Z <= maxes[2] && node.Z >= mins[2];
            if (maybeInsideElement == false) return new double[0];

            const int jacobianSize = 3;
            const int maxIterations = 1000;
            const double tolerance = 1e-10;
            int iterations = 0;
            double deltaNaturalCoordinatesNormSquare = 100;
            double[] naturalCoordinates = new double[] { 0, 0, 0 };
            const double toleranceSquare = tolerance * tolerance;

            while (deltaNaturalCoordinatesNormSquare > toleranceSquare && iterations < maxIterations)
            {
                iterations++;
                var shapeFunctions = CalcH20Shape(naturalCoordinates[0], naturalCoordinates[1], naturalCoordinates[2]);
                double[] coordinateDifferences = new double[] { 0, 0, 0 };
                for (int i = 0; i < shapeFunctions.Length; i++)
                {
                    coordinateDifferences[0] += shapeFunctions[i] * element.Nodes[i].X;
                    coordinateDifferences[1] += shapeFunctions[i] * element.Nodes[i].Y;
                    coordinateDifferences[2] += shapeFunctions[i] * element.Nodes[i].Z;
                }
                coordinateDifferences[0] = node.X - coordinateDifferences[0];
                coordinateDifferences[1] = node.Y - coordinateDifferences[1];
                coordinateDifferences[2] = node.Z - coordinateDifferences[2];

                double[,] faXYZ = GetCoordinatesTranspose(element);
                double[] nablaShapeFunctions = CalcH20NablaShape(naturalCoordinates[0], naturalCoordinates[1], naturalCoordinates[2]);
                var inverseJacobian = CalcH20JDetJ(faXYZ, nablaShapeFunctions).Item2;

                double[] deltaNaturalCoordinates = new double[] { 0, 0, 0 };
                for (int i = 0; i < jacobianSize; i++)
                    for (int j = 0; j < jacobianSize; j++)
                        deltaNaturalCoordinates[i] += inverseJacobian[j, i] * coordinateDifferences[j];
                for (int i = 0; i < 3; i++)
                    naturalCoordinates[i] += deltaNaturalCoordinates[i];

                deltaNaturalCoordinatesNormSquare = 0;
                for (int i = 0; i < 3; i++)
                    deltaNaturalCoordinatesNormSquare += deltaNaturalCoordinates[i] * deltaNaturalCoordinates[i];
                //deltaNaturalCoordinatesNormSquare = Math.Sqrt(deltaNaturalCoordinatesNormSquare);
            }

            return naturalCoordinates.Count(x => Math.Abs(x) - 1.0 > tolerance) > 0 ? new double[0] : naturalCoordinates;
        }

        #endregion

    }
}
