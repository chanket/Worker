using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Common.Frames;
using System.Security.Cryptography;

namespace Common.IO
{
    /// <summary>
    /// 实现基于流透明地读写<see cref="FrameBase"/>的功能。
    /// </summary>
    public class IOBase
    {
        /// <summary>
        /// 从流中读取指定字节数；如缓冲区中字节数不足，则会等待直到读取完毕。
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <returns></returns>
        private async Task ReadToBuffer(Stream stream, byte[] buffer, int offset, int count)
        {
            int size = await stream.ReadAsync(buffer, offset, count).ConfigureAwait(false);
            if (size == 0) throw new IOException("Stream Closed.");
            if (size < count)
            {
                await ReadToBuffer(stream, buffer, offset + size, count - size).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 从目标流中读取下一个Frame对象
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <returns></returns>
        protected async Task<FrameBase> ReadFrame(Stream stream)
        {
            //Read Size As Int32
            byte[] sizeBuffer = new byte[4];
            await ReadToBuffer(stream, sizeBuffer, 0, sizeBuffer.Length).ConfigureAwait(false);
            int size = BitConverter.ToInt32(sizeBuffer, 0);

            if (size > 0 && size <= Common.Configs.MaxFrameSize)
            {
                //Read Frame To Buffer
                byte[] buffer = new byte[size];
                await ReadToBuffer(stream, buffer, 0, buffer.Length).ConfigureAwait(false);

                SymmetricAlgorithm des = Rijndael.Create();
                des.Mode = CipherMode.CBC;
                des.Key = Configs.AesKey;
                des.IV = Configs.AesIV;
                using (MemoryStream ms = new MemoryStream(buffer))
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                using (DeflateStream ds = new DeflateStream(cs, CompressionMode.Decompress))
                {
                    BinaryFormatter formatter = new BinaryFormatter() { Binder = new Binder(), };
                    FrameBase frame = formatter.Deserialize(ds) as FrameBase;

                    if (frame != null)
                    {
                        return frame;
                    }
                    else
                    {
                        throw new FormatException("Invalid Frame.");
                    }
                }
            }
            else
            {
                throw new FormatException("Wrong Size.");
            }
        }

        /// <summary>
        /// 将Frame对象写入目标流中。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="frame"></param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <returns></returns>
        protected async Task WriteFrame(Stream stream, FrameBase frame)
        {
            SymmetricAlgorithm des = Rijndael.Create();
            des.Mode = CipherMode.CBC;
            des.Key = Configs.AesKey;
            des.IV = Configs.AesIV;
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
                using (DeflateStream ds = new DeflateStream(cs, CompressionLevel.Optimal, true))
                {
                    BinaryFormatter formatter = new BinaryFormatter() { Binder = new Binder(), };
                    formatter.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded;
                    formatter.Serialize(ds, frame);
                }
                cs.FlushFinalBlock();

                if (ms.Length > Common.Configs.MaxFrameSize)
                {
                    throw new FormatException("Maximum Frame Size Exceeded.");
                }

                int size = (int)ms.Length;
                await stream.WriteAsync(BitConverter.GetBytes(size), 0, 4).ConfigureAwait(false);
                await stream.WriteAsync(ms.GetBuffer(), 0, size).ConfigureAwait(false);
            }
        }
    }
}
