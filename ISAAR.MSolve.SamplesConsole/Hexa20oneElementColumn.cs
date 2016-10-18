using ISAAR.MSolve.PreProcessor;
using ISAAR.MSolve.PreProcessor.Elements;
using ISAAR.MSolve.PreProcessor.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.SamplesConsole
{
    public static class Hexa20oneElementColumn
    {
        public static void MakeCantileverBeam(Model model, double startX, double startY, double startZ, int startNodeID, int startElementID, int subdomainID)

        {

            int nodeID = startNodeID;


            model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX, Y = startY, Z = startZ });
            model.NodesDictionary.Add(nodeID + 1, new Node() { ID = nodeID + 1, X = startX, Y = startY + 0.25, Z = startZ });
            model.NodesDictionary.Add(nodeID + 2, new Node() { ID = nodeID + 2, X = startX, Y = startY + 0.125, Z = startZ });
            model.NodesDictionary.Add(nodeID + 3, new Node() { ID = nodeID + 3, X = startX, Y = startY, Z = startZ + 0.125 });
            model.NodesDictionary.Add(nodeID + 4, new Node() { ID = nodeID + 4, X = startX, Y = startY + 0.25, Z = startZ + 0.125 });

            model.NodesDictionary.Add(nodeID + 5, new Node() { ID = nodeID + 5, X = startX, Y = startY, Z = startZ + 0.25 });
            model.NodesDictionary.Add(nodeID + 6, new Node() { ID = nodeID + 6, X = startX, Y = startY + 0.25, Z = startZ + 0.25 });
            model.NodesDictionary.Add(nodeID + 7, new Node() { ID = nodeID + 7, X = startX, Y = startY + 0.125, Z = startZ + 0.25 });
            model.NodesDictionary.Add(nodeID + 8, new Node() { ID = nodeID + 8, X = startX + 0.125, Y = startY, Z = startZ });
            model.NodesDictionary.Add(nodeID + 9, new Node() { ID = nodeID + 9, X = startX + 0.125, Y = startY + 0.25, Z = startZ });

            model.NodesDictionary.Add(nodeID + 10, new Node() { ID = nodeID + 10, X = startX + 0.125, Y = startY, Z = startZ + 0.25 });
            model.NodesDictionary.Add(nodeID + 11, new Node() { ID = nodeID + 11, X = startX + 0.125, Y = startY + 0.25, Z = startZ + 0.25 });
            model.NodesDictionary.Add(nodeID + 12, new Node() { ID = nodeID + 12, X = startX + 0.25, Y = startY, Z = startZ });
            model.NodesDictionary.Add(nodeID + 13, new Node() { ID = nodeID + 13, X = startX + 0.25, Y = startY + 0.25, Z = startZ });
            model.NodesDictionary.Add(nodeID + 14, new Node() { ID = nodeID + 14, X = startX + 0.25, Y = startY + 0.125, Z = startZ });

            model.NodesDictionary.Add(nodeID + 15, new Node() { ID = nodeID + 15, X = startX + 0.25, Y = startY, Z = startZ + 0.125 });
            model.NodesDictionary.Add(nodeID + 16, new Node() { ID = nodeID + 16, X = startX + 0.25, Y = startY + 0.25, Z = startZ + 0.125 });
            model.NodesDictionary.Add(nodeID + 17, new Node() { ID = nodeID + 17, X = startX + 0.25, Y = startY, Z = startZ + 0.25 });
            model.NodesDictionary.Add(nodeID + 18, new Node() { ID = nodeID + 18, X = startX + 0.25, Y = startY + 0.25, Z = startZ + 0.25 });
            model.NodesDictionary.Add(nodeID + 19, new Node() { ID = nodeID + 19, X = startX + 0.25, Y = startY + 0.125, Z = startZ + 0.25 });

            model.NodesDictionary[nodeID].Constraints.Add(DOFType.X);
            model.NodesDictionary[nodeID].Constraints.Add(DOFType.Y);
            model.NodesDictionary[nodeID].Constraints.Add(DOFType.Z);

            model.NodesDictionary[nodeID + 1].Constraints.Add(DOFType.X);
            model.NodesDictionary[nodeID + 1].Constraints.Add(DOFType.Y);
            model.NodesDictionary[nodeID + 1].Constraints.Add(DOFType.Z);

            model.NodesDictionary[nodeID + 12].Constraints.Add(DOFType.X);
            model.NodesDictionary[nodeID + 12].Constraints.Add(DOFType.Y);
            model.NodesDictionary[nodeID + 12].Constraints.Add(DOFType.Z);

            model.NodesDictionary[nodeID + 13].Constraints.Add(DOFType.X);
            model.NodesDictionary[nodeID + 13].Constraints.Add(DOFType.Y);
            model.NodesDictionary[nodeID + 13].Constraints.Add(DOFType.Z);

            model.NodesDictionary[nodeID + 2].Constraints.Add(DOFType.X);
            model.NodesDictionary[nodeID + 2].Constraints.Add(DOFType.Y);
            model.NodesDictionary[nodeID + 2].Constraints.Add(DOFType.Z);

            model.NodesDictionary[nodeID + 8].Constraints.Add(DOFType.X);
            model.NodesDictionary[nodeID + 8].Constraints.Add(DOFType.Y);
            model.NodesDictionary[nodeID + 8].Constraints.Add(DOFType.Z);

            model.NodesDictionary[nodeID + 14].Constraints.Add(DOFType.X);
            model.NodesDictionary[nodeID + 14].Constraints.Add(DOFType.Y);
            model.NodesDictionary[nodeID + 14].Constraints.Add(DOFType.Z);

            model.NodesDictionary[nodeID + 9].Constraints.Add(DOFType.X);
            model.NodesDictionary[nodeID + 9].Constraints.Add(DOFType.Y);
            model.NodesDictionary[nodeID + 9].Constraints.Add(DOFType.Z);

            int elementID = startElementID;
            Element e;
            ElasticMaterial3D material = new ElasticMaterial3D()
            {
                YoungModulus = 2.0e7,
                PoissonRatio = 0.3
            };

            //for (int i = 0; i < 4; i++)
            //{

            e = new Element()
            {
                ID = elementID,
                ElementType = new Hexa20(material)

            };

            e.NodesDictionary.Add(startNodeID, model.NodesDictionary[startNodeID]);
            e.NodesDictionary.Add(startNodeID + 12, model.NodesDictionary[startNodeID + 12]);
            e.NodesDictionary.Add(startNodeID + 13, model.NodesDictionary[startNodeID + 13]);
            e.NodesDictionary.Add(startNodeID + 1, model.NodesDictionary[startNodeID + 1]);

            e.NodesDictionary.Add(startNodeID + 5, model.NodesDictionary[startNodeID + 5]);
            e.NodesDictionary.Add(startNodeID + 17, model.NodesDictionary[startNodeID + 17]);
            e.NodesDictionary.Add(startNodeID + 18, model.NodesDictionary[startNodeID + 18]);
            e.NodesDictionary.Add(startNodeID + 6, model.NodesDictionary[startNodeID + 6]);

            e.NodesDictionary.Add(startNodeID + 8, model.NodesDictionary[startNodeID + 8]);
            e.NodesDictionary.Add(startNodeID + 14, model.NodesDictionary[startNodeID + 14]);
            e.NodesDictionary.Add(startNodeID + 9, model.NodesDictionary[startNodeID + 9]);
            e.NodesDictionary.Add(startNodeID + 2, model.NodesDictionary[startNodeID + 2]);

            //e.NodesDictionary.Add(startNodeID + 12 * i + 5, model.NodesDictionary[startNodeID + 12 * i + 5]);
            //e.NodesDictionary.Add(startNodeID + 12 * i + 17, model.NodesDictionary[startNodeID + 12 * i + 17]);
            //e.NodesDictionary.Add(startNodeID + 12 * i + 18, model.NodesDictionary[startNodeID + 12 * i + 18]);
            //e.NodesDictionary.Add(startNodeID + 12 * i + 6, model.NodesDictionary[startNodeID + 12 * i + 6]);

            e.NodesDictionary.Add(startNodeID + 10, model.NodesDictionary[startNodeID + 10]);
            e.NodesDictionary.Add(startNodeID + 19, model.NodesDictionary[startNodeID + 19]);
            e.NodesDictionary.Add(startNodeID + 11, model.NodesDictionary[startNodeID + 11]);
            e.NodesDictionary.Add(startNodeID + 7, model.NodesDictionary[startNodeID + 7]);

            e.NodesDictionary.Add(startNodeID + 3, model.NodesDictionary[startNodeID + 3]);
            e.NodesDictionary.Add(startNodeID + 15, model.NodesDictionary[startNodeID + 15]);
            e.NodesDictionary.Add(startNodeID + 16, model.NodesDictionary[startNodeID + 16]);
            e.NodesDictionary.Add(startNodeID + 4, model.NodesDictionary[startNodeID + 4]);

            model.ElementsDictionary.Add(e.ID, e);
            model.SubdomainsDictionary[subdomainID].ElementsDictionary.Add(e.ID, e);


            //   elementID++;
            //}
        }


    }

}
