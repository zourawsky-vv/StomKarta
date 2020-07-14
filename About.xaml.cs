using System;
using System.Reflection;
using System.Windows;

namespace StomKarta
{
    /// <summary>
    /// Логика взаимодействия для About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            Version strVersion = Assembly.GetExecutingAssembly().GetName().Version;

            version.Text = Properties.Resources.aboutVersion + " " + strVersion;

            butOK.Focus();
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
