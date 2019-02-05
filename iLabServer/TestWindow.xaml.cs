using System;
using System.Collections.Generic;
using System.Data;
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
using System.Xml.Linq;
using System.Xml;

namespace iLabServer
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public string PetName { get; set; }
        public string Color { get; set; }
        //public int Speed { get; set; }
        public string Make { get; set; } 

        public TestWindow()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, RoutedEventArgs e)
        {
            
            /*
            XDocument inventoryDoc = XDocument.Load("Inventory.xml");
            // Find the colors for a given make.
            var makeInfo = from car in inventoryDoc.Descendants("Car")
                               //where (string)car.Element("Make") == "BMW"
                           select new
                           {
                               PetName = (string)car.Element("PetName"),
                               Make = (string)car.Element("Make"),
                               Color = (string)car.Element("Color")
                           };//car.Element("Color").Value;
            // Build a string representing each color.
            string data = string.Empty;
            foreach (var item in makeInfo)
            {
                data += string.Format("PetName {0}, Color {1}, Make {2}\n", item.PetName,
                    item.Color, item.Make);
            }


            //DataSet dataSet = new DataSet();
            //dataSet.ReadXml(@"Inventory.xml");
            ListCollectionView Cars = new ListCollectionView(makeInfo.ToList());
            Cars.GroupDescriptions.Add(new PropertyGroupDescription("Make"));
            //dataGrid1.ItemsSource = Cars;
            
            
            //dataGrid1.ItemsSource = dataSet.Tables[0].DefaultView;

            */

        }

    }
}
