using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Products;
using E_CommerceManagementSystem.Models;
using E_CommerceManagementSystem.Services;
using E_CommerceManagementSystem.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceManagementSystem.UnitTests.Services
{
    public class ProductServiceTests 
    {
        
        [Fact]
        public async Task GetAllProduct_WhenProductsExist_ReturnsAllProducts()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var products = new List<Product>
            {
                new Product { ProductId = 1,Name="Laptop",Description ="new laptop",Price=200,stock=20,CategoryId=1 },
                new Product { ProductId = 2,Name="iPhone",Description ="new mobile",Price=400,stock=10,CategoryId=1 },
                new Product { ProductId = 3,Name="LG TV",Description ="new smart scrren",Price=50,stock=5,CategoryId=1 },
            };
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
            var mockAuditLogService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAuditLogService.Object);

            var result = await service.GetAllProduct();

            Assert.Equal(3, result.Count);
            Assert.Contains(result, x => x.Name == "Laptop");
            Assert.Contains(result, x => x.Name == "iPhone");
            Assert.Contains(result, x => x.Name == "LG TV");
        }
        [Fact]
        public async Task GetAllProduct_WhenNoProducts_ReturnsEmptyList()
        {
            await using var dbcontext = TestDbContextFactory.Create();
            var products = new List<Product>();
            dbcontext.Products.AddRange(products);
            await dbcontext.SaveChangesAsync();
            var mockAuditLogService = new Mock<IAuditLogService>();
            var service =  new ProductsService(dbcontext, mockAuditLogService.Object);
            var result = await service.GetAllProduct();
            Assert.Empty(result);
            

        }
        [Fact]
        public async Task GetProduct_WhenProductExist_ReturnProduct()
        {
            await using var dbcontext = TestDbContextFactory.Create();
            var product = new Product { ProductId = 1, Name = "Laptop", Description = "new laptop", Price = 200, stock = 20, CategoryId = 1 };
            dbcontext.Products.Add(product);
            await dbcontext.SaveChangesAsync();
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbcontext,mockAudithService.Object);
            var result = await service.GetProduct(1);
            Assert.NotNull(result);
            Assert.Equal("Laptop",result.Name);

        }
        [Fact]
        public async Task GetProduct_WhenProductNotExist_Returnnull()
        {
            await using var dbcontext = TestDbContextFactory.Create();
            
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbcontext, mockAudithService.Object);
            var result = await service.GetProduct(1);
            
            Assert.Null(result);

        }
        [Fact]
        public async Task AddProduct_WhenRequestValid_ReturnProduct()
        {
            await using var dbcontext = TestDbContextFactory.Create();
            var product = new CreateProductRequest {  Name = "Laptop", Description = "new laptop", Price = 200, Stock = 20, CategoryId = 1 };
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbcontext, mockAudithService.Object);
            var result = await service.AddProduct(product);
            var savedProduct = await dbcontext.Products
             .FirstOrDefaultAsync(x => x.Name == "Laptop");

            Assert.NotNull(savedProduct);
            Assert.Equal(200, savedProduct.Price);
            Assert.Equal(20, savedProduct.stock);
        }
        [Fact]
        public async Task Updateproduct_WhenRequestValid_ReturnProduct()
        {
            await using var dbcontext = TestDbContextFactory.Create();
            var product = new Product {ProductId=1, Name = "Laptop", Description = "new laptop", Price = 200, stock = 20, CategoryId = 1 };
            var user = new Users { Id = 1 ,UserName ="demo1",Email="demo1@gmail.com",Role="Admin"};
            dbcontext.Users.Add(user);
            dbcontext.Products.Add(product);
            await dbcontext.SaveChangesAsync();
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbcontext, mockAudithService.Object);
            var updateproduct = new UpdateProductRequest { Name = "phone", Description = "new phone", Price = 400, Stock = 10, CategoryId = 2 };
            var result = await service.UpdateProduct(1, 1, updateproduct);
            var savedProduct = await dbcontext.Products.FirstOrDefaultAsync(x => x.ProductId == 1);
            Assert.NotNull(result);
            Assert.Equal("phone", result.Name);
            Assert.NotNull(savedProduct);
            Assert.Equal(10, savedProduct.stock);
            Assert.Equal("phone", savedProduct.Name);
            Assert.Equal("new phone", savedProduct.Description);
            Assert.Equal(400, savedProduct.Price);
            Assert.Equal(2, savedProduct.CategoryId);
            mockAudithService.Verify(
                x => x.CreateAsync(
                    It.Is<AuditLogRequest>(a => a.UserId == 1),
                    AuditAction.ProductChange),
                Times.Once);

        }
        [Fact]
        public async Task Deleteproduct_ValidRequest_ReturnProduct()
        {
            await using var dbcontext = TestDbContextFactory.Create();
            var product = new Product { ProductId = 1, Name = "Laptop", Description = "new laptop", Price = 200, stock = 20, CategoryId = 1 };
            dbcontext.Products.Add(product);
            await dbcontext.SaveChangesAsync();
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbcontext, mockAudithService.Object);
            var result = await service.DeleteProduct(1);
            var deletedProduct = await dbcontext.Products.FirstOrDefaultAsync(x => x.ProductId == 1);
            Assert.NotNull(result);
            Assert.Equal("Laptop", result.Name);
            Assert.Equal(200, result.Price);
            Assert.Equal(20, result.Stock);

            Assert.Null(deletedProduct);
        }
        [Fact]
        public async Task GetProductsAsync_ValidRequest_ReturnsProducts()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var products = new List<Product>
            {
                new Product { ProductId = 1,Name="Laptop 1",Description ="new laptop",Price=200,stock=20,CategoryId=1 },
                new Product { ProductId = 2,Name="Laptop 2",Description ="new laptop",Price=300,stock=10,CategoryId=1 },
                new Product { ProductId = 3,Name="iPhone",Description ="new mobile",Price=400,stock=10,CategoryId=1 },
                new Product { ProductId = 4,Name="LG TV",Description ="new smart scrren",Price=50,stock=5,CategoryId=1 },
            };
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAudithService.Object);
            var request = new ProductFilterRequest { Page=1,PageSize=10 , MinPrice =250};
            var result = await service.GetProductsAsync(request);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count);

            Assert.All(
                result.Data,
                x => Assert.True(x.Price >= 250));

        }
        [Fact]
        public async Task AddReview_ValidRequest_ReturnReview()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var product = new Product
            {
                ProductId = 1,
                Name = "Laptop",
                Description = "New Laptop",
                Price = 200,
                stock = 20,
                CategoryId = 1
            };

            var order = new Order
                    {
                        UserId = 1,
                        OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 200
                    }
                }
                    };

            dbContext.Products.Add(product);
            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync();

            var reviewRequest = new ReviewRequest { ProductId = 1, Rating = 5, Comment = "good product" };
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAudithService.Object);
            var result = await service.AddReview(1,reviewRequest);
            var existingReview = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Equal(5, result.Rating);
            Assert.Equal("Good product", result.Comment);

            Assert.NotNull(existingReview);
            Assert.Equal(1, existingReview.UserId);
            Assert.Equal(1, existingReview.ProductId);
            Assert.Equal(5, existingReview.Rating);

        }
        [Fact]
        public async Task AddReview_WhenUserDidNotPurchaseProduct_ReturnsNull()
        {
            await using var dbContext = TestDbContextFactory.Create();

            var order = new Order
            {
                UserId = 1,
                OrderItems = new List<OrderItem>()
            };
            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync();
            var reviewRequest = new ReviewRequest { ProductId = 1, Rating = 5, Comment = "good product" };
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAudithService.Object);
            var result = await service.AddReview(1, reviewRequest);
            Assert.Null(result);

        }
        [Fact]
        public async Task AddReview_WhenRatingIsLessThanOne_ReturnsNull()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var product = new Product
            {
                ProductId = 1,
                Name = "Laptop",
                Description = "New Laptop",
                Price = 200,
                stock = 20,
                CategoryId = 1
            };

            var order = new Order
            {
                UserId = 1,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 200
                    }
                }
            };

            dbContext.Products.Add(product);
            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync();

            var reviewRequest = new ReviewRequest { ProductId = 1, Rating = 0, Comment = "good product" };
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAudithService.Object);
            var result = await service.AddReview(1, reviewRequest);
            
            Assert.Null(result);
            

        }
        [Fact]
        public async Task AddReview_WhenRatingIsGreaterThanFive_ReturnsNull()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var product = new Product
            {
                ProductId = 1,
                Name = "Laptop",
                Description = "New Laptop",
                Price = 200,
                stock = 20,
                CategoryId = 1
            };

            var order = new Order
            {
                UserId = 1,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 200
                    }
                }
            };

            dbContext.Products.Add(product);
            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync();

            var reviewRequest = new ReviewRequest { ProductId = 1, Rating = 6, Comment = "good product" };
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAudithService.Object);
            var result = await service.AddReview(1, reviewRequest);

            Assert.Null(result);


        }
        [Fact]
        public async Task AddReview_WhenUserAlreadyReviewedProduct_ReturnsNull()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var product = new Product
            {
                ProductId = 1,
                Name = "Laptop",
                Description = "New Laptop",
                Price = 200,
                stock = 20,
                CategoryId = 1
            };

            var order = new Order
            {
                UserId = 1,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 200
                    }
                }
            };
            var review = new Review
            {
                Id = 1,
                UserId = 1,
                ProductId = 1,
                Rating = 5
            };

            dbContext.Products.Add(product);
            dbContext.Orders.Add(order);
            dbContext.Reviews.Add(review);

            await dbContext.SaveChangesAsync();

            var reviewRequest = new ReviewRequest { ProductId = 1, Rating = 4, Comment = "good product" };
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAudithService.Object);
            var result = await service.AddReview(1, reviewRequest);
            var exisrreview = await dbContext.Reviews.FirstOrDefaultAsync(x => x.UserId == 1);
            Assert.Null(result);
            Assert.NotNull(exisrreview);


        }
        [Fact]
        public async Task GetReviews_ValidRequst_ReturnReviews()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var reviws = new List<Review>
            {
                new Review { Id = 1, UserId = 1, ProductId = 1, Rating = 5, Comment = "good product" },
                new Review { Id = 2, UserId = 2, ProductId = 1, Rating = 4, Comment = "good product" },
                new Review { Id = 3, UserId = 3, ProductId = 1, Rating = 3, Comment = "good product" },
            };
            dbContext.Reviews.AddRange(reviws);
            await dbContext.SaveChangesAsync();
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAudithService.Object);
            var result = await service.GetReviews(1, 1, 10);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Data.Count);

        }
        [Fact]
        public async Task UpdateReview_ValidRequest_ReturnReviews()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var review = new Review { Id = 1, UserId = 1, ProductId = 1, Rating = 5, Comment = "good product" };
            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAudithService.Object);
            var updateReviewrequest = new UpdateReviewRequest { NewRating = 4, Comment = "decent" };
            var result = await service.UpdateReview(1,1, updateReviewrequest);
            var savedReview = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Equal(4, result.Rating);
            Assert.NotNull(savedReview);
            Assert.Equal(4, savedReview.Rating);


        }
        [Fact]
        public async Task DeleteReview_ValidRequest_returnReview()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var review = new Review { Id = 1, UserId = 1, ProductId = 1, Rating = 5, Comment = "good product" };
            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();
            var mockAudithService = new Mock<IAuditLogService>();
            var service = new ProductsService(dbContext, mockAudithService.Object);
            var result = await service.DeleteReview(1,1);
            var checkdeletedreview = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.NotNull(result);
            Assert.Null(checkdeletedreview);

        }

        

    }
}
