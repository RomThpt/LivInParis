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
            var graphe = new Graphe(RepresentationMode.Liste);

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
            var graphe = new Graphe(RepresentationMode.Liste);
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
            var graphe = new Graphe(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(1, 2);
            graphe.AjouterLien(2, 0);

            bool contientCycle = graphe.ContientCycle();

            Assert.IsTrue(contientCycle, "Le graphe devrait contenir un cycle");
        }

        [TestMethod]
        public void Test_DetectionCycle_SansCycle()
        {
            var graphe = new Graphe(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(1, 2);
            graphe.AjouterLien(2, 3);

            bool contientCycle = graphe.ContientCycle();

            Assert.IsTrue(contientCycle, "Le graphe ne devrait pas contenir de cycle");
        }

        [TestMethod]
        public void Test_Visualisation_FichierCree()
        {
            var graphe = new Graphe(RepresentationMode.Liste);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(1, 2);
            string cheminFichier = Path.Combine(Path.GetTempPath(), "test_graphe.png");

            GraphVisualizer.Visualize(graphe, cheminFichier);

            Assert.IsTrue(File.Exists(cheminFichier), "Le fichier de visualisation du graphe devrait exister");
            FileInfo fi = new FileInfo(cheminFichier);
            Assert.IsTrue(fi.Length > 0, "Le fichier de visualisation du graphe ne devrait pas être vide");

            File.Delete(cheminFichier);
        }
    }
}
