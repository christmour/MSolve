using ISAAR.MSolve.PreProcessor;
using ISAAR.MSolve.PreProcessor.Elements;
using ISAAR.MSolve.PreProcessor.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.SamplesConsole
{
    public static class Hexa20SimpleCantileverBeam
    {
        public static void MakeCantileverBeam(Model model, double startX, double startY, double startZ, int startNodeID, int startElementID, int subdomainID)

        {

            int nodeID = startNodeID;

            for (int j = 0; j < 4; j++)
            {
                if (nodeID % 2 == 0)
                {
                    model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX, Y = startY + 0.25, Z = startZ + 0.25 * (j / 2) });
                }
                else
                {
                    model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX, Y = startY, Z = startZ + 0.25 * (j / 2) });
                }
                model.NodesDictionary[nodeID].Constraints.Add(DOFType.X);
                model.NodesDictionary[nodeID].Constraints.Add(DOFType.Y);
                model.NodesDictionary[nodeID].Constraints.Add(DOFType.Z);

                nodeID++;
            }

                          
                    model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX, Y = startY + 0.125, Z = startZ  });

                     model.NodesDictionary[nodeID].Constraints.Add(DOFType.X);
                     model.NodesDictionary[nodeID].Constraints.Add(DOFType.Y);
                     model.NodesDictionary[nodeID].Constraints.Add(DOFType.Z);

                    model.NodesDictionary.Add(nodeID + 1, new Node() { ID = nodeID + 1, X = startX, Y = startY , Z = startZ + 0.125  });

                    model.NodesDictionary[nodeID + 1].Constraints.Add(DOFType.X);
                    model.NodesDictionary[nodeID + 1].Constraints.Add(DOFType.Y);
                    model.NodesDictionary[nodeID + 1].Constraints.Add(DOFType.Z);

                    model.NodesDictionary.Add(nodeID + 2, new Node() { ID = nodeID + 2, X = startX, Y = startY + 0.25, Z = startZ + 0.125 });

                    model.NodesDictionary[nodeID + 2].Constraints.Add(DOFType.X);
                    model.NodesDictionary[nodeID + 2].Constraints.Add(DOFType.Y);
                    model.NodesDictionary[nodeID + 2].Constraints.Add(DOFType.Z);

                    model.NodesDictionary.Add(nodeID + 3, new Node() { ID = nodeID + 3, X = startX, Y = startY + 0.125, Z = startZ + 0.25 });

                    model.NodesDictionary[nodeID + 3].Constraints.Add(DOFType.X);
                    model.NodesDictionary[nodeID + 3].Constraints.Add(DOFType.Y);
                    model.NodesDictionary[nodeID + 3].Constraints.Add(DOFType.Z);

                nodeID = nodeID+4;


            for (int j = 0; j < 4; j++)
            {
                    if (nodeID % 2 == 0)
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.125 , Y = startY + 0.25, Z = startZ + 0.25 * (j / 2) });
                    }
                    else
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.125 , Y = startY, Z = startZ + 0.25 * (j / 2) });
                    }
                    nodeID++;
             }


            for (int j = 0; j < 4; j++)
            {
                    if (nodeID % 2 == 0)
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.25 , Y = startY + 0.25, Z = startZ + 0.25 * (j / 2) });
                    }
                    else
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.25 , Y = startY, Z = startZ + 0.25 * (j / 2) });
                    }
                    nodeID++;
            }
            

           
                model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.25, Y = startY + 0.125, Z = startZ });
                model.NodesDictionary.Add(nodeID + 1, new Node() { ID = nodeID + 1, X = startX + 0.25, Y = startY, Z = startZ + 0.125 });
                model.NodesDictionary.Add(nodeID + 2, new Node() { ID = nodeID + 2, X = startX + 0.25, Y = startY + 0.25, Z = startZ + 0.125 });
                model.NodesDictionary.Add(nodeID +3, new Node() { ID = nodeID + 3, X = startX + 0.25, Y = startY + 0.125, Z = startZ + 0.25 });

                nodeID = nodeID + 4;

            for (int j = 0; j < 4; j++)
            {
                    if (nodeID % 2 == 0)
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.375 , Y = startY + 0.25, Z = startZ + 0.25 * (j / 2) });
                    }
                    else
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.375 , Y = startY, Z = startZ + 0.25 * (j / 2) });
                    }
                    nodeID++;
            }


            for (int j = 0; j < 4; j++)
            {
                    if (nodeID % 2 == 0)
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.5 , Y = startY + 0.25, Z = startZ + 0.25 * (j / 2) });
                    }
                    else
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.5 , Y = startY, Z = startZ + 0.25 * (j / 2) });
                    }
                    nodeID++;
           }
           

           
                model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.5, Y = startY + 0.125, Z = startZ });
                model.NodesDictionary.Add(nodeID + 1, new Node() { ID = nodeID + 1, X = startX + 0.5, Y = startY, Z = startZ + 0.125 });
                model.NodesDictionary.Add(nodeID + 2, new Node() { ID = nodeID + 2, X = startX + 0.5, Y = startY + 0.25, Z = startZ + 0.125 });
                model.NodesDictionary.Add(nodeID + 3, new Node() { ID = nodeID + 3, X = startX + 0.5, Y = startY + 0.125, Z = startZ + 0.25 });

                nodeID = nodeID + 4;

            for (int j = 0; j < 4; j++)
            {
                if (nodeID % 2 == 0)
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.625 , Y = startY + 0.25, Z = startZ + 0.25 * (j / 2) });
                    }
                    else
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.625 , Y = startY, Z = startZ + 0.25 * (j / 2) });
                    }
                    nodeID++;
            }


            for (int j = 0; j < 4; j++)
            {
                    if (nodeID % 2 == 0)
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.75 , Y = startY + 0.25, Z = startZ + 0.25 * (j / 2) });
                    }
                    else
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.75 , Y = startY, Z = startZ + 0.25 * (j / 2) });
                    }
                    nodeID++;
            }
           

            
                model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.75, Y = startY + 0.125, Z = startZ });
                model.NodesDictionary.Add(nodeID + 1, new Node() { ID = nodeID + 1, X = startX + 0.75, Y = startY, Z = startZ + 0.125 });
                model.NodesDictionary.Add(nodeID + 2, new Node() { ID = nodeID + 2, X = startX + 0.75, Y = startY + 0.25, Z = startZ + 0.125 });
                model.NodesDictionary.Add(nodeID + 3, new Node() { ID = nodeID + 3, X = startX + 0.75, Y = startY + 0.125, Z = startZ + 0.25 });

                nodeID = nodeID + 4;

            for (int j = 0; j < 4; j++)
            {
                    if (nodeID % 2 == 0)
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.875 , Y = startY + 0.25, Z = startZ + 0.25 * (j / 2) });
                    }
                    else
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 0.875 , Y = startY, Z = startZ + 0.25 * (j / 2) });
                    }
                    nodeID++;
            }


            for (int j = 0; j < 4; j++)
            {
                    if (nodeID % 2 == 0)
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 1.0 , Y = startY + 0.25, Z = startZ + 0.25 * (j / 2) });
                    }
                    else
                    {
                        model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 1.0 , Y = startY, Z = startZ + 0.25 * (j / 2) });
                    }
                   nodeID++;                
            }

            
                model.NodesDictionary.Add(nodeID, new Node() { ID = nodeID, X = startX + 1.0, Y = startY + 0.125, Z = startZ });
                model.NodesDictionary.Add(nodeID + 1, new Node() { ID = nodeID + 1, X = startX + 1.0, Y = startY, Z = startZ + 0.125 });
                model.NodesDictionary.Add(nodeID + 2, new Node() { ID = nodeID + 2, X = startX + 1.0, Y = startY + 0.25, Z = startZ + 0.125 });
                model.NodesDictionary.Add(nodeID + 3, new Node() { ID = nodeID + 3, X = startX + 1.0, Y = startY + 0.125, Z = startZ + 0.25 });

                nodeID = nodeID + 4;

            int elementID = startElementID;
            Element e;
            ElasticMaterial3D material = new ElasticMaterial3D()
            {
                YoungModulus = 2.0e7,
                PoissonRatio = 0.3
            };

            for (int i = 0; i < 4; i++)
            {

                e = new Element()
                {
                    ID = elementID,
                    ElementType = new Hexa20(material)

                };

                e.NodesDictionary.Add(startNodeID + 12 * i, model.NodesDictionary[startNodeID + 12 * i]);
                e.NodesDictionary.Add(startNodeID + 12* i + 12, model.NodesDictionary[startNodeID + 12 * i + 12]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 13, model.NodesDictionary[startNodeID + 12 * i + 13]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 1, model.NodesDictionary[startNodeID + 12 * i + 1]);

                e.NodesDictionary.Add(startNodeID + 12 * i + 2, model.NodesDictionary[startNodeID + 12 * i + 2]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 14, model.NodesDictionary[startNodeID + 12 * i + 14]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 15, model.NodesDictionary[startNodeID + 12 * i + 15]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 3, model.NodesDictionary[startNodeID + 12 * i + 3]);

                e.NodesDictionary.Add(startNodeID + 12 * i + 8, model.NodesDictionary[startNodeID + 12 * i + 8]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 16, model.NodesDictionary[startNodeID + 12 * i + 16]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 9, model.NodesDictionary[startNodeID + 12 * i + 9]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 4, model.NodesDictionary[startNodeID + 12 * i + 4]);

                //e.NodesDictionary.Add(startNodeID + 12 * i + 5, model.NodesDictionary[startNodeID + 12 * i + 5]);
                //e.NodesDictionary.Add(startNodeID + 12 * i + 17, model.NodesDictionary[startNodeID + 12 * i + 17]);
                //e.NodesDictionary.Add(startNodeID + 12 * i + 18, model.NodesDictionary[startNodeID + 12 * i + 18]);
                //e.NodesDictionary.Add(startNodeID + 12 * i + 6, model.NodesDictionary[startNodeID + 12 * i + 6]);

                e.NodesDictionary.Add(startNodeID + 12 * i + 10, model.NodesDictionary[startNodeID + 12 * i + 10]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 19, model.NodesDictionary[startNodeID + 12 * i + 19]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 11, model.NodesDictionary[startNodeID + 12 * i + 11]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 7, model.NodesDictionary[startNodeID + 12 * i + 7]);

                e.NodesDictionary.Add(startNodeID + 12 * i + 5, model.NodesDictionary[startNodeID + 12 * i + 5]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 17, model.NodesDictionary[startNodeID + 12 * i + 17]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 18, model.NodesDictionary[startNodeID + 12 * i + 18]);
                e.NodesDictionary.Add(startNodeID + 12 * i + 6, model.NodesDictionary[startNodeID + 12 * i + 6]);

                model.ElementsDictionary.Add(e.ID, e);
                model.SubdomainsDictionary[subdomainID].ElementsDictionary.Add(e.ID, e);


                elementID++;
            }
        }


    }

}
