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

            /*Title = Properties.Resources.formAboutTitle; // {x:Static res:Resources.formAboutTitle}*/

            butOK.Focus();
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
