using Microsoft.EntityFrameworkCore;
using MenShop_Assignment.Datas;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MenShop_Assignment.Datas
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchDetail> BranchDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<Fabric> Fabrics { get; set; }
        public DbSet<HistoryPrice> HistoryPrices { get; set; }
        public DbSet<ImagesProduct> ImagesProducts { get; set; }
        public DbSet<ImageCollection> ImageCollections { get; set; }
        public DbSet<InputReceipt> InputReceipts { get; set; }
        public DbSet<InputReceiptDetail> InputReceiptDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OutputReceipt> OutputReceipts { get; set; }
        public DbSet<OutputReceiptDetail> OutputReceiptDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionDetail> CollectionDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentDiscount> PaymentDiscounts { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<StorageDetail> StorageDetails { get; set; }
        public DbSet<GHTKOrder> GHTKOrders { get; set; }
        public DbSet<GHTKProduct> GHTKProducts { get; set; }
        public DbSet<DiscountPrice> DiscountPrices { get; set; }
        public DbSet<DiscountPriceDetail> DiscountPriceDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "BranchManager", NormalizedName = "BRANCHMANAGER" },
                new IdentityRole { Id = "3", Name = "BranchEmployee", NormalizedName = "BRANCHEMPLOYEE" },
                new IdentityRole { Id = "4", Name = "Customer", NormalizedName = "CUSTOMER" },
                new IdentityRole { Id = "5", Name = "Factory", NormalizedName = "FACTORY" },
                new IdentityRole { Id = "6", Name = "RevenueManager", NormalizedName = "REVENUEMANAGER" },
                new IdentityRole { Id = "7", Name = "Shipper", NormalizedName = "SHIPPER" },
                new IdentityRole { Id = "8", Name = "WarehouseManager", NormalizedName = "WAREHOUSEMANAGER" }
            );
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Branch>().HasKey(b => b.BranchId);
            modelBuilder.Entity<BranchDetail>().HasKey(b => new {b.BranchId,b.ProductDetailId});
            modelBuilder.Entity<Cart>().HasKey(c => c.CartId);
            modelBuilder.Entity<CartDetail>().HasKey(c => new {c.ProductDetailId,c.CartId});
            modelBuilder.Entity<CategoryProduct>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Color>().HasKey(c => c.ColorId);
            modelBuilder.Entity<CustomerAddress>().HasKey(c => c.Id);
            modelBuilder.Entity<Fabric>().HasKey(f=>f.FabricId);
            modelBuilder.Entity<HistoryPrice>().HasKey(h=>h.Id);
            modelBuilder.Entity<ImagesProduct>().HasKey(i => i.Id);
            modelBuilder.Entity<InputReceipt>().HasKey(i=>i.ReceiptId);
            modelBuilder.Entity<InputReceiptDetail>().HasKey(i=>new {i.ReceiptId,i.ProductDetailId});
            modelBuilder.Entity<Order>().HasKey(o=>o.OrderId);
            modelBuilder.Entity<OrderDetail>().HasKey(o => new {o.OrderId,o.ProductDetailId});
            modelBuilder.Entity<OutputReceipt>().HasKey(o=>o.ReceiptId);
            modelBuilder.Entity<OutputReceiptDetail>().HasKey(o=>new {o.ReceiptId,o.ProductDetailId});
            modelBuilder.Entity<Product>().HasKey(p=>p.ProductId);
            modelBuilder.Entity<ProductDetail>().HasKey(p => p.DetailId);
            modelBuilder.Entity<Size>().HasKey(s=>s.SizeId);
            modelBuilder.Entity<Storage>().HasKey(s=>s.StorageId);
            modelBuilder.Entity<StorageDetail>().HasKey(s => new { s.StorageId, s.ProductDetailId });
			modelBuilder.Entity<Payment>().HasKey(p => p.PaymentId);
			modelBuilder.Entity<PaymentDiscount>().HasKey(pd => pd.DiscountId);
            modelBuilder.Entity<GHTKOrder>().HasKey(go => go.Id);
            modelBuilder.Entity<GHTKProduct>().HasKey(gp=>gp.ProductId);

            // CategoryProduct - Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired(false);


            // User - Branch (Manager)
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Manager)
                .WithOne(u => u.ManagedBranch)
                .HasForeignKey<Branch>(b => b.ManagerId);

			// Cấu hình cho Payment
			modelBuilder.Entity<Payment>().HasKey(p => p.PaymentId);
			modelBuilder.Entity<Payment>()
				.HasOne(p => p.Order)
				.WithMany(o => o.Payments)
				.HasForeignKey(p => p.OrderId);

			// Cấu hình cho PaymentDiscount
			modelBuilder.Entity<PaymentDiscount>().HasKey(pd => pd.DiscountId);
            modelBuilder.Entity<PaymentDiscount>()
                .HasOne(pd => pd.Payment)
                .WithMany(p => p.Discounts)
                .HasForeignKey(pd => pd.PaymentId);

            // User - Branch (Employee)
            modelBuilder.Entity<User>()
                .HasOne(u => u.WorkedBranch)
                .WithMany(b => b.Employees)
                .HasForeignKey(u => u.BranchId)
                .IsRequired(false);

            // Branch - BranchDetail
            modelBuilder.Entity<BranchDetail>()
                .HasOne(bd => bd.Branch)
                .WithMany(b => b.BranchDetails)
                .HasForeignKey(bd => bd.BranchId);

            modelBuilder.Entity<BranchDetail>()
                .HasOne(bd => bd.ProductDetail)
                .WithMany(pd => pd.BranchDetails)
                .HasForeignKey(bd => bd.ProductDetailId);

            // User - Cart
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithOne(u => u.CustomerCart)
                .HasForeignKey<Cart>(c => c.CustomerId)
                .IsRequired(false);

            // Cart - CartDetail
            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.Cart)
                .WithMany(c => c.Details)
                .HasForeignKey(cd => cd.CartId);

            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.ProductDetail)
                .WithMany(pd => pd.CartDetails)
                .HasForeignKey(cd => cd.ProductDetailId)
                .IsRequired(false);

            // CategoryProduct - Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired(false);

            // Color - ProductDetail
            modelBuilder.Entity<ProductDetail>()
                .HasOne(pd => pd.Color)
                .WithMany(c => c.ProductDetails)
                .HasForeignKey(pd => pd.ColorId);

            // User - CustomerAddress
            modelBuilder.Entity<CustomerAddress>()
                .HasOne(ca => ca.Customer)
                .WithMany(u => u.CustomerAddresses)
                .HasForeignKey(ca => ca.CustomerId);

            // Fabric - ProductDetail
            modelBuilder.Entity<ProductDetail>()
                .HasOne(pd => pd.Fabric)
                .WithMany(f => f.ProductDetails)
                .HasForeignKey(pd => pd.FabricId);

            // ProductDetail - HistoryPrice
            modelBuilder.Entity<HistoryPrice>()
                .HasOne(hp => hp.ProductDetail)
                .WithMany(pd => pd.HistoryPrices)
                .HasForeignKey(hp => hp.ProductDetailId);

            // ProductDetail - ImagesProduct
            modelBuilder.Entity<ImagesProduct>()
                .HasOne(ip => ip.ProductDetail)
                .WithMany(pd => pd.Images)
                .HasForeignKey(ip => ip.ProductDetailId);

            modelBuilder.Entity<Collection>()
                .HasMany(c => c.Images)
                .WithOne(i => i.Collection)
                .HasForeignKey(i => i.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collection>()
                .HasMany(c => c.CollectionDetails)
                .WithOne(cd => cd.Collection)
                .HasForeignKey(cd => cd.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<CollectionDetail>()
                .HasOne(cd => cd.Collection)
                .WithMany(c => c.CollectionDetails)
                .HasForeignKey(cd => cd.CollectionId);

            modelBuilder.Entity<CollectionDetail>()
                .HasOne(cd => cd.Product)
                .WithMany(p => p.CollectionDetails)
                .HasForeignKey(cd => cd.ProductId);
            modelBuilder.Entity<ImageCollection>()
                .HasOne(img => img.Collection)
                .WithMany(c => c.Images)
                .HasForeignKey(img => img.CollectionId)
                .IsRequired(false);
            // User - InputReceipt (Manager)
            modelBuilder.Entity<InputReceipt>()
                .HasOne(ir => ir.Manager)
                .WithMany(u => u.InputReceipts)
                .HasForeignKey(ir => ir.ManagerId)
                .IsRequired(false);

            // Storage - InputReceipt
            modelBuilder.Entity<InputReceipt>()
                .HasOne(ir => ir.Storage)
                .WithMany(s => s.InputReceipts)
                .HasForeignKey(ir => ir.StorageId);

            // InputReceipt - InputReceiptDetail
            modelBuilder.Entity<InputReceiptDetail>()
                .HasOne(ird => ird.InputReceipt)
                .WithMany(ir => ir.InputReceiptDetails)
                .HasForeignKey(ird => ird.ReceiptId);

            modelBuilder.Entity<InputReceiptDetail>()
                .HasOne(ird => ird.ProductDetail)
                .WithMany(pd => pd.InputReceiptDetails)
                .HasForeignKey(ird => ird.ProductDetailId);

            // User - Order (Customer)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(u => u.CustomerOrders)
                .HasForeignKey(o => o.CustomerId)
                .IsRequired(false);

            // User - Order (Employee)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany(u => u.EmployeesOrders)
                .HasForeignKey(o => o.EmployeeId)
                .IsRequired(false);
            //User - Order (Shipper)
            modelBuilder.Entity<Order>()
                .HasOne(o=>o.Shipper)
                .WithMany(u=>u.ShipperOrders)
                .HasForeignKey(o => o.ShipperId)
                .IsRequired(false);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Branch)
                .WithMany(b => b.Orders)
                .HasForeignKey(o => o.BranchId);
            // Order - OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.Details)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.ProductDetail)
                .WithMany(pd => pd.OrderDetails)
                .HasForeignKey(od => od.ProductDetailId);

            // User - OutputReceipt (Manager)
            modelBuilder.Entity<OutputReceipt>()
                .HasOne(or => or.Manager)
                .WithMany(u => u.OutputReceipts)
                .HasForeignKey(or => or.ManagerId)
                .IsRequired(false);

            // Branch - OutputReceipt
            modelBuilder.Entity<OutputReceipt>()
                .HasOne(or => or.Branch)
                .WithMany(b => b.OutputReceipts)
                .HasForeignKey(or => or.BranchId);

            // OutputReceipt - OutputReceiptDetail
            modelBuilder.Entity<OutputReceiptDetail>()
                .HasOne(ord => ord.OutputReceipt)
                .WithMany(or => or.OutputReceiptDetails)
                .HasForeignKey(ord => ord.ReceiptId);

            modelBuilder.Entity<OutputReceiptDetail>()
                .HasOne(ord => ord.ProductDetail)
                .WithMany(pd => pd.OutputReceiptDetails)
                .HasForeignKey(ord => ord.ProductDetailId);

            // Product - ProductDetail
            modelBuilder.Entity<ProductDetail>()
                .HasOne(pd => pd.Product)
                .WithMany(p => p.ProductDetails)
                .HasForeignKey(pd => pd.ProductId);

            // Size - ProductDetail
            modelBuilder.Entity<ProductDetail>()
                .HasOne(pd => pd.Size)
                .WithMany(s => s.ProductDetails)
                .HasForeignKey(pd => pd.SizeId);

            // CategoryProduct - Storage
            modelBuilder.Entity<Storage>()
                .HasOne(s => s.CategoryProduct)
                .WithOne(cp => cp.Storage)
                .HasForeignKey<Storage>(s => s.CategoryId)
                .IsRequired(false);

            // User - Storage (Manager)
            modelBuilder.Entity<Storage>()
                .HasOne(s => s.Manager)
                .WithMany(u => u.Storages)
                .HasForeignKey(s => s.ManagerId)
                .IsRequired(false);

            // Storage - StorageDetail
            modelBuilder.Entity<StorageDetail>()
                .HasOne(sd => sd.Storage)
                .WithMany(s => s.StorageDetails)
                .HasForeignKey(sd => sd.StorageId);

            modelBuilder.Entity<StorageDetail>()
                .HasOne(sd => sd.ProductDetail)
                .WithMany(pd => pd.StorageDetails)
                .HasForeignKey(sd => sd.ProductDetailId);

            // User - User (Manager)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithMany(u => u.Employees)
                .HasForeignKey(u => u.ManagerId)
                .IsRequired(false);
            //
            modelBuilder.Entity<User>()
    .OwnsOne(u => u.WorkArea);

            modelBuilder.Entity<Branch>()
    .OwnsOne(b => b.Address);


        }
    }
}