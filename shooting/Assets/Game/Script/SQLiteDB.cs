using System;
using System.Collections.Generic;
//using System.Data;
using System.IO;
using System.Linq;
using System.Text;

#if SERVER_SIDE
[ServiceStack.DataAnnotations.EnumAsInt]
#endif
public enum LANGUAGE_TYPE
{
    None,
    KOR,
    ENG,
    CN,
    TW,
    HK,
    JPN,
    VI,
    TH,
    AR,
    MAX,
}

public class LocalTableLocale
{
    private static LANGUAGE_TYPE type = LANGUAGE_TYPE.KOR;

    public static LANGUAGE_TYPE Locale
    {
        get
        {
            return type;
        }
    }

    public static void SetLocale(LANGUAGE_TYPE type)
    {
        LocalTableLocale.type = type;
    }
}

public interface ISQLiteLoader
{
    bool Load(String filename);
    void Unload();

    SQLiteQuery Query(String query);
}

public interface ISQLiteQuery
{
    String GetString(String field);
    Int32 GetInt32(String field);
    Double GetDouble(String field);
    bool IsNULL(String feld);
    bool Step();
    void Release();
}

public interface ILocalTable
{
    void Create(SQLiteQuery dataReader);
}

public class SqliteDB<T> where T : ILocalTable
{
    public List<T> ReadAll(ISQLiteLoader con, String columns, String name)
    {
        List<T> list = new List<T>();

        SQLiteQuery query = con.Query(String.Format("SELECT {0} FROM {1}", columns, name));
        while (query.Step())
        {
            var ins = Activator.CreateInstance<T>();
            ins.Create(query);
            list.Add(ins);
        }

        return list;
    }

    public virtual void OnLoad(List<T> list)
    {

    }

    protected virtual String GetTableName()
    {
        return "";
    }

    protected virtual int Id()
    {
        return 0;
    }

    protected virtual bool IsLocale()
    {
        return false;
    }

    public String ToString(SQLiteQuery reader, String name)
    {
        if (reader.IsNULL(name))
            return "";

        return reader.GetString(name);
    }

    public Int32 ToInt32(SQLiteQuery reader, String name)
    {
        int value = 0;
        try
        {
            if (reader.IsNULL(name))
                value = 0;
            else
                value = reader.GetInteger(name);
        } catch(Exception e)
        {
        }

        return value;
    }

    public UInt32 ToUInt32(SQLiteQuery reader, String name)
    {
        return (uint)reader.GetInteger(name);
    }

    public float ToFloat(SQLiteQuery reader, String name)
    {
        return (float)reader.GetDouble(name);
    }

    public T2 ToEnum<T2>(SQLiteQuery reader, String name)
    {
        return (T2)Enum.Parse(typeof(T2), reader.GetString(name));
    }

    public byte ToByte(SQLiteQuery reader, String name)
    {
        return (byte)reader.GetInteger(name);
    }

    public Boolean ToBoolean(SQLiteQuery reader, String name)
    {
        if (reader.GetInteger(name) == 1)
            return true;

        return false;
    }

    public List<TT> ToList<TT>(SQLiteQuery reader, String name)
    {
        String value = reader.GetString(name);
        if (value == "NONE")
            value = "";

        if (string.IsNullOrEmpty(value))
            return null;

        var list = ArrayUInt32Converter(value);

        List<TT> data = new List<TT>();
        foreach (var row in list)
        {
            object v = row;
            data.Add((TT)v);
        }

        return data;
    }

    public List<int> ToListInt32(SQLiteQuery reader, String name)
    {
        String value = reader.GetString(name);
        if (value == "NONE")
            value = "";
        return ArrayUInt32Converter(value);
    }

    public List<String> ToListString(SQLiteQuery reader, String name)
    {
        String value = reader.GetString(name);
        if (value == "NONE")
            value = "";
        return ArrayStringConverter(value);
    }

    public List<float> ToListSingle(SQLiteQuery reader, String name)
    {
        String value = reader.GetString(name);
        if (value == "NONE")
            value = "";
        return ArrayFloatConverter(value);
    }

    public static List<int> ArrayUInt32Converter(string text)
    {
        string temp = text.Replace(" ", "");

        if (temp.Length <= 0 || temp.Contains("NONE"))
            return null;

        if ('[' == temp[0])
            temp = temp.Remove(0, 1);

        if (']' == temp[temp.Length - 1])
            temp = temp.Remove(temp.Length - 1, 1);

        string[] arrData = temp.Split(',');

        List<int> info = new List<int>();
        for (int i = 0; i < arrData.Length; ++i)
        {
            info.Add(Convert.ToInt32(arrData[i]));
        }

        return info;
    }

    public static List<float> ArrayFloatConverter(string text)
    {
        string temp = text.Replace(" ", "");

        if (temp.Length <= 0)
            return null;

        if ('[' == temp[0])
            temp = temp.Remove(0, 1);

        if (']' == temp[temp.Length - 1])
            temp = temp.Remove(temp.Length - 1, 1);

        string[] arrData = temp.Split(',');

        List<float> info = new List<float>();
        for (int i = 0; i < arrData.Length; ++i)
        {
            info.Add(Convert.ToSingle(arrData[i]));
        }

        return info;
    }

    public static List<string> ArrayStringConverter(string param)
    {
        // 공백 제거
        string temp = param.Replace(" ", "");

        if (temp.Length <= 0)
            return null;

        // 첫번째/마지막 괄호 제거
        if ('[' == temp[0])
            temp = temp.Remove(0, 1);

        if (']' == temp[temp.Length - 1])
            temp = temp.Remove(temp.Length - 1, 1);


        return temp.Split(',').ToList();
    }
}
