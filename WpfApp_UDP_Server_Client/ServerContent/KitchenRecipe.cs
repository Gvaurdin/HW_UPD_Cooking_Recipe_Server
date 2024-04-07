using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace WpfApp_UDP_Server_Client.ServerContent
{
    public class KitchenRecipe
    {
        public string name { get; set; }
        public string image { get; set; }

        public List<string> ingredients { get; set; }

        public KitchenRecipe() { }

        public KitchenRecipe(string name, string image, List<string> ingredients)
        {
            this.name = name;
            this.image = image;
            this.ingredients = ingredients;
        }

        public byte[] Serialize()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using(MemoryStream stream = new MemoryStream()) 
            {
                binaryFormatter.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        public static KitchenRecipe Deserialize(byte[] bytesKitch)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using(MemoryStream stream = new MemoryStream()) 
            {
                return (KitchenRecipe)binaryFormatter.Deserialize(stream);
            }
        }

        public static List<KitchenRecipe> CreateListKitchenRecipes()
        {
            List<KitchenRecipe> kitchenRecipes = new List<KitchenRecipe>()
            {
                new KitchenRecipe("Оливье", "images/olivier_salad.jpeg",
                new List<string>{"Вареная колбаса", "Картофель", "Морковь", "Соленые огурцы", "Яйцо куриное", "зеленый горошек", "Соль", "Майонез" }),

                new KitchenRecipe ("Суп борщ", "images/soup_borscht.jpeg",
                new List<string>{"Свекла","Морковь", "Петрушка корень", "Капуста", "Картофель", "Лук", "Чеснок", "Помидоры", "Фасоль", "Перец сладкий", "Зелень укропа"}),

                new KitchenRecipe("Яишница с беконом", "images/scrambled_eggs_with_bacon.jpg",
                new List<string>{"Яйцо куриное", "Бекон"})
            };

            return kitchenRecipes;
        }

    }
}
