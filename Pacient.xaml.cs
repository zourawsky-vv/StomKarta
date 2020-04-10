using System;
using System.Windows;
using System.Windows.Input;

namespace StomKarta
{
    /// <summary>
    /// Логика взаимодействия для Pacient.xaml
    /// </summary>
    public partial class Pacient : Window
    {
        PacientInfo pacientInfo;

        char[] ruChars = new char[] { 'й', 'ц', 'у', 'к', 'е', 'н', 'г', 'ш', 'щ', 'з', 'ф', 'ы', 'в', 'а', 'п', 'р', 'о', 'л', 'д', 'я', 'ч', 'с', 'м', 'и', 'т', 'ь', '"', 'ю' };
        char[] enChars = new char[] { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm', '@', '.' };
        public Pacient(PacientInfo pacient)
        {
            InitializeComponent();

            /*Title = Properties.Resources.formPacientAddTitle;*/
            /*buttonAPAdd.Content = Properties.Resources.pacientButtonSave;*/
            /*buttonAPExit.Content = Properties.Resources.pacientButtonExit;*/
            /*labelFam.Content = Properties.Resources.pacientLabelFam;*/
            /*labelIm.Content = Properties.Resources.pacientLabelIm;*/
            /*labelOt.Content = Properties.Resources.pacientLabelOtc;*/
            /*labelSex.Content = Properties.Resources.pacientLabelSex;*/
            /*labelDR.Content = Properties.Resources.pacientLabelDR;*/
            /*labelEMail.Content = Properties.Resources.pacientLabelEMail;*/
            /*labelAddress.Content = Properties.Resources.pacientLabelAddress;*/
            /*labelComment.Content = Properties.Resources.pacientLabelComment;*/
            /*labelTelefons.Content = Properties.Resources.pacientLabelTelefons;*/
            /*labelNumber.Content = Properties.Resources.pacientLabelNumber;*/
            /*buttonTelAdd.Content = Properties.Resources.pacientButtonAddTel;*/
            /*buttonTelDel.Content = Properties.Resources.pacientButtonDelTel;*/
            /*labelPrivateUserInfo.Content = Properties.Resources.pacientLabelPrivateUserInfo;*/
            /*labelNickName.Content = Properties.Resources.pacientLabelNickName;*/
            /*labelFromComment.Content = Properties.Resources.pacientLabelFromComment;*/
            /*labelTelFormat.Text = Properties.Resources.pacientLabelTelFormat;*/

            labelNickName.Content = MainWindow.labelNickNameText;

            pacientInfo = pacient;

            FillCombo(); // Sex

            if (pacientInfo.RowUpdate != new DateTime()) // {edit mode} else {add mode - fields empty}
            {
                Title = Properties.Resources.formPacientEditTitle;
                initTextFields();
            }

            textBoxFam.Focus();
        }
        private void FillCombo()
        {
            comboBoxSex.Items.Add("мужской");
            comboBoxSex.Items.Add("женский");
        }
        private void initTextFields()
        {
            if (!String.IsNullOrWhiteSpace(pacientInfo.Fam))
                textBoxFam.Text = pacientInfo.Fam;
            textBoxIm.Text = pacientInfo.Im;
            if (!String.IsNullOrWhiteSpace(pacientInfo.Otc))
                textBoxOt.Text = pacientInfo.Otc;
            comboBoxSex.SelectedIndex = pacientInfo.SexId;
            if (pacientInfo.DR != null)
                datePicker.SelectedDate = pacientInfo.DR;
            if (!String.IsNullOrWhiteSpace(pacientInfo.Mail))
                textBoxEMail.Text = pacientInfo.Mail;
            if (!String.IsNullOrWhiteSpace(pacientInfo.Adress))
                textBoxAddress.Text = pacientInfo.Adress;
            if (!String.IsNullOrWhiteSpace(pacientInfo.Comment))
                textBoxComment.Text = pacientInfo.Comment;
            
            for (int i = 0; i < pacientInfo.Telefons.Count; i++)
            {
                listBoxTel.Items.Add(pacientInfo.Telefons[i]);
            }

            if (!String.IsNullOrWhiteSpace(pacientInfo.NickName))
                textBoxNickName.Text = pacientInfo.NickName;
            if (!String.IsNullOrWhiteSpace(pacientInfo.FromComment))
                textBoxFromComment.Text = pacientInfo.FromComment;
        }
        private void textBoxNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if ((textBoxNumber.CaretIndex - 1) != 0 &&
                (textBoxNumber.CaretIndex - 1) != 2 &&
                (textBoxNumber.CaretIndex - 1) != 3 &&
                (textBoxNumber.CaretIndex - 1) != 7 &&
                (textBoxNumber.CaretIndex - 1) != 8 &&
                (textBoxNumber.CaretIndex - 1) != 12 &&
                (textBoxNumber.CaretIndex - 1) != 15 &&
                !char.IsDigit(textBoxNumber.Text[textBoxNumber.CaretIndex - 1]))
            {
                isNotDigit();
            }

            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                repairTelefonMask();
            }

            // +7 (000) 000-00-00 - string
            // 012345678901234567 - index
            switch (textBoxNumber.CaretIndex)
            {
                case 0:
                case 2:
                case 3:
                    textBoxNumber.SelectionStart = 4;
                    textBoxNumber.SelectionLength = 1; 
                    break;
                case 1:
                case 4:
                case 5:
                case 6:
                case 9:
                case 10:
                case 11:
                case 13:
                case 14:
                case 16:
                case 17:
                    textBoxNumber.SelectionLength = 1;
                    break;
                case 7:
                case 8:
                    textBoxNumber.SelectionStart = 9;
                    textBoxNumber.SelectionLength = 1;
                    break;
                case 12:
                    textBoxNumber.SelectionStart = 13;
                    textBoxNumber.SelectionLength = 1;
                    break;
                case 15:
                    textBoxNumber.SelectionStart = 16;
                    textBoxNumber.SelectionLength = 1;
                    break;
                case 18:
                    buttonTelAdd.Focus();
                    break;
                default:
                    break;
            }
        }
        private void isNotDigit()
        {
            MessageBox.Show(Properties.Resources.pacientMessageNotDigit,
                    Properties.Resources.messageCaptionError,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);

            int ind = textBoxNumber.CaretIndex - 1;
            textBoxNumber.Text = textBoxNumber.Text.Replace(textBoxNumber.Text[ind], '0');
            textBoxNumber.CaretIndex = ind;
        }
        private void repairTelefonMask()
        {
            // +7 (000) 000-00-00 - string
            // 012345678901234567 - index
            int ind = textBoxNumber.CaretIndex;
            switch (ind)
            {
                case 0:
                    textBoxNumber.Text = textBoxNumber.Text.Insert(textBoxNumber.CaretIndex, "+");
                    break;
                case 1:
                    textBoxNumber.Text = textBoxNumber.Text.Insert(textBoxNumber.CaretIndex, "7");
                    break;
                case 3:
                    textBoxNumber.Text = textBoxNumber.Text.Insert(textBoxNumber.CaretIndex, "(");
                    break;
                case 7:
                    textBoxNumber.Text = textBoxNumber.Text.Insert(textBoxNumber.CaretIndex, ")");
                    break;
                case 2:
                case 8:
                    textBoxNumber.Text = textBoxNumber.Text.Insert(textBoxNumber.CaretIndex, " ");
                    break;
                case 12:
                case 15:
                    textBoxNumber.Text = textBoxNumber.Text.Insert(textBoxNumber.CaretIndex, "-");
                    break;
                default:
                    textBoxNumber.Text = textBoxNumber.Text.Insert(textBoxNumber.CaretIndex, "0");
                    break;
            }
            textBoxNumber.CaretIndex = ind;
        }
        private void initBoxNumber()
        {
            // +7 (000) 000-00-00 - string
            // 012345678901234567 - index
            textBoxNumber.Text = Properties.Resources.pacientLabelTelFormat;
            textBoxNumber.SelectionStart = 4;
            textBoxNumber.SelectionLength = 1;
            textBoxNumber.Focus();
        }
        private void buttonTelAdd_Click(object sender, RoutedEventArgs e)
        {
            listBoxTel.Items.Add(textBoxNumber.Text);
            initBoxNumber();
        }
        private void buttonTelDel_Click(object sender, RoutedEventArgs e)
        {
            if (!textBoxNumber.Text.Equals(Properties.Resources.pacientLabelTelFormat))
            {
                initBoxNumber();
            }
            else if (listBoxTel.SelectedIndex != -1)
            {
                listBoxTel.Items.Remove(listBoxTel.SelectedItem);
            }
            else if (listBoxTel.Items.Count > 0)
            {
                listBoxTel.Items.RemoveAt(listBoxTel.Items.Count - 1);
            }
        }
        private void buttonPacientSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxIm.Text) || comboBoxSex.SelectedIndex == -1)
            {
                MessageBox.Show(Properties.Resources.pacientMessageRequiredEmpty,
                    Properties.Resources.messageCaptionError,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            if (!String.IsNullOrWhiteSpace(textBoxFam.Text))
                pacientInfo.Fam = textBoxFam.Text;
            
            pacientInfo.Im = textBoxIm.Text;
            
            if (!String.IsNullOrWhiteSpace(textBoxOt.Text))
                pacientInfo.Otc = textBoxOt.Text;
            
            pacientInfo.SexId = (byte)(comboBoxSex.SelectedIndex);

            if (datePicker.SelectedDate != null)
                pacientInfo.DR = datePicker.SelectedDate;

            if (!String.IsNullOrWhiteSpace(textBoxEMail.Text))
                pacientInfo.Mail = textBoxEMail.Text;

            if (!String.IsNullOrWhiteSpace(textBoxAddress.Text))
                pacientInfo.Adress = textBoxAddress.Text;

            if (!String.IsNullOrWhiteSpace(textBoxComment.Text))
                pacientInfo.Comment = textBoxComment.Text;

            pacientInfo.RowUpdate = DateTime.Now;

            if (!String.IsNullOrWhiteSpace(textBoxNickName.Text))
                pacientInfo.NickName = textBoxNickName.Text;

            if (!String.IsNullOrWhiteSpace(textBoxFromComment.Text))
                pacientInfo.FromComment = textBoxFromComment.Text;

            if (listBoxTel.Items.Count != 0)
            {
                pacientInfo.Telefons.Clear();

                for (int i = 0; i < listBoxTel.Items.Count; i++)
                {
                    pacientInfo.Telefons.Add(listBoxTel.Items[i].ToString());
                }
            }

            pacientInfo.Telefons.TrimExcess();

            DialogResult = true;
            Close();
        }
        private void buttonPacientExit_Click(object sender, RoutedEventArgs e)
        {
            pacientInfo = null;

            DialogResult = false;
            Close();
        }
        private void textBoxEMail_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBoxEMail.Text.Length > 0)
            {
                int ind = textBoxEMail.CaretIndex - 1;

                if (Array.Exists(ruChars, character => character == textBoxEMail.Text[ind]))
                {
                    int pos = Array.IndexOf(ruChars, textBoxEMail.Text[ind]);

                    textBoxEMail.Text = textBoxEMail.Text.Replace(textBoxEMail.Text[ind], enChars[pos]);
                }

                textBoxEMail.CaretIndex = ind + 1;
            }
        }
        private void textBoxEMail_LostFocus(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < textBoxEMail.Text.Length; i++)
            {
                if (Array.Exists(ruChars, character => character == textBoxEMail.Text[i]))
                {
                    int pos = Array.IndexOf(ruChars, textBoxEMail.Text[i]);

                    textBoxEMail.Text = textBoxEMail.Text.Replace(textBoxEMail.Text[i], enChars[pos]);
                }
            }
        }
        private void buttonTelAdd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                textBoxNickName.Focus();
            }
        }
    }
}
