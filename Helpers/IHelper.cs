namespace GenerateEncryptedFile.Helpers
{
    public interface IHelper
    {
        public byte[] EncryptStringToBytes(string plainText, string password);
        public string DecryptBytesToString(byte[] encryptedBytes, string password);
    }
}
