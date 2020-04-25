using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using ServiceStack.OrmLite;
using static Tutorial_Server.ResAuth;

namespace Tutorial_Server
{
    public class AccountData
    {
        public int idx { get; set; }
        public string accountKey { get; set; }
    }

    public class t_account
    {
        public int idx { get; set; }
        public string accountKey { get; set; }
        public DateTime loginTime { get; set; }
        public AuthType authType { get; set; }   //테이블에 속성을 만들때는 TinyInt(4) 정도로 만든다.
    }

    public class t_inventory
    {
        public int idx { get; set; }
        public int itemID { get; set; }
        public int count { get; set; }
        public int opetion { get; set; }
        public int accountIdx { get; set; }
    }
    public class t_item
    {
        public int idx { get; set; }
        public int itemID { get; set; }
        public int option { get; set; }
    }

    public class Command
    {
        protected CommonResponse Response(ResultCode result)
        {
            CommonResponse response = new CommonResponse();
            response.result = result;
            return response;
        }
        protected CommonResponse Response(CommonResponse res)
        {
            res.result = ResultCode.OK;
            return res;
        }

    }
    [CommandClass("인증")]
    public class AuthCommand : Command
    {


        [Command("인증 시작")]
        public CommonResponse Auth(ReqAuth req)
        {

            List<DataInventory> invertoryList = new List<DataInventory>();
            int idxAccount = 0;
            //using (var db = ORMContext.Open())
            //{  // 속성으로 만든 클래스가 있어야함
            //    var data = db.Single<AccountData>(w => w.accountKey == req.accountKey);
            //    idxAccount = data.idx;
            //}

            using (var db = ORMContext.Open())
            {
                using (var tran = db.OpenTransaction()) //Commit을 하기 전에는 RollBack 저리가 된다. //트랜젝션을 OPen하는 순간 Commit을 안하면 무조건 RollBack이다.
                {
                    var data = db.Single<t_account>(w => w.accountKey == req.accountKey);
                    if (data == null)
                        return Response(ResultCode.Error);
                    idxAccount = data.idx;

                    var rows = db.Select<t_inventory>(w => w.idx == idxAccount);

                    db.UpdateOnly(() => new t_account { loginTime = DateTime.UtcNow }, w => w.idx == idxAccount);  //CTRL +R 두번 누르면 다 바뀜 , 세계 서비스 제공시 UtcNow로 하는게 좋음
                    var format = $"UPDATE new_table set loginTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' Where idx = {idxAccount}";
                    //tran.Rollback();
                    tran.Commit();
                }


                //db.ExecuteNonQuery(format);
            }
            var res = new ResAuth();
            res.accessKey = "werwerwerwerwerwer";
            res.sessionKey = RedisProvider.CreateSession(idxAccount);
            res.success = true;
            res.inventory = invertoryList;
            return Response(res);
        }
        [Command("Account Creatation")]
        public CommonResponse AccountCreation(ReqAccountCreation req)
        {
            ResAccountCreation res = new ResAccountCreation();
            using(var db = ORMContext.Open())
            {
                if (db.Exists<t_account>(w => w.accountKey == req.accountKey))
                    return Response(ResultCode.Error);

                long lastKey = 0;
                if(req.authType == AuthType.GameCenter || req.authType == AuthType.Google)
                {
                    t_account acc = new t_account();
                    acc.accountKey = req.accountKey;
                    acc.loginTime = DateTime.UtcNow;
                    acc.authType = req.authType;
                    //lastKey = db.Insert<t_account>(acc);
                    lastKey = db.Insert(acc);
                }
                else if(req.authType == AuthType.Guest)
                {
                    t_account acc = new t_account();
                    var guid = Guid.NewGuid();
                    acc.accountKey = guid.ToString();
                    acc.loginTime = DateTime.UtcNow;
                    acc.authType = req.authType;
                    lastKey = db.Insert<t_account>(acc);
                }
                RedisProvider.CreateSession((int)lastKey);
            }
            //req.accountKey; //클리이언트가 Access 키를 넘겨줌
            //req.authType;

            //1. 해당키가 존재하는지 확인
            // 1-1 해당키가 존재하면 에러
            // 2 해당키를 가지고 데이터베이스에 데이터를 생성
            // 2-1 Guest 인증의 경우 키를 생성할것

            return Response(ResultCode.OK);
        }

        public CommonResponse ItemGet(ReqItemGet req) //세션키를 이용해서 아이템을 받아옴
        {
            int accountID = Convert.ToInt32(RedisProvider.GetSession(req.sessionKey));
            using (var db = DatabaseContext.Open())
            {
                using (var read = db.ExecuteQuery($"Select * from t_inventory Where accountIDx = {accountID}"))
                {

                }

                // "SELECT * FROM t_inventory a Join t_time b ON a.itemID = b.itemID";

            }
            //using (var db = ORMContext.Open())
            //{
            //    var query = db.From<t_inventory>().Join<t_item>((x, y) => x.itemID == y.itemID).Where(w => w.accountIdx == accountID);
            //    var rows = db.Select(query);

            //    List<int> list = new List<int> { 101,102};
            //    db.Select<t_inventory>
            //}



            return Response(ResultCode.OK);
        }

    }


}
