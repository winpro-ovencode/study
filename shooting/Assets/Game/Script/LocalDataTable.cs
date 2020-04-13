using System;
using System.Collections;
using System.Collections.Generic;
#if !SERVER_SIDE
using UnityEngine;
#endif


public partial class DataEnemy : SqliteDB<DataEnemy>, ILocalTable
{

    private static string _ColumnNames = "[id],[name],[image],[hp]";
    public static string _QuotedTableName = "Enemy";
    private static List<DataEnemy> cashedList = new List<DataEnemy>();

    public Int32 id { get; set; }
    public String name { get; set; }
    public String image { get; set; }
    public Int32 hp { get; set; }

    public override string ToString()
    {
          string text = "DataEnemy[";
          text += "id=";
          text += this.id.ToString();
          text += ", ";
          text += "name=";
          if(this.name!=null) text += this.name;
          text += ", ";
          text += "image=";
          if(this.image!=null) text += this.image;
          text += "]";
          return text;
    }

    static public void LoadTable(ISQLiteLoader con)
    {
        Clear();
        DataEnemy table = new DataEnemy();
        cashedList = table.ReadAll(con, _ColumnNames, _QuotedTableName);

    }

    static public List<DataEnemy> GetAll()
    {
        return cashedList;
    }

    protected override bool IsLocale()
    {
        return false;
    }

    protected override string GetTableName()
    {
        return _QuotedTableName;
    }

    public void Create(SQLiteQuery dataReader)
    {

        id = ToInt32(dataReader, "id");
        name = ToString(dataReader, "name");
        image = ToString(dataReader, "image");
        hp = ToInt32(dataReader, "hp");
    }

    static public void Clear()
    {
        cashedList.Clear();

    }
    static public void LoadComplete()
    {
        DataEnemy dummy = new DataEnemy();
        dummy.OnLoad(cashedList);
    }
}


public partial class DataLevel : SqliteDB<DataLevel>, ILocalTable
{

    private static string _ColumnNames = "[id],[distance],[backgroundSpeed],[enemySpeed],[enemyDelay]";
    public static string _QuotedTableName = "t_Level";
    private static List<DataLevel> cashedList = new List<DataLevel>();

    public Int32 id { get; set; }
    public Int32 distance { get; set; }
    public Single backgroundSpeed { get; set; }
    public Single enemySpeed { get; set; }
    public Single enemyDelay { get; set; }

    public override string ToString()
    {
          string text = "DataLevel[";
          text += "id=";
          text += this.id.ToString();
          text += ", ";
          text += "distance=";
          text += this.distance.ToString();
          text += ", ";
          text += "backgroundSpeed=";
          text += this.backgroundSpeed.ToString();
          text += "]";
          return text;
    }

    static public void LoadTable(ISQLiteLoader con)
    {
        Clear();
        DataLevel table = new DataLevel();
        cashedList = table.ReadAll(con, _ColumnNames, _QuotedTableName);

    }

    static public List<DataLevel> GetAll()
    {
        return cashedList;
    }

    protected override bool IsLocale()
    {
        return false;
    }

    protected override string GetTableName()
    {
        return _QuotedTableName;
    }

    public void Create(SQLiteQuery dataReader)
    {

        id = ToInt32(dataReader, "id");
        distance = ToInt32(dataReader, "distance");
        backgroundSpeed = ToFloat(dataReader, "backgroundSpeed");
        enemySpeed = ToFloat(dataReader, "enemySpeed");
        enemyDelay = ToFloat(dataReader, "enemyDelay");
    }

    static public void Clear()
    {
        cashedList.Clear();

    }
    static public void LoadComplete()
    {
        DataLevel dummy = new DataLevel();
        dummy.OnLoad(cashedList);
    }
}
[System.Serializable]
public class LocalDataTable
{
	static public bool g_isLoaded = false;

	static public void LoadAll(ISQLiteLoader loader, int version = 99)
	{

		DataEnemy.LoadTable(loader);
		DataLevel.LoadTable(loader);
	}
#if !SERVER_SIDE
	static public IEnumerator LoadAllAsync(ISQLiteLoader loader, int version = 99)
	{

		DataEnemy.LoadTable(loader);
		yield return null;
		DataLevel.LoadTable(loader);
		yield return null;
		g_isLoaded = true;
	}
#endif
	static public void LoadComplete()
	{
		DataEnemy.LoadComplete();
		DataLevel.LoadComplete();
	}
}
namespace Eppy
{
  public static class Tuple
  {
      public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 second)
      {
          return new Tuple<T1, T2>(item1, second);
      }
      public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 second, T3 third)
      {
          return new Tuple<T1, T2, T3>(item1, second, third);
      }
  }
  public sealed class Tuple<T1, T2>
  {
      private T1 item1;
      private T2 item2;
      public T1 Item1
      {
          get { return item1; }
          set { item1 = value; }
      }
      public T2 Item2
      {
          get { return item2; }
          set { item2 = value; }
      }
      public Tuple(T1 item1, T2 item2)
      {
          this.item1 = item1;
          this.item2 = item2;
      }
      public override string ToString()
      {
          return string.Format("Tuple({0}, {1})", Item1, Item2);
      }
      public override int GetHashCode()
      {
          int hash = 17;
          hash = hash * 23 + item1.GetHashCode();
          hash = hash * 23 + item2.GetHashCode();
          return hash;
      }
      public override bool Equals(object o)
      {
        if (o.GetType() != typeof(Tuple<T1, T2>))
        {
            return false;
        }
        var other = (Tuple<T1, T2>)o;
        return this == other;
      }
      public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
      {
          return a.item1.Equals(b.item1) && a.item2.Equals(b.item2);
      }
      public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
      {
          return !(a == b);
      }
  }
      public sealed class Tuple<T1, T2, T3>
      {
      private readonly T1 item1;
      private readonly T2 item2;
      private readonly T3 item3;
      public T1 Item1
      {
          get { return item1; }
      }
      public T2 Item2
      {
          get { return item2; }
      }
      public T3 Item3
      {
          get { return item3; }
      }
      public Tuple(T1 item1, T2 item2, T3 item3)
      {
          this.item1 = item1;
          this.item2 = item2;
          this.item3 = item3;
      }
      public override int GetHashCode()
      {
          int hash = 17;
          hash = hash * 23 + (item1 == null ? 0 : item1.GetHashCode());
          hash = hash * 23 + (item2 == null ? 0 : item2.GetHashCode());
          hash = hash * 23 + (item3 == null ? 0 : item3.GetHashCode());
          return hash;
      }
      public override bool Equals(object o)
      {
          if (!(o is Tuple<T1, T2, T3>)) {
              return false;
          }

          var other = (Tuple<T1, T2, T3>)o;

          return this == other;
      }
      
      public static bool operator==(Tuple<T1, T2, T3> a, Tuple<T1, T2, T3> b)
      {
          if (object.ReferenceEquals(a, null)) {
              return object.ReferenceEquals(b, null);
          }
          if (a.item1 == null && b.item1 != null) return false;
          if (a.item2 == null && b.item2 != null) return false;
          if (a.item3 == null && b.item3 != null) return false;
          return
              a.item1.Equals(b.item1) &&
              a.item2.Equals(b.item2) &&
              a.item3.Equals(b.item3);
      }
      public static bool operator!=(Tuple<T1, T2, T3> a, Tuple<T1, T2, T3> b)
      {
          return !(a == b);
      }
      
      public void Unpack(Action<T1, T2, T3> unpackerDelegate)
      {
          unpackerDelegate(Item1, Item2, Item3);
      }
  }
}
