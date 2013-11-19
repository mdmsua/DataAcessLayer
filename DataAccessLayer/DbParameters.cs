using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataAccessLayer.Core
{
    public sealed class DbParameters : IReadOnlyDictionary<string, object>
    {
        private readonly int _capacity;

        private readonly IDictionary<string, object> _dictionary;

        #region ctor
        public DbParameters(int capacity)
        {
            _capacity = capacity;
            _dictionary = new Dictionary<string, object>(capacity);
        } 
        #endregion

        #region IReadOnlyDictionary<string, object>
        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public IEnumerable<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public bool TryGetValue(string key, out object value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public IEnumerable<object> Values
        {
            get { return _dictionary.Values; }
        }

        public object this[string key]
        {
            get { return _dictionary[key]; }
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        } 
        #endregion

        public static DbParameters Create(int capacity)
        {
            return new DbParameters(capacity);
        }

        public static DbParameters From<T>(T entity)
        {
            var properties = entity.GetType().GetProperties().Where(p => p.CanRead).ToList();
            var parameters = Create(properties.Count);
            properties.ForEach(property => TrySetParameter(parameters, property, entity));
            return parameters;
        }

        public DbParameters Set(string key, object value)
        {
            if (ContainsKey(key))
                _dictionary.Add(key, value);    // throws ArgumentException to prevent overwriting of existing entry
            if (Count == _capacity)
                throw new IndexOutOfRangeException(string.Format("Maximum capacity is {0}", _capacity));
            _dictionary[key] = value;
            return this;
        }

        private static bool TrySetParameter<T>(DbParameters parameters, PropertyInfo property, T value)
        {
            try
            {
                parameters.Set(property.Name, property.GetValue(value));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
