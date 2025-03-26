using Microsoft.VisualStudio.TestTools.UnitTesting;
using LivInParis.Models;
using System;


namespace LivInParis.Tests
{
    [TestClass]
    public class TestsGraphe
    {
        [TestMethod]
        public void Test_AjouterNoeudEtLien()
        {
            var graphe = new Graphe<int>(RepresentationMode.Liste);

            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(1, 2);

            Assert.IsTrue(graphe.Noeuds.ContainsKey(0), "Le nœud 0 devrait être présent");
            Assert.IsTrue(graphe.Noeuds.ContainsKey(1), "Le nœud 1 devrait être présent");
            Assert.IsTrue(graphe.Noeuds.ContainsKey(2), "Le nœud 2 devrait être présent");
            Assert.AreEqual(2, graphe.Liens.Count, "Il devrait y avoir exactement 2 liens");
        }

        [TestMethod]
        public void Test_ParcoursLargeurEtProfondeur_Connexite()
        {
            var graphe = new Graphe<int>(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(0, 2);
            graphe.AjouterLien(1, 3);
            graphe.AjouterLien(2, 4);

            bool estConnexe = graphe.EstConnexe();

            Assert.IsTrue(estConnexe, "Le graphe devrait être connexe");
        }

        [TestMethod]
        public void Test_DetectionCycle_ContientCycle()
        {
            var graphe = new Graphe<int>(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(1, 2);
            graphe.AjouterLien(2, 0);

            bool contientCycle = graphe.ContientCycle();

            Assert.IsTrue(contientCycle, "Le graphe devrait contenir un cycle");
        }

        [TestMethod]
        public void Test_DetectionCycle_SansCycle()
        {
            var graphe = new Graphe<int>(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(1, 2);
            graphe.AjouterLien(2, 3);

            bool contientCycle = graphe.ContientCycle();

            Assert.IsTrue(contientCycle, "Le graphe ne devrait pas contenir de cycle");
        }

        [TestMethod]
        public void Test_Visualisation_FichierCree()
        {
            var graphe = new Graphe<int>(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(1, 2);
            string cheminFichier = Path.Combine(Path.GetTempPath(), "test_graphe.png");

            GraphVisualizer.Visualize(graphe, cheminFichier);

            Assert.IsTrue(File.Exists(cheminFichier), "Le fichier de visualisation du graphe devrait exister");
            FileInfo fi = new FileInfo(cheminFichier);
            Assert.IsTrue(fi.Length > 0, "Le fichier de visualisation du graphe ne devrait pas être vide");

            File.Delete(cheminFichier);
        }

        [TestMethod]
        public void Test_Dijkstra()
        {
            var graphe = new Graphe<int>(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1, 1.0);
            graphe.AjouterLien(0, 2, 4.0);
            graphe.AjouterLien(1, 2, 2.0);
            graphe.AjouterLien(1, 3, 5.0);
            graphe.AjouterLien(2, 3, 1.0);

            var distances = graphe.Dijkstra(0);

            Assert.AreEqual(0.0, distances[0], 0.001, "Distance de 0 à 0 devrait être 0");
            Assert.AreEqual(1.0, distances[1], 0.001, "Distance de 0 à 1 devrait être 1");
            Assert.AreEqual(3.0, distances[2], 0.001, "Distance de 0 à 2 devrait être 3");
            Assert.AreEqual(4.0, distances[3], 0.001, "Distance de 0 à 3 devrait être 4");
        }

        [TestMethod]
        public void Test_BellmanFord()
        {
            var graphe = new Graphe<int>(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1, 1.0);
            graphe.AjouterLien(0, 2, 4.0);
            graphe.AjouterLien(1, 2, 2.0);
            graphe.AjouterLien(1, 3, 5.0);
            graphe.AjouterLien(2, 3, 1.0);

            var distances = graphe.BellmanFord(0);

            Assert.AreEqual(0.0, distances[0], 0.001, "Distance de 0 à 0 devrait être 0");
            Assert.AreEqual(1.0, distances[1], 0.001, "Distance de 0 à 1 devrait être 1");
            Assert.AreEqual(3.0, distances[2], 0.001, "Distance de 0 à 2 devrait être 3");
            Assert.AreEqual(4.0, distances[3], 0.001, "Distance de 0 à 3 devrait être 4");
        }

        [TestMethod]
        public void Test_ReconstruireChemin()
        {
            var graphe = new Graphe<int>(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1, 1.0);
            graphe.AjouterLien(1, 2, 2.0);
            graphe.AjouterLien(2, 3, 1.0);

            var distances = new Dictionary<int, double>
            {
                { 0, 0.0 },
                { 1, 1.0 },
                { 2, 3.0 },
                { 3, 4.0 }
            };

            var predecesseurs = new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 1 },
                { 3, 2 }
            };

            var chemin = graphe.ReconstruireChemin(0, 3, predecesseurs);

            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 3 }, chemin, "Le chemin reconstruit n'est pas correct");
        }
    }
}
