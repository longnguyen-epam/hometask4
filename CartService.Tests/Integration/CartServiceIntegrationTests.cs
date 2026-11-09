using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using CartService.BLL;
using CartService.DAL;
using CartService.Entities;
using Shouldly;

namespace CartService.Tests.Integration
{
    public sealed class CartServiceIntegrationTests : IDisposable
    {
        private readonly LiteDatabase _db = new(new MemoryStream());
        private readonly CartService.BLL.CartService _service;

        public CartServiceIntegrationTests() => _service = new(new CartRepository(_db));

        public void Dispose() => _db.Dispose();

        [Fact]
        public void Added_items_are_persisted_and_can_be_read_back()
        {
            _service.AddItem("cart-1", new CartItem
            {
                Id = 1,
                Name = "Book",
                Price = 9.99m,
                Quantity = 2,
                ImageUrl = "https://img.example.com/book.png",
                ImageAltText = "Book cover",
            });
            _service.AddItem("cart-1", new CartItem { Id = 2, Name = "Pen", Price = 1.5m, Quantity = 1 });

            var items = _service.GetItems("cart-1");

            items.Count.ShouldBe(2);
            items[0].Price.ShouldBe(9.99m);
            items[0].ImageAltText.ShouldBe("Book cover");
        }

        [Fact]
        public void Removed_item_is_gone_after_reload()
        {
            _service.AddItem("cart-1", new CartItem { Id = 1, Name = "Book", Price = 10m, Quantity = 1 });

            _service.RemoveItem("cart-1", 1);

            _service.GetItems("cart-1").ShouldBeEmpty();
        }

        [Fact]
        public void Carts_are_isolated_by_id()
        {
            _service.AddItem("cart-1", new CartItem { Id = 1, Name = "Book", Price = 10m, Quantity = 1 });

            _service.GetItems("cart-2").ShouldBeEmpty();
        }
    }

}
