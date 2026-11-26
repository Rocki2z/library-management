using System;
using System.Collections.Generic;
using System.Windows.Forms;
using library_management.model;
using library_management.View;



namespace library_management
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try { DatabaseService.Initialize(); }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot connect to PostgreSQL. Check LIBRARY_DB_CONNECTION in .env and make sure the library_management database exists.\n\n" + ex.Message,
                    "Database setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new frmLogIn());
        }
    }
}
