using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerCore
{
    static class Utils
    {
        /// <summary>
        /// 使用<paramref name="arguments"/>作为参数启动<paramref name="filename"/>，
        /// 指定<paramref name="inputs"/>为标准输入，
        /// 在<paramref name="timeout"/>毫秒前返回所有输出。
        /// </summary>
        public static async Task<List<string>> ExecuteResultsAsync(string filename, string arguments, IEnumerable<string> inputs, int timeout = Timeout.Infinite)
        {
            List<string> retval = new List<string>();
            using (Process process = new Process())
            using (Timer timer = new Timer(new TimerCallback((object obj) => { try { process.Kill(); } catch { } }), null, timeout, Timeout.Infinite))
            {
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = filename;
                process.StartInfo.Arguments = arguments;

                if (process.Start())
                {
                    try
                    {
                        foreach (string input in inputs)
                        {
                            await process.StandardInput.WriteLineAsync(input).ConfigureAwait(false);
                        }

                        for (string output = await process.StandardOutput.ReadLineAsync().ConfigureAwait(false);
                            output != null;
                            output = await process.StandardOutput.ReadLineAsync().ConfigureAwait(false))
                        {
                            retval.Add(output);
                        }
                    }
                    catch
                    {
                        return retval;
                    }
                }
                else
                {
                    return null;
                }
            }

            return retval;
        }

        /// <summary>
        /// 打开<paramref name="filename"/>文件并返回首次满足<paramref name="regex"/>的字符串。
        /// </summary>
        /// <returns>如果匹配失败返回空；如果发生异常返回null。</returns>
        public static async Task<string> MatchFileContentAsync(string filename, Regex regex)
        {
            try
            {
                using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    for (string line = await sr.ReadLineAsync().ConfigureAwait(false);
                        line != null;
                        line = await sr.ReadLineAsync().ConfigureAwait(false))
                    {
                        var matchCpuName = regex.Match(line);
                        if (matchCpuName.Success)
                        {
                            return matchCpuName.Groups[1].Value;
                        }
                    }
                }

                return "";
            }
            catch
            {
                return null;
            }
        }
    }
}
