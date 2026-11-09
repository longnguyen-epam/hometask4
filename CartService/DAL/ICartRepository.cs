using CartService.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.DAL
{
    public interface ICartRepository
    {
        Cart? Get(string cartId);
        void Save(Cart cart);
    }
}
