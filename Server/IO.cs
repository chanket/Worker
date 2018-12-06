using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Protocol.Frames;
using System.IO.Compression;

namespace Protocol
{
    public class IO
    {
        /// <summary>
        /// 从目标流中读取下一个Frame对象
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="FormatException"></exception>
        /// <returns></returns>
        protected async Task<Frame> ReadFrame(Stream stream)
        {
            //Read Size As Int32
            byte[] sizeBuffer = new byte[4];
            if (await stream.ReadAsync(sizeBuffer, 0, sizeBuffer.Length).ConfigureAwait(false) != sizeBuffer.Length)
            {
                throw new FormatException("Size Error.");
            };
            int size = BitConverter.ToInt32(sizeBuffer, 0);

            if (size > 0)
            {
                //Read Frame To Buffer
                byte[] buffer = new byte[size];
                if (await stream.ReadAsync(buffer, 0, size).ConfigureAwait(false) != size)
                {
                    throw new FormatException("Unexpected End Of Stream.");
                };

                //Deserialize Buffer To Frame
                using (MemoryStream ms = new MemoryStream(buffer))
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Frame frame = formatter.Deserialize(ds) as Frame;

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
        /// 将Frame对象写入目标流中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="frame"></param>
        /// <exception cref="IOException"></exception>
        /// <returns></returns>
        protected async Task WriteFrame(Stream stream, Frame frame)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionLevel.Optimal, true))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ds, frame);
                }

                try
                {
                    int size = (int)ms.Length;

                    //Write Size As Int32
                    await stream.WriteAsync(BitConverter.GetBytes(size), 0, 4).ConfigureAwait(false);

                    //Write Serialized Frame
                    await stream.WriteAsync(ms.GetBuffer(), 0, size).ConfigureAwait(false);
                }
                catch
                {
                    throw new IOException("Cannot Write To Stream.");
                }
            }
        }
    }
}
