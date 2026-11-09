using CartService.DAL;
using CartService.Entities;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Tests.Unit
{
    public class CartServiceTests
    {
        private readonly ICartRepository _repository = Substitute.For<ICartRepository>();
        private readonly CartService.BLL.CartService _service;

        public CartServiceTests() => _service = new (_repository);

        private static CartItem Item(int id = 1, int quantity = 1) =>
            new() { Id = id, Name = "Book", Price = 10m, Quantity = quantity };

        [Fact]
        public void GetItems_returns_empty_list_when_cart_does_not_exist()
        {
            _service.GetItems("cart-1").ShouldBeEmpty();
        }

        [Fact]
        public void AddItem_creates_cart_when_it_does_not_exist()
        {
            _service.AddItem("cart-1", Item());

            _repository.Received(1).Save(Arg.Is<Cart>(c => c.Id == "cart-1" && c.Items.Count == 1));
        }

        [Fact]
        public void AddItem_increases_quantity_when_item_is_already_in_cart()
        {
            var cart = new Cart { Id = "cart-1", Items = [Item(quantity: 2)] };
            _repository.Get("cart-1").Returns(cart);

            _service.AddItem("cart-1", Item(quantity: 3));

            cart.Items.ShouldHaveSingleItem().Quantity.ShouldBe(5);
            _repository.Received(1).Save(cart);
        }

        [Theory]
        [InlineData(0, "Book", 1, 1)]
        [InlineData(1, " ", 1, 1)]
        [InlineData(1, "Book", -1, 1)]
        [InlineData(1, "Book", 1, 0)]
        public void AddItem_rejects_invalid_item(int id, string name, decimal price, int quantity)
        {
            var item = new CartItem { Id = id, Name = name, Price = price, Quantity = quantity };

            Should.Throw<ArgumentException>(() => _service.AddItem("cart-1", item));
            _repository.DidNotReceiveWithAnyArgs().Save(default!);
        }

        [Fact]
        public void RemoveItem_removes_item_and_saves_cart()
        {
            var cart = new Cart { Id = "cart-1", Items = [Item(1), Item(2)] };
            _repository.Get("cart-1").Returns(cart);

            _service.RemoveItem("cart-1", 1);

            cart.Items.ShouldHaveSingleItem().Id.ShouldBe(2);
            _repository.Received(1).Save(cart);
        }
    }
}
