using library_management.model;
using library_management.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace library_management.VIew
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;
            btnCancel.Click += (s, e) => Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var username = txtUsername.Text.Trim();
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(txtPassword.Text))
            { MessageBox.Show("Enter a username and password."); return; }
            if (txtPassword.Text.Length < 8) { MessageBox.Show("Use a password with at least 8 characters."); return; }
            try
            {
                if (!DatabaseService.RegisterUser(username, txtPassword.Text))
                { MessageBox.Show("That username is already registered."); return; }
                MessageBox.Show("Registration successful. You can now sign in.");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { MessageBox.Show("Could not register. Check the database connection.\n\n" + ex.Message); }
        }

        private void lblSignIn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
