using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSVRInstaller
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

            Application.Run(new Form1(AutoFindGame()));
        }

        private static string AutoFindGame()
        {
            string gameExeName = "Virtual Succubus.exe"; // The name of the executable to find

            // The provider for Windows Search
            string connectionString = "Provider=Search.CollatorDSO;Extended Properties='Application=Windows';";

            // Query the Windows Search index. 
            // We look for items where the file name matches our exe. 
            // Note: SystemIndex is the name of the search catalog.
            // System.FileName is a known Windows Search property.
            // Use '%' wildcards if partial matches are needed, e.g. '%MyGame.exe%'
            string query = "SELECT System.ItemPathDisplay FROM SYSTEMINDEX " +
                           "WHERE System.FileName = '" + gameExeName + "'";

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // System.ItemPathDisplay is the full path to the file
                                string filePath = reader.GetString(0);
                                Console.WriteLine("Found file: " + filePath);
                                return filePath;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying Windows Search: " + ex.Message);
                return "";
            }
            return "";
        }
    }
}
