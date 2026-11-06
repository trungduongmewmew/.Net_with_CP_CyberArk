using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace BaselineLab
{
    class Program
    {
        
        const string SqlServer   = "192.168.4.66,1433";
        const string SqlDatabase = "LabDB";

        
        const string CliPath = @"C:\Program Files\CyberArk\ApplicationPasswordSdk\CLIPasswordSDK.exe";
        const string CpAppId = ".net";
        const string CpSafe  = "DB";
        const string CpFolder = "Root";

        class DbAccount
        {
            public string DbUser;    
            public string CpObject;  

            public DbAccount(string dbUser, string cpObject)
            {
                DbUser = dbUser;
                CpObject = cpObject;
            }
        }

        
        static readonly DbAccount[] Accounts = new DbAccount[]
        {
            new DbAccount("app_rw",     "Database-MySQLx64-192.168.4.66-app_rw"),
            new DbAccount("app_ro",     "Database-MySQLx64-192.168.4.66-app_ro"),
            new DbAccount("sql_backup", "Database-MySQLx64-192.168.4.66-sql_backup"),
            new DbAccount("batch_job",  "Database-MySQLx64-192.168.4.66-batch_job"),
            new DbAccount("svc_admin",  "Database-MySQLx64-192.168.4.66-svc_admin")
        };

        static void Main(string[] args)
        {
            foreach (DbAccount acc in Accounts)
            {
                Console.WriteLine("=== " + acc.DbUser + " ===");
                try
                {
                    string password = GetPasswordFromCli(acc.CpObject);
                    TestSqlConnection(acc.DbUser, password);
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

        static string GetPasswordFromCli(string objectName)
        {
            string query = "Safe=" + CpSafe + ";Folder=" + CpFolder + ";Object=" + objectName;
            string args = "GetPassword /p AppDescs.AppID=" + CpAppId
                        + " /p Query=\"" + query + "\" /o Password";

            ProcessStartInfo psi = new ProcessStartInfo(CliPath, args);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;

            using (Process p = Process.Start(psi))
            {
                string stdout = p.StandardOutput.ReadToEnd();
                string stderr = p.StandardError.ReadToEnd();

                if (!p.WaitForExit(15000))
                {
                    try { p.Kill(); } catch { }
                    throw new InvalidOperationException("CLI not respond in 15 seconds.");
                }

               
                if (p.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        "CLI exit code " + p.ExitCode + ". " + stderr.Trim());
                }

                string password = stdout.Trim();
                if (password.Length == 0)
                {
                    throw new InvalidOperationException("CLI không trả về password.");
                }
                return password;
            }
        }

        static void TestSqlConnection(string user, string password)
        {
            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
            b.DataSource = SqlServer;
            b.InitialCatalog = SqlDatabase;
            b.UserID = user;
            b.Password = password;
            b.ConnectTimeout = 10;

            using (SqlConnection conn = new SqlConnection(b.ConnectionString))
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