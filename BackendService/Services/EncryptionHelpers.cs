using System.Security.Cryptography;
using System.Text;

namespace BackendService.Services;

public static class EncryptionHelpers
{
    public static string ComputeHash(string input, HashAlgorithm algorithm = null)
    {
        if (algorithm == null)
        {
            algorithm = new SHA256CryptoServiceProvider();
        }
        Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

        return BitConverter.ToString(hashedBytes);
    }
}