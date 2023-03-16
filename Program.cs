// https://www.codingame.com/ide/puzzle/death-first-search-episode-2


class Player
{
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
            (int, int) AreteACouper = Dijkstra_Arete(Noeuds, SI, Sorties);
            Noeuds[AreteACouper.Item1, AreteACouper.Item2] = 0;
            Noeuds[AreteACouper.Item2, AreteACouper.Item1] = 0;
            Console.WriteLine(AreteACouper.Item1 + " " + AreteACouper.Item2);
        }
        // Fonction pour trouver l'arète à couper
        (int, int) Dijkstra_Arete(int[,] grapheNoeuds, int noeudEntree, int[] Sorties)
        {
            int[] Distances = new int[grapheNoeuds.GetLength(0)]; // distances (somme des poids) la plus courte entre i et SI
            int[] NoeudAmont = new int[grapheNoeuds.GetLength(0)]; // chemin du point - noeud amont de i sur le chemin
            int[] DegreNoeuds = new int[grapheNoeuds.GetLength(0)]; // degre de charque noeud i
            List<List<int>> C_Sorties = new List<List<int>>();
            List<List<int>> C_Voisins = new List<List<int>>();

            InitAndDataGrafDijkstra();
            /// Initialisation et donnée pour les tableau:  Distances / NoeudAmont / SortiesPlus / Chemin
            void InitAndDataGrafDijkstra()
            {
                int[,] GrapheDuDijkstra = new int[grapheNoeuds.GetLength(0), grapheNoeuds.GetLength(0)]; // distance du Chemin le plus court
                bool[] NoeudsPrisEnCompte = new bool[grapheNoeuds.GetLength(0)]; // indique si le sommet est pris en compte dans le parcours

                #region création du graphe Dijkstra => Distances, NoeudAmont, DegreNoeuds
                for (int i = 0; i < grapheNoeuds.GetLength(0); i++)
                {
                    int nbDegret = 0;
                    Distances[i] = int.MaxValue;
                    NoeudAmont[i] = int.MinValue;
                    for (int j = 0; j < grapheNoeuds.GetLength(0); j++)
                    {
                        GrapheDuDijkstra[i, j] = int.MaxValue;
                        if (grapheNoeuds[i, j] == 1)
                        {
                            nbDegret++;
                        }
                    }
                    DegreNoeuds[i] = nbDegret;

                }
                Distances[noeudEntree] = 0;
                GrapheDuDijkstra[noeudEntree, noeudEntree] = 0;
                int count = 0;
                do
                {
                    int IndexMiniDistance = MinimunDistance2(Distances, NoeudsPrisEnCompte);
                    NoeudsPrisEnCompte[IndexMiniDistance] = true;
                    GrapheDuDijkstra[IndexMiniDistance, IndexMiniDistance] = Distances[IndexMiniDistance];

                    for (int i = 0; i < N; i++)
                    {

                        if (!NoeudsPrisEnCompte[i]
                            && grapheNoeuds[IndexMiniDistance, i] != 0
                            && Distances[IndexMiniDistance] != int.MaxValue
                            && Distances[IndexMiniDistance] + grapheNoeuds[IndexMiniDistance, i] < Distances[i]
                            && GrapheDuDijkstra[IndexMiniDistance, i] == int.MaxValue)
                        {
                            Distances[i] = Distances[IndexMiniDistance] + grapheNoeuds[IndexMiniDistance, i];
                            GrapheDuDijkstra[IndexMiniDistance, i] = Distances[i];
                            NoeudAmont[i] = IndexMiniDistance;
                        }
                    }
                    count++;
                } while (count != grapheNoeuds.GetLength(0));

                #region Création des chemins => C_Sorties, C_Voisins
                // création des listes chemins des sorties et celui des voisins
                // but minimiser les cycles
                for (int i = 0; i < grapheNoeuds.GetLength(0); i++)
                {
                    List<int> cheminS = new List<int>();
                    List<int> cheminV = new List<int>();
                    // chemins sorties
                    if (Sorties[i] == 1)
                    {
                        cheminS.Add(i);
                        R_Chemin2(i, cheminS);
                        C_Sorties.Add(cheminS);
                    }
                    // chemin voisin
                    int nbSorties = 0;
                    for (int j = 0; j < N; j++)
                    {
                        GrapheDuDijkstra[i, j] = int.MaxValue;// init
                        if (grapheNoeuds[i, j] == 1)
                        {
                            if (Sorties[j] == 1)
                            {
                                nbSorties++;
                            }
                        }
                    }
                    if (nbSorties > 1)
                    {
                        cheminV.Add(i);
                        R_Chemin2(i, cheminV);
                        C_Voisins.Add(cheminV);
                    }
                    //funtion création du chemin
                    void R_Chemin2(int index, List<int> chemin)
                    {
                        for (int j = 0; j < N; j++)
                        {
                            if (NoeudAmont[index] == j)
                            {
                                chemin.Add(j);
                                R_Chemin2(j, chemin);
                            }
                        }
                    }
                }
                #endregion

                #endregion
            }

            // trouver bramche à couper
            return AreteACouper2();
            (int, int) AreteACouper2()
            {
                int ResultNoeud1 = -1;
                int ResultNoeud2 = -1;
                //(int, int) result = (-1, -1);
                int minDistanceVoisin = int.MaxValue;
                int minDistanceSortie = int.MaxValue;
                int maxSortiesVoisin = int.MinValue;
                bool passSorties = true;
                // boucle sur les voisins
                if (!C_Sorties.Exists(x => Distances[x[0]] == 1))
                {
                    foreach (var cheminVoisin in C_Voisins)
                    {

                        #region initialisation des variables de test
                        int nb_SortiesV = 0;
                        int index_SV = -1;
                        int nbAreteDeSortieSurChemin = 0;
                        int maxDegre = int.MinValue;
                        //init 
                        for (int i = 0; i < grapheNoeuds.GetLength(0); i++)
                        {
                            if (Sorties[i] == 1)
                            {
                                if (grapheNoeuds[cheminVoisin[0], i] == 1)
                                {
                                    nb_SortiesV++; // nombre d'arète avec une sortie sur le chemin  du noeud voisin
                                    if (DegreNoeuds[i] > maxDegre) // le voisin le plus proche
                                    {
                                        maxDegre = DegreNoeuds[i];
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
                            foreach (var cheminSortie in C_Sorties)
                            {
                                if (grapheNoeuds[x, cheminSortie[0]] != 0)
                                {
                                    //nbSortieSurChemin++;
                                    //S_plus.Add(x);
                                    result = true;
                                }
                            }
                            return result;
                        }
                        #endregion

                        if (Distances[cheminVoisin[0]] <= nbAreteDeSortieSurChemin)
                        {
                            if (Distances[cheminVoisin[0]] < minDistanceVoisin)
                            {
                                minDistanceVoisin = Distances[cheminVoisin[0]];
                                maxSortiesVoisin = nbAreteDeSortieSurChemin;
                                AttributionVoisin(index_SV, cheminVoisin[0]);
                                passSorties = false;
                            }
                            else if (Distances[cheminVoisin[0]] == minDistanceVoisin)
                            {
                                if (nb_SortiesV > maxSortiesVoisin)
                                {
                                    minDistanceVoisin = Distances[cheminVoisin[0]];
                                    maxSortiesVoisin = nbAreteDeSortieSurChemin;
                                    AttributionVoisin(index_SV, cheminVoisin[0]);
                                    passSorties = false;
                                }
                            }
                        }
                        else if (C_Voisins.Count() == 1)
                        {
                            AttributionVoisin(index_SV, cheminVoisin[0]);
                            passSorties = false;
                        }
                    }
                }
                // boucle sur les sorties si pas d'arète attribuée par un des voisins
                if (passSorties)
                {
                    foreach (var cheminSortie in C_Sorties)
                    {
                        if (Distances[cheminSortie[0]] < minDistanceSortie)
                        {
                            minDistanceSortie = Distances[cheminSortie[0]];
                            AttributionSortie(cheminSortie[0]);
                        }
                    }

                }
                return (ResultNoeud1, ResultNoeud2);
                void AttributionSortie(int i)
                {
                    ResultNoeud1 = i;
                    ResultNoeud2 = NoeudAmont[i];
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
}