using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walmart.SDK.Entities
{
    public class Category : WalmartEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<Category> Children { get; set; }
    }
}
