// https://www.codingame.com/ide/puzzle/death-first-search-episode-2

class Player // 100% codingame
{
    class Dijkstra
    {
        #region propertys
        /// <summary>
        /// Remote, distance of exits nodes (i) to enter
        /// </summary>
        public int[] Remote { get; private set; }

        /// <summary>
        /// Get uostream node of (i)
        /// </summary>
        public int[] UpstreamNodes { get; private set; }

        /// <summary>
        /// Number of likes of node (i) 
        /// </summary>
        public int[] NodesWeights { get; private set; }

        /// <summary>
        /// The lists nodes (j) represened the way to exit (i) to enter  
        /// </summary>
        public List<List<int>> OutputPaths { get; private set; } = new List<List<int>>();

        /// <summary>
        /// Neighbors: node between two or more exits
        /// The lists of nodes (j) represened the way to neighbors (i) to enter 
        /// The enter node are not in this list
        /// </summary>
        public List<List<int>> NeighborsPatchs { get; private set; } = new List<List<int>>();

        /// <summary>
        /// A Dijkstra graph
        /// https://fr.wikipedia.org/wiki/Algorithme_de_Dijkstra
        /// </summary>
        public int[,] GraphDijkstra { get; private set; }
        #endregion

        #region constructor
        /// <summary>
        /// Graphic design by Dijkstra, Remote, UpstreamNodes and NodesWeights
        /// outputs propertys:
        /// int[] Remote, int[] UpstreamNodes, int[] NodesWeights, List<List<int>> OutputPaths and List<List<int>> OutputPaths
        /// </summary>
        /// <param name="nodesGraph"></param>
        /// <param name="inputNode"></param>
        /// <param name="outputNodes"></param>
        public Dijkstra(int[,] nodesGraph, int inputNode, int[] outputNodes)
        {
            #region Graphic design of GraphDijkstra, Remote and UpstreamNodes
            bool[] IsInGraphe = new bool[nodesGraph.GetLength(0)];
            GraphDijkstra = new int[nodesGraph.GetLength(0), nodesGraph.GetLength(0)];
            NodesWeights = new int[nodesGraph.GetLength(0)];
            // ++ ici
            Remote = new int[nodesGraph.GetLength(0)];
            UpstreamNodes = new int[nodesGraph.GetLength(0)];
            //
            for (int i = 0; i < nodesGraph.GetLength(0); i++)
            {
                int nbLinks = 0;
                Remote[i] = int.MaxValue; // default value
                UpstreamNodes[i] = int.MinValue; // default value
                for (int j = 0; j < nodesGraph.GetLength(0); j++)
                {
                    GraphDijkstra[i, j] = int.MaxValue;
                    if (nodesGraph[i, j] == 1)
                    {
                        nbLinks++;
                    }
                }
                NodesWeights[i] = nbLinks;

            }
            Remote[inputNode] = 0;
            GraphDijkstra[inputNode, inputNode] = 0;
            int count = 0;

            do
            {
                int IndexMiniDistance = MinimunDistance(Remote, IsInGraphe);
                IsInGraphe[IndexMiniDistance] = true;
                GraphDijkstra[IndexMiniDistance, IndexMiniDistance] = Remote[IndexMiniDistance];

                for (int i = 0; i < nodesGraph.GetLength(0); i++)
                {

                    if (!IsInGraphe[i]
                        && nodesGraph[IndexMiniDistance, i] != 0
                        && Remote[IndexMiniDistance] != int.MaxValue
                        && Remote[IndexMiniDistance] + nodesGraph[IndexMiniDistance, i] < Remote[i]
                        && GraphDijkstra[IndexMiniDistance, i] == int.MaxValue)
                    {
                        // The distance of a node is the minimum distance of its nearest neighbor from the input added with its distance to that neighbor
                        Remote[i] = Remote[IndexMiniDistance] + nodesGraph[IndexMiniDistance, i];
                        GraphDijkstra[IndexMiniDistance, i] = Remote[i];
                        UpstreamNodes[i] = IndexMiniDistance;
                    }
                }
                count++;
            } while (count != nodesGraph.GetLength(0));
            #endregion

            #region Graphic design of OutputPaths and NeighborsPatchs 
            // Creation of the patchs of the nodes of output ou neighbor, then addition in appropriate list  
            for (int i = 0; i < nodesGraph.GetLength(0); i++)
            {
                List<int> wayExit = new List<int>();
                List<int> wayNeighbor = new List<int>();
                // patch of output
                if (outputNodes[i] == 1)
                {
                    wayExit.Add(i);
                    CreatePatch(i, wayExit);
                    OutputPaths.Add(wayExit);
                }
                // patch of neighbor
                int nbSorties = 0;
                for (int j = 0; j < nodesGraph.GetLength(0); j++)
                {
                    GraphDijkstra[i, j] = int.MaxValue;// init
                    if (nodesGraph[i, j] == 1)
                    {
                        if (outputNodes[j] == 1)
                        {
                            nbSorties++;
                        }
                    }
                }
                if (nbSorties > 1)
                {
                    wayNeighbor.Add(i);
                    CreatePatch(i, wayNeighbor);
                    NeighborsPatchs.Add(wayNeighbor);
                }
                // Recurive function to create the patch of a node
                void CreatePatch(int index, List<int> chemin)
                {
                    for (int j = 0; j < nodesGraph.GetLength(0); j++)
                    {
                        if (UpstreamNodes[index] == j)
                        {
                            chemin.Add(j);
                            CreatePatch(j, chemin);
                        }
                    }
                }
            }
            #endregion

            #region Function minimun distance
            // Find the minimum distance of node not in graph
            static int MinimunDistance(int[] distances, bool[] isInGraphe)
            {
                int nimDistance = int.MaxValue;
                int indexMinDistance = -1;
                for (int i = 0; i < distances.Length; i++)
                {
                    if (isInGraphe[i] == false && distances[i] <= nimDistance)
                    {
                        nimDistance = distances[i];
                        indexMinDistance = i;
                    }
                }
                return indexMinDistance;
            }
            #endregion
        }
        #endregion
    }
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        int L = int.Parse(inputs[1]); // the number of links
        int E = int.Parse(inputs[2]); // the number of exit gateways
        int[,] Noeuds = new int[N, N]; // graphe des noeuds arètes
        int[] Sorties = new int[N]; // listes des noeuds de sortie
        for (int i = 0; i < L; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);
            Noeuds[N1, N2] = 1;
            Noeuds[N2, N1] = 1;
        }
        // définition des noeuds de sortie
        for (int i = 0; i < E; i++)
        {
            int EI = int.Parse(Console.ReadLine()); // the index of a gateway node
            Sorties[EI] = 1;
        }
        // game loop
        while (true)
        {
            int SI = int.Parse(Console.ReadLine());
            // début du chrono
            var watch = System.Diagnostics.Stopwatch.StartNew();
            (int, int) AreteACouper = Dijkstra_Arete(Noeuds, SI, Sorties);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Noeuds[AreteACouper.Item1, AreteACouper.Item2] = 0;
            Noeuds[AreteACouper.Item2, AreteACouper.Item1] = 0;
            Console.WriteLine(AreteACouper.Item1 + " " + AreteACouper.Item2);
        }

        // Fonction pour trouver l'arète à couper
        (int, int) Dijkstra_Arete(int[,] grapheNoeuds, int noeudEntree, int[] sorties)
        {
            Dijkstra G_DjK = new Dijkstra(grapheNoeuds, noeudEntree, sorties);
            return AreteACouper2();
            (int, int) AreteACouper2()
            {
                int ResultNoeud1 = -1;
                int ResultNoeud2 = -1;
                int minDistanceVoisin = int.MaxValue;
                int minDistanceSortie = int.MaxValue;
                int maxSortiesVoisin = int.MinValue;
                bool passSorties = true;
                // boucle sur les voisins
                if (!G_DjK.OutputPaths.Exists(x => G_DjK.Remote[x[0]] == 1))
                {

                    foreach (var cheminVoisin in G_DjK.NeighborsPatchs)
                    {
                        #region initialisation des variables de test
                        int nb_SortiesV = 0;
                        int index_SV = -1;
                        int nbAreteDeSortieSurChemin = 0;
                        int maxDegre = int.MinValue;
                        for (int i = 0; i < grapheNoeuds.GetLength(0); i++)
                        {
                            if (sorties[i] == 1)
                            {
                                if (grapheNoeuds[cheminVoisin[0], i] == 1)
                                {
                                    nb_SortiesV++; 
                                    if (G_DjK.NodesWeights[i] > maxDegre) // le voisin le plus proche
                                    {
                                        maxDegre = G_DjK.NodesWeights[i];
                                        index_SV = i; // index de la sortie attribuée au voisin pour définir l'arc à couper
                                    }
                                }
                            }

                        }
                        // compte le nombre d'arète entre un noeud amont à une sortie sur le chemin du voisin
                        nbAreteDeSortieSurChemin = cheminVoisin.FindAll(x => predicat_nbSortieSurChemin(x)).Count();
                        bool predicat_nbSortieSurChemin(int x)
                        {
                            bool result = false;
                            foreach (var cheminSortie in G_DjK.OutputPaths)
                            {
                                if (grapheNoeuds[x, cheminSortie[0]] != 0)
                                {
                                    result = true;
                                }
                            }
                            return result;
                        }
                        #endregion

                        if (G_DjK.Remote[cheminVoisin[0]] <= nbAreteDeSortieSurChemin)
                        {
                            if (G_DjK.Remote[cheminVoisin[0]] < minDistanceVoisin)
                            {
                                minDistanceVoisin = G_DjK.Remote[cheminVoisin[0]];
                                maxSortiesVoisin = nbAreteDeSortieSurChemin;
                                AttributionVoisin(index_SV, cheminVoisin[0]);
                                passSorties = false;
                            }
                            else if (G_DjK.Remote[cheminVoisin[0]] == minDistanceVoisin)
                            {
                                if (nb_SortiesV > maxSortiesVoisin)
                                {
                                    minDistanceVoisin = G_DjK.Remote[cheminVoisin[0]];
                                    maxSortiesVoisin = nbAreteDeSortieSurChemin;
                                    AttributionVoisin(index_SV, cheminVoisin[0]);
                                    passSorties = false;

                                }
                            }

                        }
                        else if (G_DjK.NeighborsPatchs.Count() == 1)// && !G_DjK.OutputPaths.Exists(x => Distances[x[0]]==1))
                        {
                            AttributionVoisin(index_SV, cheminVoisin[0]);
                            passSorties = false;
                        }
                    }
                }
                // boucle sur les sorties si pas d'arète attribuée par un des voisins
                if (passSorties)
                {
                    foreach (var cheminSortie in G_DjK.OutputPaths)
                    {
                        if (G_DjK.Remote[cheminSortie[0]] < minDistanceSortie)
                        {
                            minDistanceSortie = G_DjK.Remote[cheminSortie[0]];
                            AttributionSortie(cheminSortie[0]);
                        }
                    }

                }
                return (ResultNoeud1, ResultNoeud2);
                void AttributionSortie(int i)
                {
                    ResultNoeud1 = i;
                    ResultNoeud2 = G_DjK.UpstreamNodes[i];
                }
                void AttributionVoisin(int item1, int item3)
                {
                    ResultNoeud1 = item1;
                    ResultNoeud2 = item3;
                }

            }
        }
    }

    #region minimun distance
    public static int MinimunDistance2(int[] distances, bool[] noeudsDuChemin)
    {
        int nimDistance = int.MaxValue;
        int indexMinDistance = -1;
        for (int i = 0; i < distances.Length; i++)
        {
            if (noeudsDuChemin[i] == false && distances[i] <= nimDistance)
            {
                nimDistance = distances[i];
                indexMinDistance = i;
            }
        }
        return indexMinDistance;
    }
    #endregion

    #region PRINT
    public static void AfficheGraphChemin(int[,] GrapheDuChemin)
    {
        for (int i = 0; i < GrapheDuChemin.GetLength(0); i++)
        {
            string result = "";
            for (int j = 0; j < GrapheDuChemin.GetLength(0); j++)
            {
                if (GrapheDuChemin[i, j] == int.MaxValue)
                {
                    result += "0" + " ";

                }
                else
                {
                    result += GrapheDuChemin[i, j].ToString() + " ";
                }
            }
            Console.Error.WriteLine(result);
        }
    }
    public static void AfficherChemins(int[] chemins)
    {
        Console.Error.Write("Noeud	 Noeud voisin sur le chemin au virus\n");
        for (int i = 0; i < chemins.Length; i++)
            Console.Error.Write(i + " \t\t " + chemins[i] + "\n");
    }
    public static void printSolution(int[] dist, int n)
    {
        Console.Error.Write("Noeud	 Distance "
                    + "au point de départ\n");
        for (int i = 0; i < dist.Length; i++)
            Console.Error.Write(i + " \t\t " + dist[i] + "\n");
    }
    public static void printSortie(int[] sorties)
    {
        for (int i = 0; i < sorties.Length; i++)
            Console.Error.Write(i + " \t " + sorties[i] + "\n");
    }
    #endregion

}
