using System;
using Dune.Encryption.Blowfish;

namespace Dune.Encryption
{
    public static class Crypto
    {
        private const int BlockSize = BlowfishEngine.BLOCK_SIZE;

        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (key == null) throw new ArgumentNullException("key");
            return ProcessData(data, key, forEncryption: false);
        }

        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (key == null) throw new ArgumentNullException("key");
            return ProcessData(data, key, forEncryption: true);
        }

        private static byte[] ProcessData(byte[] data, byte[] key, bool forEncryption)
        {
            byte[] input = data;

            bool paddingRequired = data.Length % BlockSize > 0;
            if (paddingRequired)
            {
                input = new byte[data.Length + BlockSize - data.Length % BlockSize];
                Buffer.BlockCopy(data, 0, input, 0, data.Length);
            }

            byte[] output = new byte[input.Length];

            BlowfishEngine engine = new BlowfishEngine();
            engine.Init(forEncryption, key);

            int offset = 0;
            while (offset < input.Length)
            {
                engine.ProcessBlock(input, offset, output, offset);

                offset += BlockSize;
            }

            if (paddingRequired)
            {
                byte[] unpaddedOutput = new byte[data.Length];
                Buffer.BlockCopy(output, 0, unpaddedOutput, 0, unpaddedOutput.Length);
                return unpaddedOutput;
            }

            return output;
        }
    }
}