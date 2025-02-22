﻿using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prog_final
{
    internal class SingletonAdherent
    {
        MySqlConnection con;
        ObservableCollection<Adherent> liste_des_adherents;
        static SingletonAdherent instance = null;

        public SingletonAdherent()
        {
            con = new MySqlConnection("Server=cours.cegep3r.info;Database=420345ri_gr00001_2366599-etienne-mac-donald;Uid=2366599;Pwd=2366599;");
            liste_des_adherents = new ObservableCollection<Adherent>();
        }

        // SINGLETON
        public static SingletonAdherent getInstance()
        {
            if (instance == null)
            {
                instance = new SingletonAdherent();
            }
            return instance;
        }

        public ObservableCollection<Adherent> getliste_des_adherents()
        {
            return liste_des_adherents;
        }

        // ajoute les adherents dans la liste (va les chercher dans la bd)
        public void getToutAdherent()
        {
            liste_des_adherents.Clear();
            try
            {
                MySqlCommand commande = new MySqlCommand();
                commande.Connection = con;
                commande.CommandText = "SELECT * FROM adherent;";

                con.Open();
                MySqlDataReader r = commande.ExecuteReader();

                while (r.Read())
                {
                    string nom = r.GetString("nom");
                    string prenom = r.GetString("prenom");
                    string adresse = r.GetString("adresse");
                    string dateNaissance = r.GetString("dateNaissance");

                    Adherent adherent = new Adherent(nom, prenom, adresse, dateNaissance);
                    liste_des_adherents.Add(adherent);
                }

                con.Close();
            }
            catch (Exception ex)
            {
                // vérification que la connection est ouverte, pour la fermer
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // utilise le fichier csv pour les ajouter dans la liste
        public async void ajoutCSV()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".csv");

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(GestionWindow.mainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

            Windows.Storage.StorageFile monFichier = await picker.PickSingleFileAsync();
            if (monFichier == null)
            {
                return;
            }

            var lignes = await Windows.Storage.FileIO.ReadLinesAsync(monFichier);

            foreach (var ligne in lignes)
            {
                var v = ligne.Split(";");

                addAdherent(v[0], v[1], v[2], v[3]);
            }
        }

        // ajoute les adherents dans la bd
        public void addAdherent(string _nom, string _prenom, string _adresse, string _dateNaissance)
        {
            try
            {
                MySqlCommand commande = new MySqlCommand();
                commande.Connection = con;
                commande.CommandText = "INSERT INTO adherent (nom, prenom, adresse, dateNaissance) VALUES (@nom, @prenom, @adresse, @dateNaissance);";

                commande.Parameters.AddWithValue("@nom", _nom);
                commande.Parameters.AddWithValue("@prenom", _prenom);
                commande.Parameters.AddWithValue("@adresse", _adresse);
                commande.Parameters.AddWithValue("@dateNaissance", _dateNaissance);

                con.Open();
                commande.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }

                Console.WriteLine(ex.Message);
            }
            getToutAdherent();
        }



    }
}
