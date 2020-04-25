using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStack.OrmLite;

namespace Tutorial_Server
{
    public class Startup
    {
        Dictionary<TProtocol, TargetInfo> dicProtocol = new Dictionary<TProtocol, TargetInfo>();
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            RegisterFromAssembly();
            //���÷����� �̿��ؼ� �������ݰ� �޼ҵ� ��Ī �ϴ� �κ�
            PrepareRedis();
            PrepareTest();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapPost("/", async context =>
                 {
                     //endpoints.MapPost�� ���ؼ� Post �޼����� �޴´�.
                     //context.Request.Body�� ���ؼ� json�� Stream���·� �޾ƿ�
                     //await stream.ReadToEndAsync(); ������ ������ ���� ���
                     //���� �Ŀ� NewTonsoft��ø�������������Ʈ�� ���ؼ� CommonRequest�� ��ȯ

                     using (var stream = new StreamReader(context.Request.Body))
                     {
                         var str = await stream.ReadToEndAsync();

                         var request = (CommonRequest) Newtonsoft.Json.JsonConvert.DeserializeObject(str, typeof(CommonRequest));
                         TargetInfo target;
                         if (!dicProtocol.TryGetValue(request.protocol, out target))
                         {
                             await context.Response.WriteAsync("Hello World!");
                             return;
                         }
                         var msg = Newtonsoft.Json.JsonConvert.DeserializeObject(str, target.parameterType);
                         var response = (CommonResponse) target.method.Invoke(target.command, new object[] { msg }); //�̻��� �ֵ��� ���� ��� ���� �κ�
                         await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(response));
                            
                     }
                 });

                
            });
        }

        private void PrepareTest()
        {
            AuthCommand auth = new AuthCommand();
            ReqAuth req = new ReqAuth();
            req.accountKey = "test";
            req.authType = AuthType.Guest;
            var reqAuth = auth.Auth(req) as ResAuth;

            ReqItemGet reqItemGet = new ReqItemGet();
            reqItemGet.sessionKey = reqAuth.sessionKey;
            auth.ItemGet(reqItemGet);
        }



        //private void PrepareDatabase()
        //{
        //    int idxAccount = 0;
        //    using (var db = ORMContext.Open())
        //    {
        //        var data = db.Single<new_table>(w => w.accountKey == "");

        //        if (data == null)
        //            return Response(ResultCode.Error);
        //        idxAccount = data.idx;

        //        db.Select(db.From<T_inventory>().)
        //    }
        //}

        private void PrepareRedis()
        {
            RedisProvider.Open("15.164.165.164", 6379, "");
        }

        public void PrefareDatabase()
        {
            using (var db = ORMContext.Open())
            {  // �Ӽ����� ���� Ŭ������ �־����
                var data = db.Single<AccountData>(w => w.accountKey == "");
            }
        }


        public void RegisterFromAssembly()
        {
            //�� ������Ʈ�� DLL ������ �����´�.
            Assembly assembly = GetType().Assembly;// dll�� ����ϴ� ��� ������ ������
            //���� ������Ʈ���� CommandClass�� ���ǵ� ��� Ŭ������ �����´�.
            var types = from type in assembly.GetTypes()
                        where Attribute.IsDefined(type, typeof(CommandClassAttribute)) //CommandClassAttribute �Ǿ� �ִ� Ŭ������ ������
                        select type;
            //�Լ��� ���������� �����Ͽ� �ش�.
            foreach(var target in types) //Ŭ������ ���� Ÿ�ٸ�Ī
            {
                RegisterFromClass(target);
            }
        }

        private void RegisterFromClass(Type target)
        {
            foreach(MethodInfo method in target.GetMethods())  //Ÿ�� Ŭ������ �ִ�  �޼ҵ� �� ������
            {
                object[] attr = method.GetCustomAttributes(typeof(CommandAttribute), true); // true�� ��ӵ� �͵� �� ������
                if (attr.Length == 0)
                    continue;
                //attr[0]
                //CommandAttribute cmdAttr = (CommandAttribute)attr[0];

                TargetInfo info = new TargetInfo();  // �������ݰ� �Լ� ��Ī�� �ʿ���
                info.command = (Command)Activator.CreateInstance(target); //Type�Ű������� �޾Ƽ� ������ ����, CommandClassAttribute�� ������ �ִ� Ŭ����
                info.method = method;
                info.parameterType = method.GetParameters()[0].ParameterType; //Auth(ReqAuth req)-> ReqAuth ������ ������

                //�������� Ŭ������ �Ӽ� ����� �����´�.
                var attrs = info.parameterType.GetCustomAttributes(typeof(ProtocolAttribute),(true));
                //�Ӽ� ��Ͽ��� ù���� �Ӽ� ������ �����´�.
                ProtocolAttribute protocol =(ProtocolAttribute) attrs[0];
                
                dicProtocol.Add(protocol.protocol, info); //


            }
        }
    }

    internal class TargetInfo
    {
        public Command command;
        public MethodInfo method;
        public Type parameterType; //Method�� ù��° �Ķ���� Ŭ���� ���� - Attribute�� �������� Key Value Match��
    }
}
