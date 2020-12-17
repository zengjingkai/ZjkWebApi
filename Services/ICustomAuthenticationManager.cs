using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZjkWebAPIDemo.Services
{

    public interface ICustomAuthenticationManager
    {
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        string Authenticate(string username, string password);

        /// <summary>
        /// 获取Token
        /// </summary>
        IDictionary<string, string> Tokens { get; }
    }
}
