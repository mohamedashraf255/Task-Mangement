using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_Mangement
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void lblRememder_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblCreateAcc_Click(object sender, EventArgs e)
        {
            CreateAccount createAccount = new CreateAccount();
            this.Hide();
            //الكود هيقف هنا لحد ما يسجل دخول و يجرع ل تسجيل الدخول 
            createAccount.ShowDialog();
            this.Show();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();

            this.Hide();

            dashboard.ShowDialog();

            
            if (dashboard.DialogResult == DialogResult.Yes)
            {
                
                this.Show();
            }
            else
            {
                
                Application.Exit();
            }
        }

        private void checkBox_Remember_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void checkBox_ShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox_ShowPass.Checked)
            {
                TextBoxPass.UseSystemPasswordChar = false;
            }
            else
            {
                TextBoxPass.UseSystemPasswordChar = true;
            }
        }

        private void TextBoxPass_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxEmail_Enter(object sender, EventArgs e)
        {
            textBoxEmail.Text = "";
            textBoxEmail.ForeColor = Color.Black;
        }

        private void textBoxEmail_Leave(object sender, EventArgs e)
        {
            textBoxEmail.Text = "Name@Gmail.com"; 
            textBoxEmail.ForeColor = Color.Gray;
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnlogin_MouseCaptureChanged(object sender, EventArgs e)
        {

        }
    }
}
