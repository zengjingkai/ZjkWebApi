using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZjkWebAPIDemo.Services;
using ZjkWebAPIDemo.Services.ServiceImpl;

namespace ZjkWebAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// 获取用户
        /// </summary>
        [HttpGet("user")]
        [Authorize(Roles = "admin")]
        public string getUser()
        {
            return "user";
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <returns></returns>
        [HttpGet("role")]
        [Authorize(Roles = "admin")]
        public IEnumerable<Claim> GetRole()
        {
            return HttpContext.User.FindAll(c => c.Type == ClaimTypes.Role);
        }


        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
         [HttpPost("login")]
        public string Login(string UserName,string Password)
        {
            ICustomAuthenticationManager ic = new CustomAuthenticationManager();
            return ic.Authenticate(UserName, Password);
        }
    }
}
