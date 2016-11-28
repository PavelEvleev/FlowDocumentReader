using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace FlowDocumentsReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string folder = null;
        public ObservableCollection<XamlFile> listFile { get; set; } = new ObservableCollection<XamlFile>();
        public MainWindow()
        {
            InitializeComponent();
            ListViewDocuments.ItemsSource = listFile;
        }

        private void Set_Directory_Click(object sender, RoutedEventArgs e)
        {
            listFile.Clear();
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if(System.Windows.Forms.DialogResult.OK== dlg.ShowDialog())
            {
                folder = dlg.SelectedPath;
                string[] files = System.IO.Directory.GetFiles(folder);
                foreach(string file in files)
                {
                    listFile.Add(new XamlFile(file));
                }
            }

        }

        private void ListViewDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            XamlFile selected = ListViewDocuments.SelectedItem as XamlFile;

            if (selected.Valid != false)
            {
                Viewer.Visibility = Visibility.Visible;
                using (FileStream file = new FileStream(selected.PathFile, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        Section content = XamlReader.Load(file) as Section;
                        FlowDocument doc = new FlowDocument(content);
                        Viewer.Document = doc;
                    }
                    catch (XamlParseException ex)
                    {
                        selected.Valid = false;
                        Viewer.Visibility = Visibility.Hidden;
                    }

                }
            }
            else { Viewer.Visibility = Visibility.Hidden; }

        }
    }


    public class XamlFile:INotifyPropertyChanged
    {
        private bool isValid = true;
        public string PathFile { get; private set; }
        public bool Valid { get {
                return isValid;
            } set {
                isValid = value;
                RiseChageProperyEvent();
            }
        }
        public string Name { get; set;}

        public XamlFile(string path)
        {
            PathFile = path;
            Valid = true;
            Name= System.IO.Path.GetFileName(path);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RiseChageProperyEvent()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Valid"));
        }


    }
}
