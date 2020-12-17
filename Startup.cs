using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ZjkWebAPIDemo.Models;

namespace ZjkWebAPIDemo
{
    /// <summary>
    /// 程序入口
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 入口构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 此方法由运行时调用。使用此方法将服务添加到容器中。
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Add(new ServiceDescriptor(typeof(DbContext),new DbContext(Configuration.GetConnectionString("DefaultConnection"))));
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("V1", new OpenApiInfo
                {
                    Description = "身份认证和授权详解",
                    Version = "V1",
                    Title = "JWT权限验证",
                  
                });
                //将xml注释展示于界面
                var xmlPath = AppDomain.CurrentDomain.BaseDirectory + "ZjkWebAPIDemo.xml";
                options.IncludeXmlComments(xmlPath);
                // 接口排序
                options.OrderActionsBy(o => o.RelativePath);
                var scheme = new OpenApiSecurityScheme()
                {
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    //头名称
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Bearer Token"
                };
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                             {
                                Type = ReferenceType.SecurityScheme,
                                 Id = "Bearer"
                             }
                         },
                         new string[] {}
                     }
                });

            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, //是否验证发行者
                    ValidateAudience = false,//在令牌验证期间验证受众 。
                    ValidateLifetime = false,//验证生命周期。
                    ValidateIssuerSigningKey = true,//是否调用对签名securityToken的SecurityKey进行验证。
                    ValidIssuer = "admin",//将用于检查令牌的发行者是否与此发行者相同。
                    ValidAudience = "admin",//检查令牌的受众群体是否与此受众群体相同。
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretsecretsecret"))
                };
            });
        }

        /// <summary>
        /// 此方法由运行时调用。使用此方法配置HTTP请求管道。
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //身份认证中间件(踩坑：授权中间件必须在认证中间件之前)
            app.UseAuthentication();
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            app.UseRouting();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "JWT权限验证");
               // c.RoutePrefix = string.Empty;

            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
