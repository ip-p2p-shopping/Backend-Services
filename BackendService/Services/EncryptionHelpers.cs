using System.Security.Cryptography;
using System.Text;

namespace BackendService.Services;

public static class EncryptionHelpers
{
    public static string ComputeHash(string input, string salt, HashAlgorithm algorithm = null)
    {
        if (algorithm == null)
        {
            algorithm = new SHA256CryptoServiceProvider();
        }
        Byte[] inputBytes = Encoding.UTF8.GetBytes(input + salt);

        Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

        return BitConverter.ToString(hashedBytes);
    }

    public static string CreateSalt() {
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        byte[] buff = new byte[6];
        rng.GetBytes(buff);
        
        // Return a Base64 string representation of the random number.
        return Convert.ToBase64String(buff);
    }
}