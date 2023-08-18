using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace TranKhanhDuy
{
    public partial class Form1 : Form
    {
        string strConnect = "Data Source=.;Initial Catalog=Interview_TranKhanhDuy;Integrated Security=True;";

        // Đối tượng kết nối
        SqlConnection conn = null;
        // Đối tượng đưa dữ liệu vào DataTable dtUser 
        SqlDataAdapter daUser = null;
        SqlDataAdapter daGender = null;

        // Đối tượng hiển thị dữ liệu lên Form 
        DataTable dtUser = null;
        DataSet dtGender = null;



        public string ConnectStr = "Connected!";
        public string DisconnectStr = "Disconnected!";
        public string ErrconnectStr = "ERR";

        public int StatusOfFlag;
        public enum CurrentStatus
        {
            Add = 0,
            Edit = 1,
            Remove = 2,
        }

        public string[] GenderArray = { "Nam", "Nữ", "Khác" };
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            Txt_Status.Text = DisconnectStr;
            Btn_Cancel.Enabled = false;
            Btn_Remove.Enabled = false;
            Btn_Save.Enabled = false;
            Btn_Edit.Enabled = false;
            Btn_Add.Enabled = false;
            ChangeStatusOfTxtToOff();
        }

        private void Btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {

                LoadData();
                Txt_Status.Text = ConnectStr;

            }
            catch (SqlException err)
            {
                MessageBox.Show(err.ToString());
                Txt_Status.Text = ErrconnectStr;
            }
        }

        public void LoadData()
        {
            conn = new SqlConnection(strConnect);
            daUser = new SqlDataAdapter("SELECT * FROM [User] WHERE (IsDisplay = 1)", conn);
            dtUser = new DataTable();
            daUser.Fill(dtUser);
            dataGridView1.DataSource = dtUser;

            //Combobox
            daGender = new SqlDataAdapter("SELECT * FROM [Gender]", conn);
            dtGender = new DataSet();
            daGender.Fill(dtGender, "Gender");

            Cbb_Gender.DisplayMember = "Name";
            Cbb_Gender.ValueMember = "Id";
            Cbb_Gender.DataSource = dtGender.Tables["Gender"];
            Btn_Remove.Enabled = true;
            Btn_Edit.Enabled = true;
            Btn_Add.Enabled = true;
            Btn_Cancel.Enabled = false;
            Btn_Save.Enabled = false;



            int l_SelectedIndex = dataGridView1.CurrentCell.RowIndex;

            Txt_FirstName.Text = dataGridView1.Rows[l_SelectedIndex].Cells[0].Value.ToString();
            Txt_LastName.Text = dataGridView1.Rows[l_SelectedIndex].Cells[1].Value.ToString();
            Txt_IdCard.Text = dataGridView1.Rows[l_SelectedIndex].Cells[2].Value.ToString();
            Txt_DateOfBirth.Text = dataGridView1.Rows[l_SelectedIndex].Cells[5].Value.ToString();
            Txt_TaxId.Text = dataGridView1.Rows[l_SelectedIndex].Cells[3].Value.ToString();
            Txt_Email.Text = dataGridView1.Rows[l_SelectedIndex].Cells[6].Value.ToString();
            Cbb_Gender.SelectedValue = ReturnSelectedValue(dataGridView1.Rows[l_SelectedIndex].Cells[4].Value.ToString());
        }

        private void Btn_Disconnect_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            Txt_Status.Text = DisconnectStr;
            Txt_Email.Clear();
            Txt_DateOfBirth.Clear();
            Txt_FirstName.Clear();
            Txt_IdCard.Clear();
            Txt_LastName.Clear();
            Cbb_Gender.SelectedItem = "";
            Btn_Cancel.Enabled = false;
            Btn_Remove.Enabled = false;
            Btn_Save.Enabled = false;
            Btn_Edit.Enabled = false;
            Btn_Add.Enabled = false;
        }

        public void ChangeStatusOfTxtToOff()
        {
            Txt_Email.Enabled = false;
            Txt_DateOfBirth.Enabled = false;
            Txt_FirstName.Enabled = false;
            Txt_IdCard.Enabled = false;
            Txt_LastName.Enabled = false;
            Cbb_Gender.Enabled = false;
        }

        public void ChangeStatusOfTxtToOn()
        {
            Txt_Email.Enabled = true;
            Txt_DateOfBirth.Enabled = true;
            Txt_FirstName.Enabled = true;
            Txt_IdCard.Enabled = true;
            Txt_LastName.Enabled = true;
            Cbb_Gender.Enabled = true;
        }

        public void InitValueOfTxt()
        {
            Txt_Email.Text = string.Empty;
            Txt_DateOfBirth.Text = string.Empty;
            Txt_FirstName.Text = string.Empty;
            Txt_IdCard.Text = string.Empty;
            Txt_LastName.Text = string.Empty;
            Cbb_Gender.Text = string.Empty;
        }
        public void ActionWhenClickBtnAdd()
        {
            Btn_Add.Enabled = false;
            Btn_Edit.Enabled = false;
            Btn_Remove.Enabled = false;

            Btn_Cancel.Enabled = true;
            Btn_Save.Enabled = true;

            InitValueOfTxt();
            Cbb_Gender.SelectedIndex = 1;
            ChangeStatusOfTxtToOn();
            StatusOfFlag = (int)CurrentStatus.Add;
            Txt_FirstName.Focus();

        }

        public void ActionWhenClickBtnEdit()
        {
            Btn_Add.Enabled = false;
            Btn_Edit.Enabled = false;
            Btn_Remove.Enabled = false;

            Btn_Cancel.Enabled = true;
            Btn_Save.Enabled = true;

            ChangeStatusOfTxtToOn();
            StatusOfFlag = (int)CurrentStatus.Edit;
        }

        public void ActionWhenClickBtnCancel()
        {
            InitValueOfTxt();
            ChangeStatusOfTxtToOff();
            Btn_Cancel.Enabled = false;
            Btn_Save.Enabled = false;

            Btn_Add.Enabled = true;
            Btn_Edit.Enabled = true;
            Btn_Remove.Enabled = true;
        }

        public void ActionWhenClickBtnRemove()
        {

            // Khai báo biến traloi 
            DialogResult traloi;
            // Hiện hộp thoại hỏi đáp 
            traloi = MessageBox.Show("Chắc xóa không?", "Trả lời",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            // Kiểm tra có nhắp chọn nút Ok không? 
            if (traloi == DialogResult.OK)
            {
                // Mở kết nối 
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Open();
                try
                {

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    //Get index
                    int l_CurrentIndex = dataGridView1.CurrentCell.RowIndex;

                    //Get IdCard
                    string l_IdCardSelected = dataGridView1.Rows[l_CurrentIndex].Cells[2].Value.ToString();

                    //Sql Command
                    cmd.CommandText = "Update [User] Set IsDisplay=N'" + 0
                        + "' Where IdCard=" + l_IdCardSelected.Trim();

                    cmd.ExecuteNonQuery();


                    InitValueOfTxt();
                    ChangeStatusOfTxtToOff();
                    Btn_Cancel.Enabled = false;
                    Btn_Save.Enabled = false;
                    Btn_Add.Enabled = true;
                    Btn_Edit.Enabled = true;
                    Btn_Remove.Enabled = true;
                    LoadData();
                }
                catch (SqlException err)
                {
                    MessageBox.Show(err.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void ActionWhenClickBtnSave()
        {
            // Mở kết nối 
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();

            //Save Data to Db()
            if (StatusOfFlag == (int)CurrentStatus.Add)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    //Insert Into
                    cmd.CommandText = "Insert Into [User] Values('" + Txt_IdCard.Text.Trim() + "',N'" + Txt_FirstName.Text.Trim() + "',N'" +
                        Txt_LastName.Text.Trim() + "',N'" + Txt_TaxId.Text.Trim() + "',N'" + Txt_Email.Text.Trim() + "',N'" + Txt_DateOfBirth.Text.Trim()
                         + "',N'" + Cbb_Gender.Text.ToString().Trim() + "',N'" + 1 + "')";
                    cmd.ExecuteNonQuery();

                    LoadData();
                    MessageBox.Show("Added item!");
                }
                catch (SqlException err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
            else if (StatusOfFlag == (int)CurrentStatus.Edit)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    //Get index
                    int l_CurrentIndex = dataGridView1.CurrentCell.RowIndex;

                    //Get IdCard
                    string l_IdCardSelected = dataGridView1.Rows[l_CurrentIndex].Cells[2].Value.ToString();

                    //Sql Command
                    cmd.CommandText = "Update [User] Set " +
                         "FirstName=N'" + Txt_FirstName.Text.Trim()
                        + "', LastName =N'" + Txt_LastName.Text.Trim()
                        + "', IdTax =N'" + Txt_TaxId.Text.Trim()
                        + "', Email =N'" + Txt_Email.Text.Trim()
                        + "', DateOfBirth =N'" + Txt_DateOfBirth.Text.Trim()
                        + "', Gender =N'" + Cbb_Gender.Text.ToString().Trim()
                        + "' Where IdCard=" + l_IdCardSelected.Trim();

                    cmd.ExecuteNonQuery();
                    LoadData();

                    MessageBox.Show("Edit Done!");
                }
                catch (SqlException err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            ActionWhenClickBtnAdd();
        }

        private void Btn_Edit_Click(object sender, EventArgs e)
        {
            ActionWhenClickBtnEdit();
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            ActionWhenClickBtnCancel();
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            ActionWhenClickBtnRemove();
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            ActionWhenClickBtnSave();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView1_CellClick(object sender,DataGridViewCellEventArgs e)
        {
            int l_SelectedIndex = dataGridView1.CurrentCell.RowIndex;

            Txt_FirstName.Text = dataGridView1.Rows[l_SelectedIndex].Cells[0].Value.ToString();
            Txt_LastName.Text = dataGridView1.Rows[l_SelectedIndex].Cells[1].Value.ToString();
            Txt_IdCard.Text = dataGridView1.Rows[l_SelectedIndex].Cells[2].Value.ToString();
            Txt_DateOfBirth.Text = dataGridView1.Rows[l_SelectedIndex].Cells[5].Value.ToString();
            Txt_TaxId.Text = dataGridView1.Rows[l_SelectedIndex].Cells[3].Value.ToString();
            Txt_Email.Text = dataGridView1.Rows[l_SelectedIndex].Cells[6].Value.ToString();
            Cbb_Gender.SelectedValue = ReturnSelectedValue(dataGridView1.Rows[l_SelectedIndex].Cells[4].Value.ToString());
        }

        public int ReturnSelectedValue(string str)
        {
            for (int i = 0; i < GenderArray.Length; i++)
            {
                if(str == GenderArray[i])
                {
                    return i +1;
                }
            }
            return 0;
        }
    }
}
