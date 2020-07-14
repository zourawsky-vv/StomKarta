using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace StomKarta
{
    /// <summary>
    /// Логика взаимодействия для Logon.xaml
    /// </summary>
    public partial class Logon : Window
    {
        SqlDataAdapter dataAdapter = null;
        DataSet dataSet = null;

        public byte idUser;
        public string userName;
        public bool isAdminUser;
        public string aliasFieldHeader;

        string passWord;
        int selUserRow = 0; // row in DB
        public Logon()
        {
            InitializeComponent();

            confirmPass.IsEnabled = false;
            textBoxConfirmPass.IsEnabled = false;
            
            ReadDbUser(); // load DB to dataSet

            FillCombo(); //  fill ComboBox from User.UserName

            comboUsers.Focus();

            // Button_Click()
        }
        private void ReadDbUser()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = Properties.Settings.Default.StomBaseConnectionString;

            try
            {
                dataAdapter = new SqlDataAdapter("SELECT [Id],[UserName],[PassWord],[IsDbAdmin],[AliasFieldHeader] FROM [StomBase].[dbo].[User]", connection);

                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                dataSet = new DataSet();

                dataAdapter.Fill(dataSet); // dataAdapter.Fill(dataSet) for load DB, dataAdapter.Update(dataSet) for save changes
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
        }
        private void FillCombo()
        {
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                comboUsers.Items.Add(dataSet.Tables[0].Rows[i]["UserName"].ToString());
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (comboUsers.SelectedItem != null && !String.IsNullOrEmpty(textBoxPassword.Password)) //selUser entPass
            {
                ChooseCurrentUserData();

                if (String.IsNullOrWhiteSpace(passWord)) // pass in DB empty
                {
                    if (SavePasswordInDb()) // Pass saved
                    {
                        DialogResult = true;
                        Close();
                    }
                    return;
                }

                if (passWord == textBoxPassword.Password.GetHashCode().ToString()) // pass equals to DB pass
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show(Properties.Resources.logonMessageWrongPassword,
                                    Properties.Resources.messageCaptionError,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error,
                                    MessageBoxResult.OK,
                                    MessageBoxOptions.DefaultDesktopOnly);

                    textBoxPassword.Password = String.Empty;
                }
            }
        }
        private void ChooseCurrentUserData()
        {
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                if ((string)dataSet.Tables[0].Rows[i]["UserName"] == (string)comboUsers.SelectedItem) //for selUser inf from DB
                {
                    idUser = (byte)dataSet.Tables[0].Rows[i]["Id"];
                    userName = (string)dataSet.Tables[0].Rows[i]["UserName"];
                    isAdminUser = (bool)dataSet.Tables[0].Rows[i]["IsDbAdmin"];
                    aliasFieldHeader = (string)dataSet.Tables[0].Rows[i]["AliasFieldHeader"];

                    passWord = (string)dataSet.Tables[0].Rows[i]["PassWord"];
                    selUserRow = i;
                    break;
                }
            }
        }
        private bool SavePasswordInDb()
        {
            if (String.IsNullOrWhiteSpace(textBoxConfirmPass.Password)) // if field Confirm disabled, open, ask to enter
            {
                if (!confirmPass.IsEnabled) // open confirm fields
                {
                    confirmPass.IsEnabled = true;
                    textBoxConfirmPass.IsEnabled = true;
                }

                MessageBox.Show(Properties.Resources.logonMessageConfirm,
                                Properties.Resources.messageCaptionInformation,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information,
                                MessageBoxResult.OK,
                                MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            else // pass in DB empty AND have Confirm
            {
                if (textBoxPassword.Password == textBoxConfirmPass.Password)
                {
                    dataSet.Tables[0].Rows[selUserRow]["PassWord"] = textBoxPassword.Password.GetHashCode().ToString();
                    dataAdapter.Update(dataSet);

                    return true;
                }
                else
                {
                    MessageBox.Show(Properties.Resources.logonMessageConfirmError,
                                    Properties.Resources.messageCaptionError,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error,
                                    MessageBoxResult.OK,
                                    MessageBoxOptions.DefaultDesktopOnly);
                    
                    textBoxConfirmPass.Password = String.Empty; // erase wrong confirm
                    
                    return false;
                }
            }
        }
        private void comboUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (confirmPass.IsEnabled)
            {
                confirmPass.IsEnabled = false;
                textBoxConfirmPass.IsEnabled = false;
            }

            textBoxPassword.Focus();
        }
    }
}
