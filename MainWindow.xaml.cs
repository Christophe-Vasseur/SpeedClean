using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpeedClean
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Dossier du répertoire Windows (en principe c:\Windows\Temp)
        public DirectoryInfo winTempDirectory;

        // Dossier temporaires des applications de l'utilisateur courant
        // C:\Users\<nom utilisateur>\AppData\Local\Temp
        public DirectoryInfo appTempDirectory; 

        public MainWindow()
        {
            InitializeComponent();
            winTempDirectory = new DirectoryInfo(@"C:\Windows\Temp");
            appTempDirectory = new DirectoryInfo(path: System.IO.Path.GetTempPath());

        }

        // Calcul de la taille d'un dossier
        public long DirectorySize(DirectoryInfo dir)
        {
            // calcul la somme de la taille de tous les fichiers à la racine
            long calcul = dir.GetFiles().Sum(Fi => Fi.Length);
                        
            // Boucle sur tous les répertoires
            // Pour chaque répertoire on calcule la taille de tous ces fichiers
            
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                try
                {
                    calcul += d.EnumerateFiles().Sum(file => file.Length);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            
            return calcul;

        }

        // Supprime les fichier d'un dossier
        // Retourne le nombre de fichiers supprimes
        public int ClearDirectory(DirectoryInfo dir)
        {
            int nbFileDelete = 0; // Nombre de fichier supprimer

            // Boucle sur les fichiers à la racine
            // Supprime tous les fichiers à la racine
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    file.Delete();
                    nbFileDelete++;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            // Boucle sur les dossiers
            // Supprime les repertoires à la racine 
            
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                try
                {
                    // true indique une suppression récursive (l'ensemble du contenu du dossier)
                    d.Delete(true);
                    nbFileDelete++;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return nbFileDelete;
        }


        private void Button_Analyser(object sender, RoutedEventArgs e)
        {
            Label_Bandeau.Content = "Effacer en un clic vos fichiers temporaires";
            AnalyzeFolders();
        }

        private void Button_Nettoyer(object sender, RoutedEventArgs e)
        {
            long nbFichier = 0;
            

            Label_Bandeau.Content = "        *** NETTOYAGE EN COURS ***";

            // Nettoyer le presse-papier
            if (Checkbox_Presse_Papier.IsChecked == true)
            {
                Clipboard.Clear();
            }

            
            try
            {
                nbFichier = ClearDirectory(winTempDirectory);
            } catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            
            
            try
            {
                nbFichier += ClearDirectory(appTempDirectory);
            } catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            

            Label_Bandeau.Content = "        *** NETTOYAGE TERMINE ***";
            label_Infos.Content = nbFichier.ToString() + " fichiers supprimés !";
            Btn_Nettoyer.IsEnabled = false;


        }

        private void Button_Goto_Site(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://www.eugenetoons.fr")
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Navigateur non trouvé !",  "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Goto_Github(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/Christophe-Vasseur/SpeedClean")
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Navigateur non trouvé !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Analyse les dossiers temporaires
        // Calcul de la taille totale
        public void AnalyzeFolders()
        {           

            long totalSize = 0;    // Taille totale qui peut être nettoyée

            totalSize = DirectorySize(winTempDirectory) / 1000000L;
            totalSize += DirectorySize(appTempDirectory) / 1000000L;

            label_Infos.Content = totalSize.ToString() + " MB (Fichiers & Dossiers)";
            Btn_Nettoyer.IsEnabled = true;


        }
    }
}
