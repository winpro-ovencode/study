#if !SERVER_SIDE
using UnityEngine;
#endif
using System;
using System.IO;
using System.Text;

public class SQLiteQueryClient : ISQLiteQuery
{
    SQLiteQuery m_qr;

    public SQLiteQueryClient(SQLiteDB db, String query)
    {
        m_qr = new SQLiteQuery(db, query);
    }

    public bool Read()
    {
        return m_qr.Step();
    }

    public String GetString(String field)
    {
        return m_qr.GetString(field);
    }

    public Int32 GetInt32(String field)
    {
        return m_qr.GetInteger(field);
    }

    public Double GetDouble(String field)
    {
        return m_qr.GetDouble(field);
    }

    public bool IsNULL(String field)
    {
        return m_qr.IsNULL(field);
    }

    public bool Step()
    {
        return m_qr.Step();
    }

    public void Release()
    {
        m_qr.Release();
    }
}

public class SQLiteLoaderClient : ISQLiteLoader
{
    private SQLiteDB m_sqlite;

    public bool Load(Stream stream)
    {
        // load db from memory stream
        SQLiteDB db = new SQLiteDB();
        db.OpenStream("stream", stream);
        db.Key("0x0102030405060708090a0b0c0d0e0f10");
        m_sqlite = db;
        return true;
    }

    public bool Load(String filename)
    {
        StringBuilder logBuilder = new StringBuilder(512);
        byte[] bytes = null;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        string dbpath = "file://" + Application.dataPath + "/" + filename;

        logBuilder.AppendFormat("1asset path is: {0}\n", dbpath);
        WWW www = new WWW(dbpath);
        while (!www.isDone) { }
        if(www.error != null)
            Debug.LogError(www.error + dbpath);
        //yield return www;
        bytes = www.bytes;

#elif UNITY_WEBPLAYER
		string dbpath = "StreamingAssets/" + filename;
        logBuilder.AppendFormat("asset path is: {0}\n", dbpath);
		WWW www = new WWW(dbpath);
        while (!www.isDone) { }
		//yield return www;
		bytes = www.bytes;
#elif UNITY_IPHONE
		string dbpath = Application.dataPath + "/Raw/" + filename;
        Log.Debug("DataTable loading.. {0}", dbpath);

        logBuilder.AppendFormat("asset path is: {0}\n", dbpath);
		try
        {	
			using ( FileStream fs = new FileStream(dbpath, FileMode.Open, FileAccess.Read, FileShare.Read) )
            {
				bytes = new byte[fs.Length];
				fs.Read(bytes,0,(int)fs.Length);
			}			
		}
        catch (Exception e)
        {
            logBuilder.AppendFormat("Fail with Exception: {0}\n", e.ToString());
		}
#elif UNITY_ANDROID
		string dbpath = Application.dataPath + "/" + filename;
        logBuilder.AppendFormat("asset path is: {0}\n", dbpath);
		WWW www = new WWW(dbpath);
        while (!www.isDone) { }
		//yield return www;
		bytes = www.bytes;
#endif  // UNITY

        if (bytes != null)
        {
            // create memory stream for db
            MemoryStream memStream = new MemoryStream();
            memStream.Write(bytes, 0, bytes.Length);

            // load db from memory stream
            SQLiteDB db = new SQLiteDB();
            db.OpenStream("stream", memStream);
            db.Key("0x0102030405060708090a0b0c0d0e0f10");
            m_sqlite = db;
        }
        return true;
    }

    public bool Load(byte[] bytes)
    {
        if (bytes != null)
        {
            // create memory stream for db
            MemoryStream memStream = new MemoryStream();
            memStream.Write(bytes, 0, bytes.Length);

            // load db from memory stream
            SQLiteDB db = new SQLiteDB();
            db.OpenStream("stream", memStream);
            db.Key("0x0102030405060708090a0b0c0d0e0f10");
            m_sqlite = db;
        }
        return true;
    }

    public SQLiteQuery Query(String query)
    {
        return new SQLiteQuery(m_sqlite, query);
    }

    public void Unload()
    {
        m_sqlite.Close();
    }
}