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
    /// �������
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ��ڹ��캯��
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// ����
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// �˷���������ʱ���á�ʹ�ô˷�����������ӵ������С�
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
                    Description = "�����֤����Ȩ���",
                    Version = "V1",
                    Title = "JWTȨ����֤",
                  
                });
                //��xmlע��չʾ�ڽ���
                var xmlPath = AppDomain.CurrentDomain.BaseDirectory + "ZjkWebAPIDemo.xml";
                options.IncludeXmlComments(xmlPath);
                // �ӿ�����
                options.OrderActionsBy(o => o.RelativePath);
                var scheme = new OpenApiSecurityScheme()
                {
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    //ͷ����
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
                    ValidateIssuer = false, //�Ƿ���֤������
                    ValidateAudience = false,//��������֤�ڼ���֤���� ��
                    ValidateLifetime = false,//��֤�������ڡ�
                    ValidateIssuerSigningKey = true,//�Ƿ���ö�ǩ��securityToken��SecurityKey������֤��
                    ValidIssuer = "admin",//�����ڼ�����Ƶķ������Ƿ���˷�������ͬ��
                    ValidAudience = "admin",//������Ƶ�����Ⱥ���Ƿ��������Ⱥ����ͬ��
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretsecretsecret"))
                };
            });
        }

        /// <summary>
        /// �˷���������ʱ���á�ʹ�ô˷�������HTTP����ܵ���
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //�����֤�м��(�ȿӣ���Ȩ�м����������֤�м��֮ǰ)
            app.UseAuthentication();
            //�����м����������Swagger��ΪJSON�ս��
            app.UseSwagger();
            app.UseRouting();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "JWTȨ����֤");
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
