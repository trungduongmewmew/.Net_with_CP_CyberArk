using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using LabConfig;

class Program
{
    static void Main()
    {
        try
        {
            string connStr = LoadDecryptedConnString();
            Console.WriteLine("Connection string has been decrypted (not printed to avoid exposure).");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT SUSER_SNAME(), DB_NAME(), @@SERVERNAME", conn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        Console.WriteLine("Login  : " + r.GetString(0));
                        Console.WriteLine("Database: " + r.GetString(1));
                        Console.WriteLine("Server : " + r.GetString(2));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
    static string LoadDecryptedConnString()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LabCrypto.dll");
        Assembly asm = Assembly.LoadFrom(path);

        Type t = asm.GetType("LabCrypto.Protector");
        MethodInfo m = t.GetMethod("Decrypt", BindingFlags.Public | BindingFlags.Static);

        return (string)m.Invoke(null, new object[] { ConnData.Cipher });
    }
}