using System;
using System.Data.SqlClient;

namespace BaselineLab
{
    class Program
    {
        // 
        const string SqlServer   = "192.168.4.66,1433";
        const string SqlDatabase = "LabDB";

        //
        static readonly string[,] DbAccounts = new string[,]
        {
            { "app_rw",     "P@ssw0rd_Lab_01!" },
            { "app_ro",     "P@ssw0rd_Lab_02!" },
            { "sql_backup", "P@ssw0rd_Lab_03!" },
            { "batch_job",  "P@ssw0rd_Lab_04!" },
            { "svc_admin",  "P@ssw0rd_Lab_05!" }
        };
        // =====================================================================

        static void Main(string[] args)
        {
            for (int i = 0; i < DbAccounts.GetLength(0); i++)
            {
                string user = DbAccounts[i, 0];
                string pass = DbAccounts[i, 1];

                Console.WriteLine("=== " + user + " ===");
                try
                {
                    TestSqlConnection(user, pass);
                    Console.WriteLine("  Connect to MSSQL: OK");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  Error: " + ex.Message);
                }
                Console.WriteLine();
            }

            Console.WriteLine("Done. Press Enter to exit.");
            Console.ReadLine();
        }

        static void TestSqlConnection(string user, string password)
        {
            string connStr = string.Format(
                "Server={0};Database={1};User Id={2};Password={3};Connection Timeout=10;",
                SqlServer, SqlDatabase, user, password);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT SUSER_SNAME()", conn))
                {
                    Console.WriteLine("  Connected to SQL with user: " + Convert.ToString(cmd.ExecuteScalar()));
                }
            }
        }
    }
}