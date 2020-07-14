using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StomKarta
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte idUser;
        string userName;
        bool isAdmin;

        bool isTablePrivateUserInfoRowExist = false;
        int tablePrivateUserInfoRow = 0;
        short privateUserInfoRow = 0;

        List<int> tableTelefonsRowsIndex = new List<int>(4);
        List<short> telefonsRowsId = new List<short>(4);

        internal static SqlConnection connection = null;
        DataTable tablePacients = null;
        DataTable tableTelefons = null;
        DataTable tablePrivateUserInfo = null;

        internal static string labelNickNameText;
        public MainWindow()
        {
            InitializeComponent();

            Activated += MainWindow_Activated;

            menuItemReestrPacient.IsEnabled = false;
            menuItemVizit.IsEnabled = false;
            menuItemUser.IsEnabled = false;
            menuItemConfiguration.IsEnabled = false;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            Activated -= MainWindow_Activated;

            showAbout();
            showLogon();

            if (userName != null)
            {
                if (isAdmin) // Role - Admin
                {
                    menuItemUser.IsEnabled = true;
                    menuItemConfiguration.IsEnabled = true;

                    ShowUsers();
                }
                else // Role - User
                {
                    menuItemReestrPacient.IsEnabled = true;
                    menuItemVizit.IsEnabled = true;
                    menuItemUser.IsEnabled = true;
                    menuItemConfiguration.IsEnabled = true;

                    statusBarItemBase.Text = Properties.Resources.baseReestr;
                    statusBarItemMode.Text = Properties.Resources.modeBrowse;

                    ReadDB();

                    dataGridPacients.DataContext = tablePacients;

                    fillPanel();

                    mainWindowLayout();

                    dataGridPacients.Focus();
                }
            }
        }
        private void fillPanel()
        {
            // tablePacients
            int colID = 0; /*[patient Id]*/
            int colDR = 5; /*[Дата рождения]*/
            int colEM = 6; /*[Эл. Почта]*/
            int colAd = 7; /*[Адрес]*/
            int colComm = 8; /*[Примечание]*/

            // tablePrivateUserInfo - [Id],[PacientId],[UserId],[NickName],[FromComment]
            int colNN = 3; /*NickName*/
            int colFC = 4; /*FromComment*/

            isTablePrivateUserInfoRowExist = false;
            tablePrivateUserInfoRow = 0;
            privateUserInfoRow = 0;

            for (int i = 0; i < tablePrivateUserInfo.Rows.Count; i++)
            {
                if ((short)tablePrivateUserInfo.Rows[i]["PacientId"] == (short)tablePacients.Rows[dataGridPacients.SelectedIndex][colID]
                    && (byte)tablePrivateUserInfo.Rows[i]["UserId"] == idUser)
                {
                    isTablePrivateUserInfoRowExist = true;
                    tablePrivateUserInfoRow = i;
                    privateUserInfoRow = (short)tablePrivateUserInfo.Rows[i]["Id"];
                    break;
                }
            }

            if (tablePrivateUserInfo.Rows.Count > 0 && isTablePrivateUserInfoRowExist &&
                !String.IsNullOrWhiteSpace(tablePrivateUserInfo.Rows[tablePrivateUserInfoRow][colNN].ToString()))
            {
                textBlockNickName.Text = tablePrivateUserInfo.Rows[tablePrivateUserInfoRow][colNN].ToString();
            }
            else
                textBlockNickName.Text = String.Empty;

            if (tablePacients.Rows[dataGridPacients.SelectedIndex][colDR] != null)
            {
                TimeSpan interval = DateTime.Today.Subtract((DateTime)tablePacients.Rows[dataGridPacients.SelectedIndex][colDR]);
                textBlockAge.Text = ((int)((double)interval.Days / 365.25)).ToString();
            }
            else
                textBlockAge.Text = String.Empty;

            listBoxTel.Items.Clear();
            tableTelefonsRowsIndex.Clear();
            telefonsRowsId.Clear();
            for (int i = 0; i < tableTelefons.Rows.Count; i++)
            {
                if ((short)tableTelefons.Rows[i]["PacientId"] == (short)tablePacients.Rows[dataGridPacients.SelectedIndex][colID])
                {
                    listBoxTel.Items.Add(tableTelefons.Rows[i]["Number"].ToString());
                    tableTelefonsRowsIndex.Add(i);
                    telefonsRowsId.Add((short)tableTelefons.Rows[i]["Id"]);
                }
            }

            if (!String.IsNullOrWhiteSpace(tablePacients.Rows[dataGridPacients.SelectedIndex][colEM].ToString()))
                textBoxEMail.Text = tablePacients.Rows[dataGridPacients.SelectedIndex][colEM].ToString();
            else
                textBoxEMail.Text = String.Empty;

            if (!String.IsNullOrWhiteSpace(tablePacients.Rows[dataGridPacients.SelectedIndex][colAd].ToString()))
                textBlockAdress.Text = tablePacients.Rows[dataGridPacients.SelectedIndex][colAd].ToString();
            else
                textBlockAdress.Text = String.Empty;

            if (tablePrivateUserInfo.Rows.Count > 0 && isTablePrivateUserInfoRowExist &&
                !String.IsNullOrWhiteSpace(tablePrivateUserInfo.Rows[tablePrivateUserInfoRow][colFC].ToString()))
            {
                textBlockFromId.Text = tablePrivateUserInfo.Rows[tablePrivateUserInfoRow][colFC].ToString();
            }
            else
                textBlockFromId.Text = String.Empty;

            if (!String.IsNullOrWhiteSpace(tablePacients.Rows[dataGridPacients.SelectedIndex][colComm].ToString()))
                textBlockComment.Text = tablePacients.Rows[dataGridPacients.SelectedIndex][colComm].ToString();
            else
                textBlockComment.Text = String.Empty;
        }
        private void ReadDB()
        {
            connection = new SqlConnection();
            connection.ConnectionString = Properties.Settings.Default.StomBaseConnectionString;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;

            sqlCommand.Parameters.Add("@idUser", SqlDbType.TinyInt).Value = idUser;
                
            sqlCommand.CommandText = "SELECT [Id],[Fam],[Im],[Otc],[LName],[DR],[Mail],[Adress],[Comment],[RowUpdate],[UserName]" + 
                                        " FROM [StomBase].[dbo].[Pacients]" + // Представление
                                        "SELECT [Id],[PacientId],[Number] FROM [StomBase].[dbo].[Telefon];" +
                                        "SELECT [Id],[PacientId],[UserId],[NickName],[FromComment] " +
                                        "FROM [StomBase].[dbo].[PrivateUserInfo] " + 
                                        "WHERE [UserId]=@idUser";

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
                Application.Current.Shutdown();
            }

            tablePacients = dataSet.Tables[0];
            tableTelefons = dataSet.Tables[1];
            tablePrivateUserInfo = dataSet.Tables[2];
        }
        private void mainWindowLayout()
        {
            double shift = 20.0; /* unknown reason, measured empiricaly */
            double titleHeight = 22.0;

            mainStatusBar.Width =  mainMenu.Width = this.ActualWidth 
                - (mainWindow.BorderThickness.Left + mainWindow.BorderThickness.Right) - shift;

            //borderCommandButtons.Width = widthMain 
            //- (mainWindow.BorderThickness.Left + mainWindow.BorderThickness.Right) - shift;

            dataGridPacients.MaxHeight = panelOtherFields.MaxHeight = this.ActualHeight
                - (mainWindow.BorderThickness.Top + mainWindow.BorderThickness.Bottom)
                - titleHeight
                - mainMenu.ActualHeight - mainStatusBar.ActualHeight - borderCommandButtons.ActualHeight;

            borderCommentHeight();
        }
        private void borderCommentHeight()
        {
            borderComment.Height = panelOtherFields.Height
                - labelAge.Height
                - labelTelefons.Height - textBoxEMail.Height * 4 /*emulate 3-row dataGridTelefons.Height*/
                - labelEMail.Height - textBoxEMail.Height
                - labelAdress.Height - borderAdress.Height
                - labelFromId.Height - borderFromId.Height
                - labelComment.Height;
        }
        private void showAbout()
        {
            About about = new About();
            _ = about.ShowDialog();
        }

        private void showLogon()
        {
            string currentBase = statusBarItemBase.Text;
            statusBarItemBase.Text = Properties.Resources.baseUser;

            string currentBaseMode = statusBarItemMode.Text;
            statusBarItemMode.Text = Properties.Resources.modeBrowse;

            Logon logon = new Logon();
            _ = logon.ShowDialog();

            idUser = logon.idUser;
            userName = logon.userName;
            isAdmin = logon.isAdminUser;
            labelNickNameText = logon.aliasFieldHeader;

            if (logon.aliasFieldHeader != null)
                labelNickName.Text = logon.aliasFieldHeader;

            statusBarItemBase.Text = currentBase;
            statusBarItemMode.Text = currentBaseMode;

            if(userName != null)
                statusBarItemUser.Text = userName;
        }

        private void ShowUsers()
        {
            // ShowUsers()
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuItemReestrPacient_Click(object sender, RoutedEventArgs e)
        {
            dataGridPacients.Focus();
        }

        private void menuItemVizitProfilactic_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemVizitTherapist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemVizitSurgeon_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemVizitOrthopedist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemUserAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemUserList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemHelpView_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            showAbout();
        }

        private void dataGridPacients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridPacients.DataContext != null)
            {
                fillPanel();
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            statusBarItemMode.Text = Properties.Resources.modeAdd;

            PacientInfo pacient = new PacientInfo(idUser);

            Pacient pacientForm = new Pacient(pacient);

            if ((bool)pacientForm.ShowDialog())
            {
                PacientFind pacientFind = new PacientFind(pacient);

                if ((bool)pacientFind.ShowDialog()) // adding new pacient
                {
                    insertPacient(pacient);

                    if (!String.IsNullOrWhiteSpace(pacient.NickName) || !String.IsNullOrWhiteSpace(pacient.FromComment))
                        insertPrivateUserInfo(pacient);

                    if (pacient.Telefons.Count != 0)
                        insertTelefon(pacient);

                    dataGridPacients.DataContext = null;
                    ReadDB();
                    dataGridPacients.DataContext = tablePacients;
                    dataGridPacients.SelectedIndex = dataGridPacients.Items.Count - 1;
                    fillPanel();
                    borderCommentHeight();
                    dataGridPacients.Focus();
                }
            }
            statusBarItemMode.Text = Properties.Resources.modeBrowse;
        }
        private void insertPacient(PacientInfo pacient)
        {
            // INSERT INTO [dbo].[Pacient]
            // ([Fam],[Im],[Otc],[SexId],[DR],[Mail],[Adress],[Comment],[RowUpdate],[UserId])
            //  VALUES(<Fam, nvarchar(35),>,<Im, nvarchar(25),>,<Otc, nvarchar(25),>,<SexId, tinyint,>,<DR, date,>,
            //<Mail, varchar50),>,<Adress, nvarchar(100),>,<Comment, nvarchar(max),>,<RowUpdate, datetime,>,<UserId, tinyint,>)

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;

            sqlCommand.Parameters.Add(newParameter("@Fam", SqlDbType.NVarChar, 35, pacient.Fam));
            sqlCommand.Parameters.Add("@Im", SqlDbType.NVarChar, 25).Value = pacient.Im; //!NULL
            sqlCommand.Parameters.Add(newParameter("@Otc", SqlDbType.NVarChar, 25, pacient.Otc));
            sqlCommand.Parameters.Add("@SexId", SqlDbType.TinyInt).Value = pacient.SexId; //!NULL
            sqlCommand.Parameters.Add(newParameter("@DR", SqlDbType.DateTime, 0, pacient.DR));
            sqlCommand.Parameters.Add(newParameter("@Mail", SqlDbType.VarChar, 50, pacient.Mail));
            sqlCommand.Parameters.Add(newParameter("@Adress", SqlDbType.NVarChar, 100, pacient.Adress));
            sqlCommand.Parameters.Add(newParameter("@Comment", SqlDbType.NVarChar, 0, pacient.Comment));
            sqlCommand.Parameters.Add("@RowUpdate", SqlDbType.DateTime).Value = pacient.RowUpdate; //!NULL
            sqlCommand.Parameters.Add("@UserId", SqlDbType.TinyInt).Value = pacient.UserId; //!NULL
            sqlCommand.CommandText = "INSERT INTO [dbo].[Pacient]" +
                " ([Fam],[Im],[Otc],[SexId],[DR],[Mail],[Adress],[Comment],[RowUpdate],[UserId])" +
                " VALUES(@Fam, @Im, @Otc, @SexId, @DR, @Mail, @Adress, @Comment, @RowUpdate, @UserId);" +
                "SELECT CAST(SCOPE_IDENTITY() AS smallint)";
            try
            {
                sqlCommand.Connection.Open();
                pacient.Id = (short)sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    Properties.Resources.messageCaptionError,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
                Application.Current.Shutdown();
            }
            finally
            {
                if (sqlCommand.Connection.State == ConnectionState.Open)
                {
                    sqlCommand.Connection.Close();
                }
            }
        }
        private void insertPrivateUserInfo(PacientInfo pacient)
        {
            //INSERT INTO [dbo].[PrivateUserInfo] ([PacientId],[UserId],[NickName],[FromComment])
            // VALUES(< PacientId, smallint,>,< UserId, tinyint,>,< NickName, nvarchar(25),>,< FromComment, nvarchar(100),>)

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;

            sqlCommand.Parameters.Add("@PacientId", SqlDbType.SmallInt).Value = pacient.Id; //!NULL
            sqlCommand.Parameters.Add("@UserId", SqlDbType.TinyInt).Value = pacient.UserId; //!NULL
            sqlCommand.Parameters.Add(newParameter("@NickName", SqlDbType.NVarChar, 25, pacient.NickName));
            sqlCommand.Parameters.Add(newParameter("@FromComment", SqlDbType.NVarChar, 100, pacient.FromComment));
            sqlCommand.CommandText = "INSERT INTO [dbo].[PrivateUserInfo]" +
                " ([PacientId],[UserId],[NickName],[FromComment])" +
                " VALUES(@PacientId, @UserId, @NickName, @FromComment)";

            sqlCommandExecuteNonQuery(sqlCommand);
        }
        private void insertTelefon(PacientInfo pacient)
        {
            for (int i = 0; i < pacient.Telefons.Count; i++)
            {
                //INSERT INTO [dbo].[Telefon] ([PacientId],[Number])
                // VALUES(< PacientId, smallint,>,< Number, char(18),>)
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.Parameters.Add("@PacientId", SqlDbType.SmallInt).Value = pacient.Id;
                sqlCommand.Parameters.Add("@Number", SqlDbType.Char, 18).Value = pacient.Telefons[i];
                sqlCommand.CommandText = "INSERT INTO [dbo].[Telefon] ([PacientId],[Number])" +
                    "VALUES(@PacientId, @Number)";
                
                sqlCommandExecuteNonQuery(sqlCommand);
            }
        }
        private SqlParameter newParameter(string parameterName, SqlDbType type, int size, object parameterObject)
        {
            SqlParameter parameter = new SqlParameter(parameterName, type);
            parameter.IsNullable = true;

            if (parameterObject == null)
                parameter.Value = DBNull.Value;
            else
                parameter.Value = parameterObject;

            if (size != 0)
                parameter.Size = size;

            return parameter;
        }
        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainWindowLayout();
        }
        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            statusBarItemMode.Text = Properties.Resources.modeEdit;

            PacientInfo pacient = new PacientInfo(idUser);

            initPacient(pacient);

            int countBefore = pacient.Telefons.Count;

            Pacient pacientForm = new Pacient(pacient);

            if ((bool)pacientForm.ShowDialog())
            {
                // some fields of pacient was changed
                updatePacient(pacient);

                if (isTablePrivateUserInfoRowExist)
                {
                    if (!String.IsNullOrWhiteSpace(pacient.NickName) || !String.IsNullOrWhiteSpace(pacient.FromComment)) //new
                    {
                        updatePrivateUserInfo(pacient);
                    }
                    else
                    {
                        deletePrivateUserInfoRow(privateUserInfoRow);
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(pacient.NickName) || !String.IsNullOrWhiteSpace(pacient.FromComment)) //new
                    {
                        insertPrivateUserInfo(pacient);
                    }
                }

                if (countBefore == 0)
                {
                    if (pacient.Telefons.Count != 0)
                    {
                        insertTelefon(pacient);
                    }
                }
                else // countBefore != 0
                {
                    if (pacient.Telefons.Count == 0)
                    {
                        deleteTelefonsAll(countBefore); //delete all
                    }
                    else
                    {
                        if (countBefore == pacient.Telefons.Count)
                        {
                            updateTelefon(pacient);
                        }
                        else
                        {
                            deleteTelefonsAll(countBefore);
                            insertTelefon(pacient);
                        }
                    }
                }
                

                int selectedIndex = dataGridPacients.SelectedIndex;
                dataGridPacients.DataContext = null;
                ReadDB();
                dataGridPacients.DataContext = tablePacients;
                dataGridPacients.SelectedIndex = selectedIndex;
                fillPanel();
                borderCommentHeight();
                dataGridPacients.Focus();
            }

            statusBarItemMode.Text = Properties.Resources.modeBrowse;
        }
        private void initPacient(PacientInfo pacient)
        {
            /*"SELECT [Id],[Fam],[Im],[Otc],[LName],[DR],[Mail],[Adress],[Comment],[RowUpdate],[UserName]" +
              " FROM [StomBase].[dbo].[Pacients]" + // Представление

              "SELECT [Id],[PacientId],[Number] FROM [StomBase].[dbo].[Telefon];" +

              "SELECT [Id],[PacientId],[UserId],[NickName],[FromComment]" +
              " FROM [StomBase].[dbo].[PrivateUserInfo] " +

              "WHERE [UserId]=@idUser"*/

            pacient.Id = (short)tablePacients.Rows[dataGridPacients.SelectedIndex][0];
            pacient.Fam = (string)tablePacients.Rows[dataGridPacients.SelectedIndex][1];
            pacient.Im = (string)tablePacients.Rows[dataGridPacients.SelectedIndex][2];
            pacient.Otc = (string)tablePacients.Rows[dataGridPacients.SelectedIndex][3];

            if ((string)tablePacients.Rows[dataGridPacients.SelectedIndex][4] == "мужской")
                pacient.SexId = 0;
            else // "женский"
                pacient.SexId = 1;

            pacient.DR = (DateTime?)tablePacients.Rows[dataGridPacients.SelectedIndex][5];
            pacient.Mail = (string)tablePacients.Rows[dataGridPacients.SelectedIndex][6];
            pacient.Adress = (string)tablePacients.Rows[dataGridPacients.SelectedIndex][7];
            pacient.Comment = (string)tablePacients.Rows[dataGridPacients.SelectedIndex][8];
            pacient.RowUpdate = (DateTime)tablePacients.Rows[dataGridPacients.SelectedIndex][9];
            //pacient.UserId initialized in constructor() by current idUser

            if (isTablePrivateUserInfoRowExist)
            {
                pacient.NickName = (string)tablePrivateUserInfo.Rows[tablePrivateUserInfoRow][3];
                pacient.FromComment = (string)tablePrivateUserInfo.Rows[tablePrivateUserInfoRow][4];
            }

            for (int i = 0; i < tableTelefonsRowsIndex.Count; i++)
            {
                pacient.Telefons.Add((string)tableTelefons.Rows[tableTelefonsRowsIndex[i]]["Number"]);
            }
            pacient.Telefons.TrimExcess();
        }
        private void updatePacient(PacientInfo pacient)
        {
            // UPDATE [StomBase].[dbo].[Pacient]
            //  SET [Fam] = <Fam, nvarchar(35),>,[Im] = <Im, nvarchar(25),>,[Otc] = <Otc, nvarchar(25),>,
            //      [SexId] = <SexId, tinyint,>,[DR] = <DR, date,>,[Mail] = <Mail, varchar(50),>,
            //      [Adress] = <Adress, nvarchar(100),>,[Comment] = <Comment, nvarchar(max),>,
            //      [RowUpdate] = <RowUpdate, datetime,>,[UserId] = <UserId, tinyint,>
            //  WHERE <Условия поиска,,>

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;

            sqlCommand.Parameters.Add("@Id", SqlDbType.SmallInt).Value = pacient.Id;
            sqlCommand.Parameters.Add(newParameter("@Fam", SqlDbType.NVarChar, 35, pacient.Fam));
            sqlCommand.Parameters.Add("@Im", SqlDbType.NVarChar, 25).Value = pacient.Im; //!NULL
            sqlCommand.Parameters.Add(newParameter("@Otc", SqlDbType.NVarChar, 25, pacient.Otc));
            sqlCommand.Parameters.Add("@SexId", SqlDbType.TinyInt).Value = pacient.SexId; //!NULL
            sqlCommand.Parameters.Add(newParameter("@DR", SqlDbType.DateTime, 0, pacient.DR));
            sqlCommand.Parameters.Add(newParameter("@Mail", SqlDbType.VarChar, 50, pacient.Mail));
            sqlCommand.Parameters.Add(newParameter("@Adress", SqlDbType.NVarChar, 100, pacient.Adress));
            sqlCommand.Parameters.Add(newParameter("@Comment", SqlDbType.NVarChar, 0, pacient.Comment));
            sqlCommand.Parameters.Add("@RowUpdate", SqlDbType.DateTime).Value = pacient.RowUpdate; //!NULL
            sqlCommand.Parameters.Add("@UserId", SqlDbType.TinyInt).Value = pacient.UserId; //!NULL
            sqlCommand.CommandText = "UPDATE [StomBase].[dbo].[Pacient] " +
                "SET [Fam] = @Fam, [Im] = @Im, [Otc] = @Otc, [SexId] = @SexId, [DR] = @DR, [Mail] = @Mail, " +
                "[Adress] = @Adress, [Comment] = @Comment, [RowUpdate] = @RowUpdate, [UserId] = @UserId " +
                "WHERE [Id] = @Id";

            sqlCommandExecuteNonQuery(sqlCommand);
        }
        private void updatePrivateUserInfo(PacientInfo pacient)
        {
            // UPDATE [StomBase].[dbo].[PrivateUserInfo]
            //  SET [PacientId] = <PacientId, smallint,>, [UserId] = <UserId, tinyint,>, [NickName] = <NickName, nvarchar(25),>
            //    , [FromComment] = <FromComment, nvarchar(100),>
            //  WHERE <Условия поиска,,>

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;

            sqlCommand.Parameters.Add("@Id", SqlDbType.SmallInt).Value = privateUserInfoRow;
            sqlCommand.Parameters.Add("@PacientId", SqlDbType.SmallInt).Value = pacient.Id; //!NULL
            sqlCommand.Parameters.Add("@UserId", SqlDbType.TinyInt).Value = pacient.UserId; //!NULL
            sqlCommand.Parameters.Add(newParameter("@NickName", SqlDbType.NVarChar, 25, pacient.NickName));
            sqlCommand.Parameters.Add(newParameter("@FromComment", SqlDbType.NVarChar, 100, pacient.FromComment));
            sqlCommand.CommandText = "UPDATE [StomBase].[dbo].[PrivateUserInfo]" + 
                " SET [PacientId] = @PacientId, [UserId] = @UserId, [NickName] = @NickName, [FromComment] = @FromComment" + 
                " WHERE [Id] = @Id";

            sqlCommandExecuteNonQuery(sqlCommand);
        }
        private void updateTelefon(PacientInfo pacient)
        {
            /* UPDATE [StomBase].[dbo].[Telefon]
            *  SET [PacientId] = <PacientId, smallint,> ,[Number] = <Number, char(18),>
            *  WHERE <Условия поиска,,>
            */

            for (int i = 0; i < pacient.Telefons.Count; i++)
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;

                sqlCommand.Parameters.Add("@Id", SqlDbType.SmallInt).Value = telefonsRowsId[i];
                sqlCommand.Parameters.Add("@PacientId", SqlDbType.SmallInt).Value = pacient.Id; //!NULL
                sqlCommand.Parameters.Add("@Number", SqlDbType.Char, 18).Value = pacient.Telefons[i];
                sqlCommand.CommandText = "UPDATE [StomBase].[dbo].[Telefon]" + 
                    " SET [PacientId] = @PacientId, [Number] = @Number" + 
                    " WHERE [Id] = @Id";
                
                sqlCommandExecuteNonQuery(sqlCommand);
            }
        }
        private void sqlCommandExecuteNonQuery(SqlCommand sqlCommand)
        {
            try
            {
                sqlCommand.Connection.Open();
                _ = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    Properties.Resources.messageCaptionError,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
                Application.Current.Shutdown();
            }
            finally
            {
                if (sqlCommand.Connection.State == ConnectionState.Open)
                {
                    sqlCommand.Connection.Close();
                }
            }
        }
        private void deletePrivateUserInfoRow(short row)
        {
            // DELETE FROM [StomBase].[dbo].[PrivateUserInfo] WHERE <Условия поиска,,>

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;

            sqlCommand.Parameters.Add("@Id", SqlDbType.SmallInt).Value = row;
            sqlCommand.CommandText = "DELETE FROM [StomBase].[dbo].[PrivateUserInfo] WHERE [Id] = @Id";

            sqlCommandExecuteNonQuery(sqlCommand);
        }
        private void deleteTelefonsAll(int count)
        {
            // DELETE FROM [StomBase].[dbo].[Telefon] WHERE <Условия поиска,,>

            for (int i = 0; i < count; i++)
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;

                sqlCommand.Parameters.Add("@Id", SqlDbType.SmallInt).Value = telefonsRowsId[i];
                sqlCommand.CommandText = "DELETE FROM [StomBase].[dbo].[Telefon] WHERE [Id] = @Id";

                sqlCommandExecuteNonQuery(sqlCommand);
            }
        }
        private void buttonAppointment_Click(object sender, RoutedEventArgs e)
        {
            string currentBase = statusBarItemBase.Text;
            statusBarItemBase.Text = Properties.Resources.baseTherapy;

            PacientInfo pacient = new PacientInfo(idUser);
            pacient.Id = (short)tablePacients.Rows[dataGridPacients.SelectedIndex][0];

            FirstExam firstExam = new FirstExam(pacient);

            if ((bool)firstExam.ShowDialog())
            {
                ;// if Save
            }

            statusBarItemBase.Text = currentBase;
        }
    }
}
