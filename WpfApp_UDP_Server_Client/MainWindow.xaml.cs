using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using WpfApp_UDP_Server_Client.ServerContent;

namespace WpfApp_UDP_Server_Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<string> GetListRecipes()
        {
            List<string> ingredietes = new List<string>()
            {
                "Салат Оливье",
                "Суп борщ",
                "Яишница с беконом"
            };

            return ingredietes;
        }

        private static void AddRecipesInBox(ComboBox comboBox)
        {
            foreach (string item in GetListRecipes())
            {
                comboBox.Items.Add(item);
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            AddRecipesInBox(boxRecipes);
            OnStateConnection(false);
            boxRecipes.SelectedIndex = 0;
            UDPClient.MessageFromServer += Client_MessageFromServer;
            UDPClient.ResponseFromServer += Client_ResponseFromServer;
        }

        private void Client_ResponseFromServer(object sender, KitchenRecipe e)
        {
            if(e != null)
            {
                RecipeWindow recipeWindow = new RecipeWindow(e);
                recipeWindow.ShowDialog();
            }
        }

        private void Client_MessageFromServer(object sender, string e)
        {
            if (e.Substring(e.Length - 2) == "-s")
            {
                MessageBox.Show(e.Substring(0, e.Length - 2), "Server", MessageBoxButton.OK, MessageBoxImage.Error);
                OnStateConnection(false);
            }
            else if (e.Substring(e.Length - 2) == "-r")
            {
                MessageBox.Show(e.Substring(0, e.Length - 2), "Server", MessageBoxButton.OK, MessageBoxImage.Information);
                buttonConnectToServer.IsEnabled = false;
                textboxNickName.Text = "";
            }
            else if (e.Substring(e.Length - 2) == "-e")
            {
                MessageBox.Show(e.Substring(0, e.Length - 2), "Server", MessageBoxButton.OK, MessageBoxImage.Information);
                OnStateConnection(false);
                buttonConnectToServer.IsEnabled = false;
                textboxNickName.Text = "";
                messageFromServer.Content = "";
            }
            else
            {
                messageFromServer.Content = e.Substring(0,e.Length-2);
                OnStateConnection(true);
            }

        }

        //private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (textboxNickName.Text.Length > 3) buttonConnectToServer.IsEnabled = true;
        //}

        private async void buttonConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress networkIPAddress = iPHostEntry.AddressList.FirstOrDefault(
                ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                && !IPAddress.IsLoopback(ip));
            int port = 49165;

            await UDPClient.StartClient(networkIPAddress, port, textboxNickName.Text);
        }

        private void OnStateConnection(bool flag)
        {
            if (flag)
            {
                buttonConnectToServer.Visibility = Visibility.Hidden;
                infoNickName.Visibility = Visibility.Hidden;
                textboxNickName.Visibility = Visibility.Hidden;
                buttonFindRecipe.Visibility = Visibility.Visible;
                boxRecipes.Visibility = Visibility.Visible;
                infoMessageServer.Visibility = Visibility.Visible;
                infoChangeIngredients.Visibility = Visibility.Visible;
                infoConnectionState.Content = "ON";
                infoConnectionState.Background = Brushes.Green;
            }
            else
            {
                buttonConnectToServer.Visibility = Visibility.Visible;
                infoNickName.Visibility = Visibility.Visible;
                textboxNickName.Visibility = Visibility.Visible;
                buttonFindRecipe.Visibility = Visibility.Hidden;
                boxRecipes.Visibility = Visibility.Hidden;
                infoMessageServer.Visibility = Visibility.Hidden;
                infoChangeIngredients.Visibility = Visibility.Hidden;
                infoConnectionState.Content = "OFF";
                infoConnectionState.Background = Brushes.Red;
            }
        }


        private void textboxNickName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textboxNickName.Text.Length > 3)
            {
                buttonConnectToServer.IsEnabled = true;
            }
        }

        private void buttonFindRecipe_Click(object sender, RoutedEventArgs e)
        {
            UDPClient.SendToServer(boxRecipes.SelectedItem.ToString());
        }
    }
}
