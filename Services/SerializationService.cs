using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using LivInParis.Models;

namespace LivInParis.Services
{
    public class SerializationService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        #region JSON Serialization

        public static void ExporterPlatsJSON(List<Plat> plats, string cheminFichier)
        {
            string json = JsonSerializer.Serialize(plats, _jsonOptions);
            File.WriteAllText(cheminFichier, json);
        }

        public static List<Plat> ImporterPlatsJSON(string cheminFichier)
        {
            if (!File.Exists(cheminFichier))
                throw new FileNotFoundException($"Le fichier {cheminFichier} n'existe pas.");

            string json = File.ReadAllText(cheminFichier);
            return JsonSerializer.Deserialize<List<Plat>>(json, _jsonOptions) ?? new List<Plat>();
        }

        public static void ExporterCommandesJSON(List<Commande> commandes, string cheminFichier)
        {
            string json = JsonSerializer.Serialize(commandes, _jsonOptions);
            File.WriteAllText(cheminFichier, json);
        }

        public static List<Commande> ImporterCommandesJSON(string cheminFichier)
        {
            if (!File.Exists(cheminFichier))
                throw new FileNotFoundException($"Le fichier {cheminFichier} n'existe pas.");

            string json = File.ReadAllText(cheminFichier);
            return JsonSerializer.Deserialize<List<Commande>>(json, _jsonOptions) ?? new List<Commande>();
        }

        #endregion

        #region XML Serialization

        public static void ExporterPlatsXML(List<Plat> plats, string cheminFichier)
        {
            var serializer = new XmlSerializer(typeof(List<Plat>));
            using var writer = new StreamWriter(cheminFichier);
            serializer.Serialize(writer, plats);
        }

        public static List<Plat> ImporterPlatsXML(string cheminFichier)
        {
            if (!File.Exists(cheminFichier))
                throw new FileNotFoundException($"Le fichier {cheminFichier} n'existe pas.");

            var serializer = new XmlSerializer(typeof(List<Plat>));
            using var reader = new StreamReader(cheminFichier);
            var result = serializer.Deserialize(reader);
            return result != null ? (List<Plat>)result : new List<Plat>();
        }

        public static void ExporterCommandesXML(List<Commande> commandes, string cheminFichier)
        {
            var serializer = new XmlSerializer(typeof(List<Commande>));
            using var writer = new StreamWriter(cheminFichier);
            serializer.Serialize(writer, commandes);
        }

        public static List<Commande> ImporterCommandesXML(string cheminFichier)
        {
            if (!File.Exists(cheminFichier))
                throw new FileNotFoundException($"Le fichier {cheminFichier} n'existe pas.");

            var serializer = new XmlSerializer(typeof(List<Commande>));
            using var reader = new StreamReader(cheminFichier);
            var result = serializer.Deserialize(reader);
            return result != null ? (List<Commande>)result : new List<Commande>();
        }

        #endregion
    }
}