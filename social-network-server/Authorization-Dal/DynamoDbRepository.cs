﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Authorization_Common.Models;

namespace Authorization_Dal
{
    public class DynamoDbRepository<T>
    {
        private DynamoDBContext _DbContext;

        public DynamoDbRepository()
        {
            var DynamoClient = new AmazonDynamoDBClient();
            _DbContext = new DynamoDBContext(DynamoClient, new DynamoDBContextConfig
            {
                //Setting the Consistent property to true ensures that you'll always get the latest 
                ConsistentRead = true,
                SkipVersionCheck = true
            });
        }

        ~DynamoDbRepository()
        {
            _DbContext.Dispose();
        }



        public T Add(T item)
        {
            T savedItem = _DbContext.Load(item);
            if (savedItem != null)
            {
                return default(T);
            }
            _DbContext.Save<T>(item);
            return _DbContext.Load(item);
        }

        public T Update(T item)
        {
            T savedItem = _DbContext.Load(item);

            if (savedItem == null)
            {
                return default(T);
            }
            _DbContext.Save(item);
            return _DbContext.Load(item);
        }

        public T Get(string id)
        {
            return _DbContext.Load<T>(id);
        }
    }
}