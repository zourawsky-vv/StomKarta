using System;
using System.Collections.Generic;
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

namespace StomKarta
{
    /// <summary>
    /// Логика взаимодействия для FirstExam.xaml
    /// </summary>
    public partial class FirstExam : Window
    {
        public FirstExam(PacientInfo pacient)
        {
            InitializeComponent();

            datePicker.SelectedDate = DateTime.Today;
            comboBox01.Items.Add(Properties.Resources.firstExamCombo010);
            comboBox01.Items.Add(Properties.Resources.firstExamCombo011);
            comboBox02.Items.Add(Properties.Resources.firstExamCombo020);
            comboBox02.Items.Add(Properties.Resources.firstExamCombo021);
            comboBox03.Items.Add(Properties.Resources.firstExamCombo030);
            comboBox03.Items.Add(Properties.Resources.firstExamCombo031);
            comboBox04.Items.Add(Properties.Resources.firstExamCombo040);
            comboBox04.Items.Add(Properties.Resources.firstExamCombo041);
            comboBox05.Items.Add(Properties.Resources.firstExamCombo050);
            comboBox05.Items.Add(Properties.Resources.firstExamCombo051);
            comboBox06.Items.Add(Properties.Resources.firstExamCombo060);
            comboBox06.Items.Add(Properties.Resources.firstExamCombo061);
            comboBox07.Items.Add(Properties.Resources.firstExamCombo070);
            comboBox07.Items.Add(Properties.Resources.firstExamCombo071);
            comboBox08.Items.Add(Properties.Resources.firstExamCombo080);
            comboBox08.Items.Add(Properties.Resources.firstExamCombo081);
            comboBox09.Items.Add(Properties.Resources.firstExamCombo090);
            comboBox09.Items.Add(Properties.Resources.firstExamCombo091);
            comboBox10.Items.Add(Properties.Resources.firstExamCombo100);
            comboBox10.Items.Add(Properties.Resources.firstExamCombo101);
            comboBox11.Items.Add(Properties.Resources.firstExamCombo110);
            comboBox11.Items.Add(Properties.Resources.firstExamCombo111);
            comboBox12.Items.Add(Properties.Resources.firstExamCombo120);
            comboBox12.Items.Add(Properties.Resources.firstExamCombo121);
            comboBox12.Items.Add(Properties.Resources.firstExamCombo122);
            comboBox13.Items.Add(Properties.Resources.firstExamCombo130);
            comboBox13.Items.Add(Properties.Resources.firstExamCombo131);
            comboBox13.Items.Add(Properties.Resources.firstExamCombo132);
            comboBox13.Items.Add(Properties.Resources.firstExamCombo133);
            comboBox13.Items.Add(Properties.Resources.firstExamCombo134);
            comboBox13.Items.Add(Properties.Resources.firstExamCombo135);
            comboBox13.Items.Add(Properties.Resources.firstExamCombo136);
            comboBox13.Items.Add(Properties.Resources.firstExamCombo137);
            comboBox13.Items.Add(Properties.Resources.firstExamCombo138);
            comboBox14.Items.Add(Properties.Resources.firstExamCombo140);
            comboBox14.Items.Add(Properties.Resources.firstExamCombo141);
            comboBox15.Items.Add(Properties.Resources.firstExamCombo150);
            comboBox15.Items.Add(Properties.Resources.firstExamCombo151);
            comboBox15.Items.Add(Properties.Resources.firstExamCombo152);
            comboBox16.Items.Add(Properties.Resources.firstExamCombo160);
            comboBox16.Items.Add(Properties.Resources.firstExamCombo161);
            comboBox17.Items.Add(Properties.Resources.firstExamCombo170);
            comboBox17.Items.Add(Properties.Resources.firstExamCombo171);
            comboBox18.Items.Add(Properties.Resources.firstExamCombo180);
            comboBox18.Items.Add(Properties.Resources.firstExamCombo181);
            comboBox19.Items.Add("1");
            comboBox19.Items.Add("1,5");
            comboBox19.Items.Add("2");
            comboBox19.Items.Add("2,5");
            comboBox19.Items.Add("3");
            comboBox19.Items.Add("3,5");
            comboBox19.Items.Add("4");
            comboBox19.Items.Add("4,5");
            comboBox19.Items.Add("5");
            comboBox20.Items.Add(Properties.Resources.firstExamCombo200);
            comboBox20.Items.Add(Properties.Resources.firstExamCombo201);
            comboBox21.Items.Add(Properties.Resources.firstExamCombo200);
            comboBox21.Items.Add(Properties.Resources.firstExamCombo201);
            comboBox22.Items.Add(Properties.Resources.firstExamCombo220);
            comboBox22.Items.Add(Properties.Resources.firstExamCombo221);
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void buttonDentalRecord_Click(object sender, RoutedEventArgs e)
        {
            buttonSave.IsEnabled = true;
        }
    }
}
