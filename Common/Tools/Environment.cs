using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Tools
{
    public static class Environment
    {
        /// <summary>
        /// 将<paramref name="input"/>中所有以“%VariableName%”表示的部分替换为当前系统下的环境变量。
        /// </summary>
        /// <returns></returns>
        public static string ReplaceEnvironmentVars(string input)
        {
            if (input != null)
            {
                Regex reg = new Regex("%(.+?)%", RegexOptions.Compiled);
                return reg.Replace(input, new MatchEvaluator((Match match) =>
                {
                    try
                    {
                        return System.Environment.GetEnvironmentVariable(match.Groups[1].Value);
                    }
                    catch
                    {
                        return "";
                    }
                }));
            }
            else
            {
                return input;
            }
        }
    }
}
