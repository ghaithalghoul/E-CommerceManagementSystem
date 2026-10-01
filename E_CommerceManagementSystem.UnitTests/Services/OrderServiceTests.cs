using E_CommerceManagementSystem.Dto.Order;
using E_CommerceManagementSystem.Migrations;
using E_CommerceManagementSystem.Models;
using E_CommerceManagementSystem.Services;
using E_CommerceManagementSystem.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cart = E_CommerceManagementSystem.Models.Cart;

namespace E_CommerceManagementSystem.UnitTests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateOrder_WhenCartIsValid_ReturnsOrder()
        {
            await using var dbContext = TestDbContextFactory.Create();

            var cart = new Cart
            {
                Id = 1,
                UserId = 1,
                CartItems = new List<CartItem>
        {
            new CartItem
            {
                Id = 1,
                CartId = 1,
                ProductId = 1,
                Quantity = 2,
                Price = 200
            },
            new CartItem
            {
                Id = 2,
                CartId = 1,
                ProductId = 2,
                Quantity = 3,
                Price = 50
            }
        }
            };

            var products = new List<Product>
    {
        new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Description = "New Laptop",
            Price = 200,
            stock = 20,
            CategoryId = 1
        },
        new Product
        {
            ProductId = 2,
            Name = "Phone",
            Description = "New Phone",
            Price = 50,
            stock = 20,
            CategoryId = 1
        }
    };

            dbContext.Carts.Add(cart);
            dbContext.Products.AddRange(products);

            await dbContext.SaveChangesAsync();

            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);

            var result = await service.CreateOrder(1);

            var order = await dbContext.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == 1);

            var savedProducts = await dbContext.Products
                .Where(x => x.ProductId == 1 || x.ProductId == 2)
                .ToListAsync();

            var remainingCartItems = await dbContext.CartItems
                .Where(x => x.CartId == 1)
                .ToListAsync();

            Assert.NotNull(result);
            Assert.NotNull(order);

            Assert.Equal(2, order.OrderItems.Count);
            Assert.Equal(OrderStatus.Pending, order.OrderStatus);

            Assert.Equal(550, order.TotalPrice);

            Assert.Equal(2, order.OrderItems
                .First(x => x.ProductId == 1).Quantity);

            Assert.Equal(3, order.OrderItems
                .First(x => x.ProductId == 2).Quantity);

            Assert.Equal(18, savedProducts
                .First(x => x.ProductId == 1).stock);

            Assert.Equal(17, savedProducts
                .First(x => x.ProductId == 2).stock);

            Assert.Empty(remainingCartItems);
        }
        [Fact]
        public async Task CreateOrder_WhenStockIsInsufficient_ReturnsNull()
        {
            await using var dbContext = TestDbContextFactory.Create();

            var cart = new Cart
            {
                Id = 1,
                UserId = 1,
                CartItems = new List<CartItem>
        {
            new CartItem
            {
                Id = 1,
                CartId = 1,
                ProductId = 1,
                Quantity = 2,
                Price = 200
            },
            new CartItem
            {
                Id = 2,
                CartId = 1,
                ProductId = 2,
                Quantity = 30,
                Price = 50
            }
        }
            };

            var products = new List<Product>
    {
        new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Description = "New Laptop",
            Price = 200,
            stock = 20,
            CategoryId = 1
        },
        new Product
        {
            ProductId = 2,
            Name = "Phone",
            Description = "New Phone",
            Price = 50,
            stock = 20,
            CategoryId = 1
        }
    };

            dbContext.Carts.Add(cart);
            dbContext.Products.AddRange(products);

            await dbContext.SaveChangesAsync();

            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var result = await service.CreateOrder(1);
            var order = await dbContext.Orders.FirstOrDefaultAsync();
            var product = await dbContext.Products
            .FirstAsync(x => x.ProductId == 2);

            Assert.Null(result);
            Assert.Null(order);
            Assert.Equal(20, product.stock);
        }
        [Fact]
        public async Task CreateOrder_WhenCartIsEmpty_ReturnsNulll()
        {
            await using var dbContext = TestDbContextFactory.Create();

            var cart = new Cart
            {
                Id = 1,
                UserId = 1,
                CartItems = new List<CartItem>()
        
            };

            

            dbContext.Carts.Add(cart);
            

            await dbContext.SaveChangesAsync();

            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var result = await service.CreateOrder(1);
            Assert.Null(result);
        }
        [Fact]
        public async Task CreateOrder_WhenProductDoesNotExist_ReturnsNull()
        {
            await using var dbContext = TestDbContextFactory.Create();

            var cart = new Cart
            {
                Id = 1,
                UserId = 1,
                CartItems = new List<CartItem>
        {
            new CartItem
            {
                Id = 1,
                CartId = 1,
                ProductId = 1,
                Quantity = 2,
                Price = 200
            },
            new CartItem
            {
                Id = 999,
                CartId = 1,
                ProductId = 2,
                Quantity = 1,
                Price = 50
            }
        }
            };

            var products = new List<Product>
    {
        new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Description = "New Laptop",
            Price = 200,
            stock = 20,
            CategoryId = 1
        }
    };

            dbContext.Carts.Add(cart);
            dbContext.Products.AddRange(products);

            await dbContext.SaveChangesAsync();

            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var result = await service.CreateOrder(1);
            var order = await dbContext.Orders.FirstOrDefaultAsync();
            Assert.Null(result);
            Assert.Null(order);
        }
        [Fact]
        public async Task CreateOrder_WhenCartDoesNotExist_ReturnsNull()
        {
            await using var dbContext = TestDbContextFactory.Create();

            

          

            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var result = await service.CreateOrder(1);
            var cart = await dbContext.Carts.FirstOrDefaultAsync();
            Assert.Null(result);
            Assert.Null(cart);
        }
        [Fact]
        public async Task GetAllOrders_WhenRequestValid_ReturnOrdersResponse()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var orders = new List<Order>
            {
                new Order { Id = 1,UserId = 1,OrderStatus= OrderStatus.Delivered,TotalPrice=400 },
                new Order { Id = 2,UserId = 1,OrderStatus= OrderStatus.Processing,TotalPrice=600 },
            };
            dbContext.Orders.AddRange(orders);
            await dbContext.SaveChangesAsync();
            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var result = await service.GetAllOrders(1);
            var userorders = await dbContext.Orders.Where(x => x.UserId == 1).ToListAsync();
            Assert.Equal(2, result.Count);
            Assert.Equal(2, userorders.Count);

        }
        [Fact]
        public async Task GetOrder_ValidRequest_ReturnOrder()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var orders = new List<Order>
            {
                new Order { Id = 1,UserId = 1,OrderStatus= OrderStatus.Delivered,TotalPrice=400 },
                new Order { Id = 2,UserId = 1,OrderStatus= OrderStatus.Processing,TotalPrice=600 },
            };
            dbContext.Orders.AddRange(orders);
            await dbContext.SaveChangesAsync();
            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var result = await service.GetOrder(1, 1);
            var existorder = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == 1 );
            Assert.NotNull(result);
            Assert.Equal(OrderStatus.Delivered, result.OrderStatus);
            Assert.Equal(400, result.TotalPrice);
            Assert.NotNull(existorder);
        }
        [Fact]
        public async Task GetOrder_WhenOrderDoesNotExist_ReturnNull()
        {
            await using var dbContext = TestDbContextFactory.Create();
            
            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var result = await service.GetOrder(1, 1);
            var existorder = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == 1 );
            Assert.Null(result);
            Assert.Null(existorder);
        }
        [Fact]
        public async Task CancelOrder_ValidRequst_ReturnOrder()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var order = new Order { Id = 1, UserId = 1, OrderStatus = OrderStatus.Processing, TotalPrice = 400 };
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var result = await service.CancelOrder(1, 1);
            var existorder = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Equal(OrderStatus.Cancelled, result.OrderStatus);
            Assert.NotNull(existorder);
            Assert.Equal(OrderStatus.Cancelled, existorder.OrderStatus);


        }
        [Fact]
        public async Task CancelOrder_WhenOrderStatusDoesNotAcceptCancalling_ReturnNull()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var order = new Order { Id = 1, UserId = 1, OrderStatus = OrderStatus.Shipped, TotalPrice = 400 };
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var result = await service.CancelOrder(1, 1);
            var existorder = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.Null(result);
            Assert.NotNull(existorder);
            Assert.Equal(OrderStatus.Shipped, existorder.OrderStatus);


        }
        [Fact]
        public async Task UpdateOrderStatus_ValidRequest_ReturnOrder()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var order = new Order { Id = 1, UserId = 1, OrderStatus = OrderStatus.Shipped, TotalPrice = 400 };
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
            var mockAuditService = new Mock<IAuditLogService>();
            var service = new OrderService(dbContext, mockAuditService.Object);
            var updaterequset = new UpdateOrderRequest { OrderId = 1, OrderStatus = OrderStatus.Delivered };
            var result = await service.UpdateOrderStatus(2, updaterequset);
            var existorder = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Equal(OrderStatus.Delivered, result.OrderStatus);
            Assert.NotNull(existorder);
            Assert.Equal(OrderStatus.Delivered, existorder.OrderStatus);

        }
    }
}
