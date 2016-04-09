using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using LooksFamiliar.Microservices.Config.Admin.SDK;
using LooksFamiliar.Microservices.Config.Models;

namespace ConfigMConsole
{
    public class Microservice
    {
        public string icon { get; set; }
        public string name { get; set; }
    }

    public class Attribute
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConfigM _configM;
        private Manifests _manifests;
        private Manifest _newModel;
        private Manifest _curManifest;

        private bool IsEditing { get; set; }
        private bool IsNew { get; set; }

        public MainWindow()
        {
            IsEditing = false;
            IsNew = false;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeSDK();
            InitializeUx();
        }

        private void InitializeSDK()
        {
            _configM = new ConfigM
            {
                DevKey = "",
                ApiUrl = ConfigurationManager.AppSettings["ConfigM"]
            };
        }

        private void InitializeUx()
        {
            MicroserviceList.Items.Clear();

            _manifests = _configM.GetAll();

            var services = (from m in _manifests.list let path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) select new Microservice {icon = path + @"\images\MicroserviceIcon.png", name = m.name}).ToList();
            foreach (var service in services)
            {
                MicroserviceList.Items.Add(service);
            }

            MicroserviceList.SelectedIndex = 0;
            MicroserviceName.Focus();
        }

        private void MicroserviceName_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsEditing = true;
            SaveButton.IsEnabled = true;
        }

        private void MicroserviceDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsEditing = true;
            SaveButton.IsEnabled = true;
        }

        private void MicroserviceCacheTTL_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsEditing = true;
            SaveButton.IsEnabled = true;
        }

        private void MicroserviceVersion_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsEditing = true;
            SaveButton.IsEnabled = true;
        }

        private void MicroserviceList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Display configuration settings on UI

            var index = MicroserviceList.SelectedIndex;
            if (index < 0) return;

            var name = ((Microservice) MicroserviceList.Items[MicroserviceList.SelectedIndex]).name;
            _curManifest = _manifests.list[index];

            MicroserviceId.Text = _curManifest.id;
            MicroserviceName.Text = _curManifest.name;
            MicroserviceDesc.Text = _curManifest.description;
            MicroserviceCacheTTL.Text = _curManifest.cachettl.ToString();
            MicroserviceModified.Text = _curManifest.modified.ToLongDateString();
            MicroserviceVersion.Text = _curManifest.version;

            // intitilaize the attribute editor

            AttributeEditor.Items.Clear();

            var attributes = _curManifest.lineitems.Select(s => new Attribute { name = s.key, value = s.val }).ToList();

            var emptyAttr = new Attribute { name = "<New>" };
            attributes.Add(emptyAttr);

            foreach (var a in attributes)
            {
                AttributeEditor.Items.Add(a);
            }

            IsNew = false;
            IsEditing = false;
        }

        private void Save()
        {
            var name = MicroserviceName.Text;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("ConfigM Manifest Name is required");
                IsEditing = false;
                IsNew = false;
                SaveButton.IsEnabled = false;
                return;
            }
            
            if (IsNew)
            {
                _newModel = new Manifest
                {
                    name = MicroserviceName.Text,
                    description = MicroserviceDesc.Text,
                    cachettl = Convert.ToInt32(MicroserviceCacheTTL.Text),
                    modified = DateTime.Now,
                    version = "1.0"
                };

                // Attributes
                var attributes = AttributeEditor.Items.Cast<Attribute>().ToList();
                foreach (var a in attributes.Where(a => a.name != "<New>"))
                {
                    _newModel.lineitems[a.name] = a.value;
                }

                _configM.Create(_newModel);

                SaveDialog saved = new SaveDialog();
                saved.Text = string.Format("{0} saved", _newModel.name);
                saved.ShowDialog();
            }
            else
            {
                var index = MicroserviceList.SelectedIndex;
                _curManifest = _manifests.list[index];
                _curManifest.name = MicroserviceName.Text;
                _curManifest.description = MicroserviceDesc.Text;
                _curManifest.cachettl = Convert.ToInt32(MicroserviceCacheTTL.Text);
                _curManifest.modified = DateTime.Now;
                _curManifest.version = MicroserviceVersion.Text;

                // Attributes
                var attributes = AttributeEditor.Items.Cast<Attribute>().ToList();
                foreach (var a in attributes.Where(a => a.name != "<New>"))
                {
                    _curManifest.lineitems[a.name] = a.value;
                }
                _configM.Update(_curManifest);

                SaveDialog saved = new SaveDialog();
                saved.Text = string.Format("{0} saved", _curManifest.name);
                saved.ShowDialog();
            }

            IsEditing = false;
            IsNew = false;
            SaveButton.IsEnabled = false;

            InitializeUx();
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            MicroserviceName.Text = string.Empty;
            MicroserviceDesc.Text = string.Empty;
            MicroserviceCacheTTL.Text = string.Empty;
            MicroserviceVersion.Text = string.Empty;
            AttributeEditor.Items.Clear();

            var attributes = new List<Attribute>();
            var emptyAttr = new Attribute {name = "<New>"};
            attributes.Add(emptyAttr);

            foreach (var a in attributes)
            {
                AttributeEditor.Items.Add(a);
            }
            MicroserviceName.Focus();
            IsNew = true;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void MicroserviceName_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Return) || (e.Key==Key.Tab))
            {
                IsEditing = true;
                SaveButton.IsEnabled = true;
            }
        }

        private void MicroserviceDesc_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Return) || (e.Key == Key.Tab))
            {
                IsEditing = true;
                SaveButton.IsEnabled = true;
            }
        }

        private void MicroserviceCacheTTL_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Return) || (e.Key == Key.Tab))
            {
                IsEditing = true;
                SaveButton.IsEnabled = true;
            }
        }

        private void MicroserviceVersion_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Return) || (e.Key == Key.Tab))
            {
                IsEditing = true;
                SaveButton.IsEnabled = true;
            }
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key != Key.Return) && (e.Key != Key.Tab)) return;
            IsEditing = true;
            SaveButton.IsEnabled = true;
            var t = (TextBox) sender;
            if (t.Name != "value") return;
            var emptyAttr = new Attribute { name = "<New>" };
            AttributeEditor.Items.Add(emptyAttr);
        }

        private void Name_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            IsEditing = true;
        }

        private void Value_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            IsEditing = true;
        }
    }
}
