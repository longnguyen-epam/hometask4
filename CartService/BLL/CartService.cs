using CartService.DAL;
using CartService.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.BLL
{
    public class CartService(ICartRepository repository)
    {
        public IReadOnlyList<CartItem> GetItems(string cartId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cartId);
            return repository.Get(cartId)?.Items ?? [];
        }

        public void AddItem(string cartId, CartItem item)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cartId);
            Validate(item);

            var cart = repository.Get(cartId) ?? new Cart { Id = cartId };
            var existing = cart.Items.Find(i => i.Id == item.Id);
            if (existing is null)
                cart.Items.Add(item);
            else
                existing.Quantity += item.Quantity;

            repository.Save(cart);
        }

        public void RemoveItem(string cartId, int itemId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cartId);

            var cart = repository.Get(cartId);
            if (cart is not null && cart.Items.RemoveAll(i => i.Id == itemId) > 0)
                repository.Save(cart);
        }

        private static void Validate(CartItem item)
        {
            if (item.Id <= 0) throw new ArgumentException("Item id must be positive.", nameof(item));
            if (string.IsNullOrWhiteSpace(item.Name)) throw new ArgumentException("Item name is required.", nameof(item));
            if (item.Price < 0) throw new ArgumentException("Price cannot be negative.", nameof(item));
            if (item.Quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(item));
            if (item.ImageUrl is not null && !Uri.IsWellFormedUriString(item.ImageUrl, UriKind.Absolute))
                throw new ArgumentException("Image URL must be absolute.", nameof(item));
        }
    }
}
