using CartService.Entities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.DAL
{
    public class CartRepository(ILiteDatabase db) : ICartRepository
    {
        private ILiteCollection<Cart> Carts => db.GetCollection<Cart>("carts");

        public Cart? Get(string cartId) => Carts.FindById(cartId);

        public void Save(Cart cart) => Carts.Upsert(cart);
    }
}
