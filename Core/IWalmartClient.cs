using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Walmart.SDK.Entities;

namespace Walmart.SDK.Core
{
    public interface IWalmartClient
    {
        void GetCategories(Func<Category, bool> callback);
        void GetProducts(string categoryId, Func<Product, bool> callback);
        bool GetProduct(string itemId, Action<Product> callback);
    }
}
