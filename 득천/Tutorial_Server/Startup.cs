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
            //리플렉션을 이용해서 프로토콜과 메소드 매칭 하는 부분
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
                     //endpoints.MapPost를 통해서 Post 메세지를 받는다.
                     //context.Request.Body를 통해서 json을 Stream형태로 받아옴
                     //await stream.ReadToEndAsync(); 끝까지 읽을때 까지 대기
                     //읽은 후에 NewTonsoft디시리얼라이즈오브젝트를 통해서 CommonRequest로 변환

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
                         var response = (CommonResponse) target.method.Invoke(target.command, new object[] { msg }); //이상한 애들이 들어온 경우 실행 부분
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
            {  // 속성으로 만든 클래스가 있어야함
                var data = db.Single<AccountData>(w => w.accountKey == "");
            }
        }


        public void RegisterFromAssembly()
        {
            //이 프로젝트에 DLL 정보를 가져온다.
            Assembly assembly = GetType().Assembly;// dll이 사용하는 모든 정보를 가져옴
            //현재 프로젝트에서 CommandClass로 정의된 모든 클래스를 가져온다.
            var types = from type in assembly.GetTypes()
                        where Attribute.IsDefined(type, typeof(CommandClassAttribute)) //CommandClassAttribute 되어 있는 클래스만 가져옴
                        select type;
            //함수와 프로토콜을 맵핑하여 준다.
            foreach(var target in types) //클래스로 부터 타겟매칭
            {
                RegisterFromClass(target);
            }
        }

        private void RegisterFromClass(Type target)
        {
            foreach(MethodInfo method in target.GetMethods())  //타겟 클래스에 있는  메소드 다 가져옴
            {
                object[] attr = method.GetCustomAttributes(typeof(CommandAttribute), true); // true는 상속된 것도 다 가져옴
                if (attr.Length == 0)
                    continue;
                //attr[0]
                //CommandAttribute cmdAttr = (CommandAttribute)attr[0];

                TargetInfo info = new TargetInfo();  // 프로토콜과 함수 매칭이 필요함
                info.command = (Command)Activator.CreateInstance(target); //Type매개변수를 받아서 생성을 해줌, CommandClassAttribute를 가지고 있는 클래스
                info.method = method;
                info.parameterType = method.GetParameters()[0].ParameterType; //Auth(ReqAuth req)-> ReqAuth 정보를 가져옴

                //프로토콜 클래스의 속성 목록을 가져온다.
                var attrs = info.parameterType.GetCustomAttributes(typeof(ProtocolAttribute),(true));
                //속성 목록에서 첫번쨰 속성 정보를 가져온다.
                ProtocolAttribute protocol =(ProtocolAttribute) attrs[0];
                
                dicProtocol.Add(protocol.protocol, info); //


            }
        }
    }

    internal class TargetInfo
    {
        public Command command;
        public MethodInfo method;
        public Type parameterType; //Method의 첫번째 파라미터 클래스 정보 - Attribute로 프로토콜 Key Value Match함
    }
}
