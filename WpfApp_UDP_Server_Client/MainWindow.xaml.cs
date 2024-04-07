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
            buttonFindRecipe.Visibility = Visibility.Hidden;
            boxRecipes.Visibility = Visibility.Hidden;
            AddRecipesInBox(boxRecipes);
            infoChangeIngredients.Visibility = Visibility.Hidden;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textboxNickName.Text.Length > 3) buttonConnectToServer.IsEnabled = true;
        }

        private void buttonConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress networkIPAddress = iPHostEntry.AddressList.FirstOrDefault(
                ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                && !IPAddress.IsLoopback(ip));
            int port = 55555;

            UDPServer.StartServer()
        }
    }
}
