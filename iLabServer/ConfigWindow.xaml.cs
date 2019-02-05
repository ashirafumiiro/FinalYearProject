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
using System.Xml.Linq;

namespace iLabServer
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        string workingPath = 
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LabServer");
        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            string labName = nameTextBox.Text.Trim();
            bool createLab = false;
            if (deviceCombobox.SelectedIndex != -1 & switchesComboBox.SelectedIndex != -1
                & channelsComboBox.SelectedIndex != -1 & !labName.Equals("")
                & !authorTextBox.Text.Trim().Equals(""))  //all fields with values
            {

                createLab = true;
            }
            else
            {
                MessageBox.Show("Fill out all Required Feilds");
            }

            if (createLab)
            {
                try
                {
                    
                    if (!Directory.Exists(workingPath))
                    {
                        Directory.CreateDirectory(workingPath);
                    }
                    string labsDir = System.IO.Path.Combine(workingPath, "Labs");
                    if (!Directory.Exists(labsDir))
                    {
                        Directory.CreateDirectory(labsDir);//for storing labs
                    }
                    
                    CreateFullXDocument(
                        System.IO.Path.Combine(labsDir,labName+".xml"));
                    MessageBox.Show("Saved");
                    //this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed To save\n" + ex.Message);
                }
            }            

        }

        private void CreateFullXDocument(string filename)
        {
            XDocument labDoc =
            new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("The xml data required for starting the lab!"));

            List<Channel> channelsList = new List<Channel>();
            ComboBoxItem cbi = (ComboBoxItem)channelsComboBox.SelectedItem;
            //create channels list
            for (int i = 0; i <  int.Parse(cbi.Content.ToString()); i++)
            {
                channelsList.Add(new Channel
                {
                    Name = "Channel" + i,
                    Url = labUrlTextBox.Text + "/wave" + i,
                    DevicePath = "ai" + i  // only support ai pins
                }); 
            }

            XElement channelsElement = new XElement("Channels", 
                from c in channelsList select new XElement("Channel",
                    new XAttribute("Name",c.Name), new XAttribute("Url", c.Url),
                    new XAttribute("DevicePath", c.DevicePath))
                );


            cbi = (ComboBoxItem)switchesComboBox.SelectedItem;
            
            for (int i = 0; i < int.Parse(cbi.Content.ToString()); i++)
            {
                
            }
            /*
            XDocument labDoc =
            new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("The xml data required for starting the lab!"),
                new XElement("Settings",
                    new XElement("Setting", new XAttribute("ID", "channel"),
                    new XElement("Name", "Green"),
                    new XElement("Url", "BMW"),
                    new XElement("PetName", "Stan")
                 ),
                new XElement("Car", new XAttribute("ID", "2"),
                    new XElement("Color", "Pink"),
                    new XElement("Make", "Yugo"),
                    new XElement("PetName", "Melvin")
                    )
                )
            );

            // Save to disk.
            labDoc.Save(filename);
            */
        }
    }

    public class Channel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string DevicePath { get; set; }

    }
}
