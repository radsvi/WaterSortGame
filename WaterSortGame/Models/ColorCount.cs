using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace WaterSortGame.Models
{
    internal class ColorCount : IEnumerable<KeyValuePair<LiquidColorName, int>>
    {
        public Dictionary<LiquidColorName, int> data { get; set; } = new Dictionary<LiquidColorName, int>();

        //public LiquidColorName ColorName
        //{
        //    get
        //    {
        //        return 
        //    }
        //}
        //public int Count { get; }
        private bool ContainsKey(LiquidColorName key)
        {
            return this.data.ContainsKey(key);
        }
        public void AddColor(LiquidColorName colorName)
        {
            if (ContainsKey(colorName))
            {
                data[colorName]++;
            }
            else
            {
                data.Add(colorName, 1);
            }
        }
        public int GetCount(LiquidColorName colorName)
        {
            if (ContainsKey(colorName))
            {
                return data[colorName];
            }
            else
            {
                return -1;
            }
        }
        public void OrderDesc()
        {
            //data = data.OrderByDescending(x => x.Value);
            // chci aby to to byl ordered list, ale zaroven stale i dictionary:
            Dictionary<LiquidColorName, int> newData = [];
            IOrderedEnumerable<KeyValuePair<LiquidColorName, int>> orderedList = data.OrderByDescending(x => x.Value);
            foreach (var item in orderedList)
            {
                newData.Add(item.Key, item.Value);
            }
            data = newData;
        }

        public IEnumerator<KeyValuePair<LiquidColorName, int>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
