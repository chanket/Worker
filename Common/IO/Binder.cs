using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.IO
{
    /// <summary>
    /// 实现客户端和服务端之间序列化帧的程序集信息转换。
    /// </summary>
    class Binder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            return Type.GetType(typeName);
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = "x";
            typeName = serializedType.FullName;
            typeName = Regex.Replace(typeName, @", [^\]]+", "");    //typeName中也携带了程序集信息
        }
    }
}
