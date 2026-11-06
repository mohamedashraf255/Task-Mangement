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
    public partial class CreateAccount : Form
    {
        public CreateAccount()
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox_ShowPass_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void lblLogin_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        private void checkBox_ShowPass_CheckedChanged_1(object sender, EventArgs e)
        {
            if(checkBox_ShowPass.Checked)
            {
                textBoxPass.UseSystemPasswordChar = false;
                textBoxConfPass.UseSystemPasswordChar = false;
            }
            else
            {
                textBoxPass.UseSystemPasswordChar = true;
                textBoxConfPass.UseSystemPasswordChar = true;
            }
        }

        private void textBoxPass_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxConfPass_TextChanged(object sender, EventArgs e)
        {

        }

        private void CreateAccount_Load(object sender, EventArgs e)
        {

        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            

            MessageBox.Show("Account created successfully; please log in.", "success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();   
        }
    }
}
