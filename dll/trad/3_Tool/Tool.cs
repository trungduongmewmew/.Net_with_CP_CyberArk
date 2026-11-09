using System;
using LabCrypto;
class Tool
{
    static void Main()
    {
        string plain =
            "Server=192.168.4.66,1433;Database=LabDB;" +
            "User ID=app_ro;Password=P@ssw0rd_Lab_02!;" +
            "TrustServerCertificate=True;";

        string cipher = Protector.Encrypt(plain);
        Console.WriteLine(cipher);
    }
}