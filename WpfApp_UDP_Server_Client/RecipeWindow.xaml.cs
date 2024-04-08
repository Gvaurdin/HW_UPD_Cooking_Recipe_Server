using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WpfApp_UDP_Server_Client.ServerContent;

namespace WpfApp_UDP_Server_Client
{
    /// <summary>
    /// Логика взаимодействия для RecipeWindow.xaml
    /// </summary>
    public partial class RecipeWindow : Window
    {
        public RecipeWindow(KitchenRecipe kitchenRecipe)
        {
            InitializeComponent();
            nameRecipe.Content = kitchenRecipe.name;
            imageRecipe.Source = new BitmapImage(new Uri(kitchenRecipe.image, UriKind.RelativeOrAbsolute));
            foreach(string ingridient in kitchenRecipe.ingredients)
            {
                listboxIngridients.Items.Add(ingridient);
            }
        }
    }
}
