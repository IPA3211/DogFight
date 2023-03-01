using System.Security.Cryptography;
using System.Text;

public class Encryptor
{
    public static string EncryptionSHA256(string message)
    {
        byte[] array = Encoding.Default.GetBytes(message);
        byte[] hashValue;
        string result = string.Empty;

        using (SHA256 mySHA256 = SHA256.Create())
        {
            hashValue = mySHA256.ComputeHash(array);
        }

        for (int i = 0; i < hashValue.Length; i++)
        {
            result += hashValue[i].ToString("x2");
        }
        return result;
    }
}