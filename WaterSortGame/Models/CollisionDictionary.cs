using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WaterSortGame.Models
{
    internal class CollisionDictionary<TKey, TValue> //: IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, List<TValue>> data = new Dictionary<TKey, List<TValue>>();
        public bool ContainsKey(TKey key)
        {
            return this.data.ContainsKey(key);
        }
        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                data[key].Add(value);
            }
            else
            {
                data.Add(key, new List<TValue>() { value });
            }
        }
        public List<TValue> this[TKey key]
        {
            get => data[key];
        }

        public Dictionary<TKey, List<TValue>> DebugData
        {
            get { return data; }
        }
    }
}