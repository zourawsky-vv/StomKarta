using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;

namespace StomKarta
{
    /// <summary>
    /// Логика взаимодействия для PacientFind.xaml
    /// </summary>
    public partial class PacientFind : Window
    {
        DataTable table = null;
        PacientInfo pacientInfo;
        public PacientFind(PacientInfo pacient)
        {
            InitializeComponent();

            pacientInfo = pacient;
            readDB();
            dataGridPacFind.DataContext = table;
            ifEmptyTable();
            buildYouEnter(pacient);
        }
        private void buildYouEnter(PacientInfo pacient)
        {
            StringBuilder sbYouEnter = new StringBuilder(Properties.Resources.pacFindText0);
            if (!String.IsNullOrWhiteSpace(pacient.Fam))
            {
                sbYouEnter.Append(" " + pacient.Fam);
            }
            if (!String.IsNullOrWhiteSpace(pacient.Im))
            {
                sbYouEnter.Append(" " + pacient.Im);
            }
            if (pacient.DR != null)
            {
                sbYouEnter.Append(", " + ((DateTime)pacient.DR).ToString("dd.MM.yyyy"));
            }

            textBlockYouEnter.Text = sbYouEnter.ToString();
        }
        private void buttonPFYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void buttonPFNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void readDB()
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = MainWindow.connection;
            sqlCommand.Parameters.Add("@Im", SqlDbType.NVarChar, 25).Value = pacientInfo.Im;

            string selectCommand = "SELECT [Id],[Fam],[Im],[DR] FROM [dbo].[Pacient] WHERE [Im]=@Im";

            if (String.IsNullOrWhiteSpace(pacientInfo.Fam) && pacientInfo.DR == null)
            {
                sqlCommand.CommandText = selectCommand;
            }
            else if (String.IsNullOrWhiteSpace(pacientInfo.Fam))
            {
                sqlCommand.Parameters.Add("@DR", SqlDbType.DateTime).Value = pacientInfo.DR;

                sqlCommand.CommandText = selectCommand + " AND [DR]=@DR";
            }
            else if (pacientInfo.DR == null)
            {
                sqlCommand.Parameters.Add("@Fam", SqlDbType.NVarChar, 35).Value = pacientInfo.Fam;

                sqlCommand.CommandText = selectCommand + " AND [Fam]=@Fam";
            }
            else
            {
                sqlCommand.Parameters.Add("@Fam", SqlDbType.NVarChar, 35).Value = pacientInfo.Fam;
                sqlCommand.Parameters.Add("@DR", SqlDbType.DateTime).Value = pacientInfo.DR;

                sqlCommand.CommandText = selectCommand + " AND [Fam]=@Fam AND [DR]=@DR";
            }

            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);

            DataSet dataSet = new DataSet();

            try
            {
                dataAdapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    Properties.Resources.messageCaptionError,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
            }

            table = dataSet.Tables[0];
        }
        private void ifEmptyTable()
        {
            if (table.Rows.Count == 0)
            {
                textBlock1.Text = Properties.Resources.pacFindText10;
                textBlock2.Visibility = Visibility.Collapsed;
                textBlock3.Visibility = Visibility.Collapsed;

                buttonPFYes.Focus();
            }
            else
                dataGridPacFind.Focus();
        }
    }
}
