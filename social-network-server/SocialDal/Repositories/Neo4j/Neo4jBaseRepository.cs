﻿using Neo4j.Driver.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
public abstract class Neo4jBaseRepository
{
    protected IDriver _driver;
    public Neo4jBaseRepository()
    {
        string neo4jUrl = ConfigurationManager.AppSettings["neo4jUrl"];
        string neo4jUn = ConfigurationManager.AppSettings["neo4jUn"];
        string neo4jPass = ConfigurationManager.AppSettings["neo4jPass"];
        _driver = GraphDatabase.Driver(neo4jUrl, AuthTokens.Basic(neo4jUn, neo4jPass));
    }

    ~Neo4jBaseRepository()
    {
        _driver.Dispose();
    }

    protected IStatementResult Query(string query)
    {
        using (ISession session = _driver.Session())
        {
            return session.Run(query);
           
        }

    }

    protected void Create(object obj)
    {
        string type = obj.GetType().Name;
        string json = JsonConvert.SerializeObject(obj);
        string query = $"create(p:{type}{json})";
        Query(query);
    }

    protected static List<T> RecordsToList<T> (IEnumerable<IRecord> records) where T : new()
    {
        var list = new List<T>();
        foreach(IRecord record in records)
        {
            list.Add(RecordToObject<T>(record));
        }
        return list;
    }

    protected static T RecordToObject<T>(IRecord record) where T : new()
    {
        var values = record.Values;
        Type type = typeof(T);
        T obj = new T();
        foreach(var value in values)
        {
            var prop = type.GetProperty(value.Key);
            if (prop != null)
            {
                var castedValue = Convert.ChangeType(value.Value, prop.PropertyType);
                if (prop.CanWrite)
                {
                    prop.SetValue(obj, castedValue);
                }
            }
            else
            {
                FlatenObject(type, obj, value, prop);
            }
        }
        return obj;
    }

    protected static void FlatenObject<T>(Type type, T obj, KeyValuePair<string, object> value, System.Reflection.PropertyInfo prop) where T : new()
    {
        var srcObj = value.Value;
        foreach (var srcProp in srcObj.GetType().GetProperties())
        {
            var destProp = type.GetProperty(srcProp.Name);
            var castedValue = Convert.ChangeType(srcProp.GetValue(srcObj), destProp.PropertyType);
            if (destProp != null && prop.CanWrite)
            {
                destProp.SetValue(obj, castedValue);
            }
        }
    }
}
