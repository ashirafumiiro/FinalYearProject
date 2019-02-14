using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LabServer");
        public ConfigWindow()
        {
            InitializeComponent();
            labUrlTextBox.Text = GetLocalIPAddress();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            string labName = labnameTextBox.Text.Trim();
            bool createLab = false;
            if (deviceCombobox.SelectedIndex != -1 & switchesComboBox.SelectedIndex != -1
                & channelsComboBox.SelectedIndex != -1 & !labName.Equals("")
                & !authorTextBox.Text.Trim().Equals("") )  //all fields with values
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
            List<Setting> channelsList = new List<Setting>();
            ComboBoxItem cbi = (ComboBoxItem)channelsComboBox.SelectedItem;
            //create channels list
            for (int i = 0; i <  int.Parse(cbi.Content.ToString()); i++)
            {
                channelsList.Add(new Setting
                {
                    Name = "Channel" + i,
                    Url = "/wave" + i,
                    DevicePath = "ai" + i,  // only support ai pins
                    Type = "Channel"
                }); 
            }

            XDocument labDoc =
            new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("The xml data required for starting the lab!"),
                new XElement("Settings", 
                from c in channelsList select new XElement("Setting",
                    new XAttribute("Name",c.Name), new XAttribute("Value", c.Url),
                    new XAttribute("DevicePath", c.DevicePath),
                    new XAttribute("Type", c.Type)
                    )
                )
             );


            cbi = (ComboBoxItem)switchesComboBox.SelectedItem;
            List<Setting> switchList = new List<Setting>();
            
            for (int i = 0; i < int.Parse(cbi.Content.ToString()); i++)
            {
                switchList.Add(new Setting
                {
                    Name = "S" + (i+1).ToString(),  //S1 is the first
                    Url = "S" + (i + 1).ToString(),
                    DevicePath = "DI" + i,  // First DI is 0
                    Type = "Switch"
                }); 
            }


            var switches = from s in switchList select new XElement("Setting",
                new XAttribute("Name", s.Name), new XAttribute("Value",s.Url),
                new XAttribute("DevicePath", s.DevicePath),
                new XAttribute("Type", s.Type));
            labDoc.Descendants("Settings").First().Add(switches);
            labDoc.Descendants("Settings").First().Add(
                new XElement("Setting", new XAttribute("Name", "Lab Name"),
                    new XAttribute("DevicePath", ""), new XAttribute("Value", labnameTextBox.Text),
                    new XAttribute("Type", "Info"))
                    );
            labDoc.Descendants("Settings").First().Add(
                new XElement("Setting", new XAttribute("Name", "Lab Author"),
                    new XAttribute("DevicePath", ""), new XAttribute("Value", authorTextBox.Text),
                    new XAttribute("Type", "Info"))
                    );
            labDoc.Descendants("Settings").First().Add(
                new XElement("Setting", new XAttribute("Name", "Lab Url"),
                    new XAttribute("DevicePath", ""), new XAttribute("Value", labUrlTextBox.Text),
                    new XAttribute("Type", "Info"))
                    );
            labDoc.Descendants("Settings").First().Add(
                new XElement("Setting", new XAttribute("Name", "Api Port"),
                    new XAttribute("DevicePath", ""), new XAttribute("Value", apiPortTexBox.Text),
                    new XAttribute("Type", "Info"))
                    );
            cbi = (ComboBoxItem)deviceCombobox.SelectedItem;
            string deviceName = cbi.Content.ToString();

            labDoc.Descendants("Settings").First().Add(
                new XElement("Setting", new XAttribute("Name", "Device"),
                    new XAttribute("DevicePath", ""), new XAttribute("Value", deviceName),
                    new XAttribute("Type", "Info"))
                    );

            //MessageBox.Show(labDoc.ToString());
            // Save to disk.
            labDoc.Save(filename);
            
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "";
        }
    }

    public class Setting
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string DevicePath { get; set; }
        public string Type { get; set; }
    }

}
