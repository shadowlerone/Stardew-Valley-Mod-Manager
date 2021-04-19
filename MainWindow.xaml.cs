using System;
using System.Collections.Generic;
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
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace Names
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<String,Mod> EnabledMods = new Dictionary<string, Mod>();
        private Dictionary<String, Mod> DisabledMods = new Dictionary<string, Mod>();
        private Dictionary<String, Mod> Mods = new Dictionary<string, Mod>();
        public MainWindow()
        {
            InitializeComponent();
            ModDir();
        }
        private void ModDir() {
            if (Directory.Exists(installPath.Text))
            {
                if (File.Exists(installPath.Text + "\\Stardew Valley.exe"))
                {
                    if (!Directory.Exists(installPath.Text + "\\Mods"))
                    {
                        Directory.CreateDirectory(installPath.Text + "\\Mods");
                    }

                    if (!Directory.Exists(installPath.Text + "\\Disabled Mods"))
                    {
                        Directory.CreateDirectory(installPath.Text + "\\Disabled Mods");
                    }
                    SetupMods(installPath.Text);
                    //ProcessDirectory(installPath.Text + "\\Disabled Mods");
                }
            }
        }
        private void resetMods() {
            EnabledMods.Clear();
            DisabledMods.Clear();
            lstDisabled.Items.Clear();
            lstEnabled.Items.Clear();
            Mods.Clear();
            ModDir();
        }
		private void SetupMods(string dir)
		{
            string[] DisabledsubdirectoryEntries = Directory.GetDirectories(dir + "\\Disabled Mods");
            foreach (string subdirectory in DisabledsubdirectoryEntries)
            {
                Mod mod = ProcessMod(subdirectory);
                Mods.Add(mod.Name, mod);
                DisabledMods.Add(mod.Name, mod);
                lstDisabled.Items.Add(mod.Name);
            }
            string[] EnabledsubdirectoryEntries = Directory.GetDirectories(dir + "\\Mods");
            foreach (string subdirectory in EnabledsubdirectoryEntries){
                Mod mod = ProcessMod(subdirectory);
                Mods.Add(mod.Name, mod);
                EnabledMods.Add(mod.Name, mod);
                lstEnabled.Items.Add(mod.Name);
            }  
        }

		private void ButtonAddName_Click(object sender, RoutedEventArgs e)
        {
        }

        private void reloadFilePath(object sender, RoutedEventArgs e)
        {
            resetMods();
        }
		private Mod ProcessMod(string subdirectory)
		{
            using (StreamReader file = File.OpenText(subdirectory + "\\manifest.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                Mod mod = (Mod)serializer.Deserialize(file, typeof(Mod));
                mod.FilePath = subdirectory;
                return mod;
            }
		}
		
		private void removeMod_Click(object sender, RoutedEventArgs e)
		{
            String destPath = installPath.Text + "\\Disabled Mods\\" + lstEnabled.SelectedItem.ToString();
            if (!Directory.Exists(installPath.Text + "\\Disabled Mods\\" + lstEnabled.SelectedItem.ToString()))
            {
                Directory.Move(Mods[lstEnabled.SelectedItem.ToString()].FilePath, destPath);
                resetMods();
            }
            
        }
		private void addMod_Click(object sender, RoutedEventArgs e)
		{
            String destPath = installPath.Text + "\\Mods\\" + lstDisabled.SelectedItem.ToString();
            if (!Directory.Exists(installPath.Text + "\\Mods\\" + lstDisabled.SelectedItem.ToString()))
            {
                Directory.Move(Mods[lstDisabled.SelectedItem.ToString()].FilePath, destPath);
                resetMods();
            }
        }

		private void selectMod_Click(object sender, RoutedEventArgs e)
		{
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                txtName.Text = openFileDialog.FileName;
        }

		private void lstEnabled_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            if (e.AddedItems.Count > 0) {
                debug.Text = e.AddedItems[0].ToString();
                Description.Text = Mods[e.AddedItems[0].ToString()].Description;
                lstDisabled.UnselectAll();
                addMod.Visibility = Visibility.Hidden;
                removeMod.Visibility = Visibility.Visible;
            } 
        }

		private void lstDisabled_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            if (e.AddedItems.Count > 0)
            {
                debug.Text = e.AddedItems[0].ToString();
                Description.Text = Mods[e.AddedItems[0].ToString()].Description;
                lstEnabled.UnselectAll();
                addMod.Visibility = Visibility.Visible;
                removeMod.Visibility = Visibility.Hidden;
            }
        }

		private void installMod_Click(object sender, RoutedEventArgs e)
		{
            String APPDATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String extractPath = APPDATA + "\\shadowlerone\\StardewModManager\\extracted";
            ZipFile.ExtractToDirectory(txtName.Text, extractPath);
            String modPath = FindMod(extractPath);
            if (modPath != "Error"){
                Mod mod = ProcessMod(modPath);
                String destPath = installPath.Text + "\\Mods\\" + mod.Name;
                Directory.Move(modPath, destPath);
                resetMods();
            }
            Directory.Delete(extractPath, true);
        }
        private String FindMod(string directory){
            if (File.Exists(directory + "\\manifest.json")){
                return directory;
			} else {
                string[] subdirectories = Directory.GetDirectories(directory);
                if (subdirectories.Length > 0)
                {
                    foreach (string subdirectory in subdirectories)
                    {
                        return FindMod(subdirectory);
                    }
                    return "Error";
                } else {
                    return "Error";
				}
            }
		}
	}
}
