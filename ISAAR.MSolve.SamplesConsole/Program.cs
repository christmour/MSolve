using ISAAR.MSolve.Analyzers;
using ISAAR.MSolve.Logging;
using ISAAR.MSolve.Matrices;
using ISAAR.MSolve.PreProcessor;
using ISAAR.MSolve.Problems;
using ISAAR.MSolve.Solvers.Skyline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.SamplesConsole
{
    class Program
    {
        private static void SolveBuildingInNoSoilSmall()
        {
            VectorExtensions.AssignTotalAffinityCount();
            Model model = new Model();
            model.SubdomainsDictionary.Add(1, new Subdomain() { ID = 1 });
            BeamBuildingBuilder.MakeBeamBuilding(model, 20, 20, 20, 5, 4, model.NodesDictionary.Count + 1,
                model.ElementsDictionary.Count + 1, 1, 4, false, false);
            model.Loads.Add(new Load() { Amount = -100, Node = model.Nodes[21], DOF = DOFType.X });
            model.ConnectDataStructures();

            SolverSkyline solver = new SolverSkyline(model);
            ProblemStructural provider = new ProblemStructural(model, solver.SubdomainsDictionary);
            LinearAnalyzer analyzer = new LinearAnalyzer(solver, solver.SubdomainsDictionary);
            StaticAnalyzer parentAnalyzer = new StaticAnalyzer(provider, analyzer, solver.SubdomainsDictionary);

            analyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] { 420 });

            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();
        }

        private static void SolveBuildingInNoSoilSmallDynamic()
        {
            VectorExtensions.AssignTotalAffinityCount();
            Model model = new Model();
            model.SubdomainsDictionary.Add(1, new Subdomain() { ID = 1 });
            BeamBuildingBuilder.MakeBeamBuilding(model, 20, 20, 20, 5, 4, model.NodesDictionary.Count + 1,
                model.ElementsDictionary.Count + 1, 1, 4, false, false);
            model.ConnectDataStructures();

            SolverSkyline solver = new SolverSkyline(model);
            ProblemStructural provider = new ProblemStructural(model, solver.SubdomainsDictionary);
            LinearAnalyzer analyzer = new LinearAnalyzer(solver, solver.SubdomainsDictionary);
            NewmarkDynamicAnalyzer parentAnalyzer = new NewmarkDynamicAnalyzer(provider, analyzer, solver.SubdomainsDictionary, 0.5, 0.25, 0.01, 0.1);

            analyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] { 420 });

            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();
        }
        private static void SolveHexa20CantileverBeam()
        {
            VectorExtensions.AssignTotalAffinityCount();
            Model model = new Model();
            model.SubdomainsDictionary.Add(1, new Subdomain() { ID = 1 });

            Hexa20SimpleCantileverBeam.MakeCantileverBeam(model, 0, 0, 0, model.NodesDictionary.Count + 1, model.ElementsDictionary.Count + 1, 1);

            model.Loads.Add(new nodalLoad() { Amount = -0.125, Node = model.Nodes[48], DOF = DOFType.Z });
            model.Loads.Add(new nodalLoad() { Amount = -0.125, Node = model.Nodes[49], DOF = DOFType.Z });
            model.Loads.Add(new nodalLoad() { Amount = -0.125, Node = model.Nodes[50], DOF = DOFType.Z });
            model.Loads.Add(new nodalLoad() { Amount = -0.125, Node = model.Nodes[51], DOF = DOFType.Z });

            model.Loads.Add(new nodalLoad() { Amount = -0.125, Node = model.Nodes[52], DOF = DOFType.Z });
            model.Loads.Add(new nodalLoad() { Amount = -0.125, Node = model.Nodes[53], DOF = DOFType.Z });
            model.Loads.Add(new nodalLoad() { Amount = -0.125, Node = model.Nodes[54], DOF = DOFType.Z });
            model.Loads.Add(new nodalLoad() { Amount = -0.125, Node = model.Nodes[55], DOF = DOFType.Z });


            model.ConnectDataStructures();

            SolverSkyline solver = new SolverSkyline(model);
            ProblemStructural provider = new ProblemStructural(model, solver.SubdomainsDictionary);
            LinearAnalyzer analyzer = new LinearAnalyzer(solver, solver.SubdomainsDictionary);
            StaticAnalyzer parentAnalyzer = new StaticAnalyzer(provider, analyzer, solver.SubdomainsDictionary);

            analyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] { 50 });

            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();
        }

        private static void SolveHexa20oneElementColumn()
        {
            VectorExtensions.AssignTotalAffinityCount();
            Model model = new Model();
            model.SubdomainsDictionary.Add(1, new Subdomain() { ID = 1 });

            Hexa20oneElementColumn.MakeCantileverBeam(model, 0, 0, 0, model.NodesDictionary.Count + 1, model.ElementsDictionary.Count + 1, 1);

            model.Loads.Add(new nodalLoad() { Amount = 0.125, Node = model.Nodes[5], DOF = DOFType.X });
            model.Loads.Add(new nodalLoad() { Amount = 0.125, Node = model.Nodes[6], DOF = DOFType.X });
            model.Loads.Add(new nodalLoad() { Amount = 0.125, Node = model.Nodes[17], DOF = DOFType.X });
            model.Loads.Add(new nodalLoad() { Amount = 0.125, Node = model.Nodes[18], DOF = DOFType.X });

            model.Loads.Add(new nodalLoad() { Amount = 0.125, Node = model.Nodes[7], DOF = DOFType.X });
            model.Loads.Add(new nodalLoad() { Amount = 0.125, Node = model.Nodes[10], DOF = DOFType.X });
            model.Loads.Add(new nodalLoad() { Amount = 0.125, Node = model.Nodes[19], DOF = DOFType.X });
            model.Loads.Add(new nodalLoad() { Amount = 0.125, Node = model.Nodes[11], DOF = DOFType.X });


            model.ConnectDataStructures();

            SolverSkyline solver = new SolverSkyline(model);
            ProblemStructural provider = new ProblemStructural(model, solver.SubdomainsDictionary);
            LinearAnalyzer analyzer = new LinearAnalyzer(solver, solver.SubdomainsDictionary);
            StaticAnalyzer parentAnalyzer = new StaticAnalyzer(provider, analyzer, solver.SubdomainsDictionary);

            analyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] { 19 });

            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();
        }

        static void Main(string[] args)
        {
            //SolveBuildingInNoSoilSmall();
            //SolveHexaCantileverBeam();  
            //SolveHexa20CantileverBeam();
            //SolveHexa20oneElementColumn();

        }
    }
}
