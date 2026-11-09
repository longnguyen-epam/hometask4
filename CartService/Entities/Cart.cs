using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Entities
{
    public class Cart
    {
        public string Id { get; set; } = "";
        public List<CartItem> Items { get; set; } = [];
    }
}
