using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace prjSpecialTopicWebAPI.Models;

public partial class TeamAProjectContext : DbContext
{
    public TeamAProjectContext()
    {
    }

    public TeamAProjectContext(DbContextOptions<TeamAProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookBinding> BookBindings { get; set; }

    public virtual DbSet<BookCategory> BookCategories { get; set; }

    public virtual DbSet<BookConditionDetail> BookConditionDetails { get; set; }

    public virtual DbSet<BookConditionRating> BookConditionRatings { get; set; }

    public virtual DbSet<BookSaleTag> BookSaleTags { get; set; }

    public virtual DbSet<ContentRating> ContentRatings { get; set; }

    public virtual DbSet<County> Counties { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<DonateCategory> DonateCategories { get; set; }

    public virtual DbSet<DonateImage> DonateImages { get; set; }

    public virtual DbSet<DonateOrder> DonateOrders { get; set; }

    public virtual DbSet<DonateOrderItem> DonateOrderItems { get; set; }

    public virtual DbSet<DonatePlan> DonatePlans { get; set; }

    public virtual DbSet<DonateProject> DonateProjects { get; set; }

    public virtual DbSet<EBookCategory> EBookCategories { get; set; }

    public virtual DbSet<EBookImage> EBookImages { get; set; }

    public virtual DbSet<EBookMain> EBookMains { get; set; }

    public virtual DbSet<EBookOrderMain> EBookOrderMains { get; set; }

    public virtual DbSet<EBookOrderStatus> EBookOrderStatuses { get; set; }

    public virtual DbSet<EbookPurchased> EbookPurchaseds { get; set; }

    public virtual DbSet<EbookRecommend> EbookRecommends { get; set; }

    public virtual DbSet<ForumPost> ForumPosts { get; set; }

    public virtual DbSet<ItemType> ItemTypes { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<LoginLog> LoginLogs { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PostBookmark> PostBookmarks { get; set; }

    public virtual DbSet<PostCategory> PostCategories { get; set; }

    public virtual DbSet<PostComment> PostComments { get; set; }

    public virtual DbSet<PostFilter> PostFilters { get; set; }

    public virtual DbSet<PostImage> PostImages { get; set; }

    public virtual DbSet<PostLike> PostLikes { get; set; }

    public virtual DbSet<RecommendationType> RecommendationTypes { get; set; }

    public virtual DbSet<Series> Series { get; set; }

    public virtual DbSet<Subscriber> Subscribers { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<UsedBook> UsedBooks { get; set; }

    public virtual DbSet<UsedBookImage> UsedBookImages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=TeamA_Project;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookBinding>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookBind__3214EC0714C9AF18");

            entity.HasIndex(e => e.Name, "UQ_BookBindings_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(5);
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookCate__3214EC075A079EC6");

            entity.HasIndex(e => e.Name, "UQ_BookCategories_Name").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__BookCate__BC7B5FB6E79E9755").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(10);
            entity.Property(e => e.Slug).HasMaxLength(255);
        });

        modelBuilder.Entity<BookConditionDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookCond__3214EC07303AAF88");

            entity.HasIndex(e => e.Name, "UQ_BookConditionDetails_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(10);
        });

        modelBuilder.Entity<BookConditionRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookCond__3214EC0740E6D402");

            entity.HasIndex(e => e.Name, "UQ_BookConditionRatings_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(5);
        });

        modelBuilder.Entity<BookSaleTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookSale__3214EC070308E9D2");

            entity.HasIndex(e => e.Name, "UQ_BookSaleTags_Name").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__BookSale__BC7B5FB69D030C93").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(10);
            entity.Property(e => e.Slug).HasMaxLength(255);
        });

        modelBuilder.Entity<ContentRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentR__3214EC07773A60C5");

            entity.HasIndex(e => e.Name, "UQ_ContentRatings_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(5);
        });

        modelBuilder.Entity<County>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Counties__3214EC079DBA9590");

            entity.HasIndex(e => e.Name, "UQ_Counties_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(5);
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__District__3214EC07B4FFBE4E");

            entity.HasIndex(e => new { e.CountyId, e.Name }, "UQ_Districts_CountyId_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(5);

            entity.HasOne(d => d.County).WithMany(p => p.Districts)
                .HasForeignKey(d => d.CountyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Districts_Counties");
        });

        modelBuilder.Entity<DonateCategory>(entity =>
        {
            entity.HasKey(e => e.DonateCategoriesId).HasName("PK__donateCa__78949A147A649D41");

            entity.ToTable("donateCategories");

            entity.Property(e => e.DonateCategoriesId).HasColumnName("donateCategories_id");
            entity.Property(e => e.CategoriesName)
                .HasMaxLength(100)
                .HasColumnName("categoriesName");
        });

        modelBuilder.Entity<DonateImage>(entity =>
        {
            entity.HasKey(e => e.DonateImageId).HasName("PK__donateIm__696847680CF6D0E3");

            entity.ToTable("donateImages");

            entity.HasIndex(e => e.DonatePlanId, "IX_donateImages_donatePlan_id");

            entity.Property(e => e.DonateImageId).HasColumnName("donateImage_id");
            entity.Property(e => e.DonateImagePath)
                .HasMaxLength(50)
                .HasColumnName("donateImagePath");
            entity.Property(e => e.DonatePlanId).HasColumnName("donatePlan_id");
            entity.Property(e => e.DonateProjectId).HasColumnName("donateProject_id");
            entity.Property(e => e.IsMain).HasColumnName("is_main");
            entity.Property(e => e.ProjectGalleryPath)
                .HasMaxLength(50)
                .HasColumnName("projectGalleryPath");

            entity.HasOne(d => d.DonatePlan).WithMany(p => p.DonateImages)
                .HasForeignKey(d => d.DonatePlanId)
                .HasConstraintName("FK_donateImages_donatePlans");

            entity.HasOne(d => d.DonateProject).WithMany(p => p.DonateImages)
                .HasForeignKey(d => d.DonateProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_donateImages_donateProjects");
        });

        modelBuilder.Entity<DonateOrder>(entity =>
        {
            entity.HasKey(e => e.DonateOrderId).HasName("PK__donateOr__62F476960AD39A4F");

            entity.ToTable("donateOrders");

            entity.Property(e => e.DonateOrderId).HasColumnName("donateOrder_id");
            entity.Property(e => e.OrderCreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("orderCreated_at");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_method");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.Uid).HasColumnName("UID");
        });

        modelBuilder.Entity<DonateOrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemsId).HasName("PK__donateOr__86607EE1AA9544A0");

            entity.ToTable("donateOrder_items");

            entity.Property(e => e.OrderItemsId).HasColumnName("orderItems_id");
            entity.Property(e => e.DonateOrderId).HasColumnName("donateOrder_id");
            entity.Property(e => e.DonatePlanId).HasColumnName("donatePlan_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.DonateOrder).WithMany(p => p.DonateOrderItems)
                .HasForeignKey(d => d.DonateOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orderItems_donateOrder");

            entity.HasOne(d => d.DonatePlan).WithMany(p => p.DonateOrderItems)
                .HasForeignKey(d => d.DonatePlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orderItems_donatePlan");
        });

        modelBuilder.Entity<DonatePlan>(entity =>
        {
            entity.HasKey(e => e.DonatePlanId).HasName("PK__donatePl__9B6171FAB6F36F2D");

            entity.ToTable("donatePlans");

            entity.Property(e => e.DonatePlanId).HasColumnName("donatePlan_id");
            entity.Property(e => e.DonateProjectId).HasColumnName("donateProject_id");
            entity.Property(e => e.PlanDescription)
                .HasColumnType("text")
                .HasColumnName("planDescription");
            entity.Property(e => e.PlanTitle)
                .HasMaxLength(50)
                .HasColumnName("planTitle");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.DonateProject).WithMany(p => p.DonatePlans)
                .HasForeignKey(d => d.DonateProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_donatePlans_donateProjects");
        });

        modelBuilder.Entity<DonateProject>(entity =>
        {
            entity.HasKey(e => e.DonateProjectId).HasName("PK__donatePr__80A69FA58E0A0CBE");

            entity.ToTable("donateProjects");

            entity.Property(e => e.DonateProjectId).HasColumnName("donateProject_id");
            entity.Property(e => e.BackerCount).HasColumnName("backerCount");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("current_amount");
            entity.Property(e => e.DonateCategoriesId).HasColumnName("donateCategories_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ProjectDescription)
                .HasColumnType("text")
                .HasColumnName("projectDescription");
            entity.Property(e => e.ProjectIsFavorite).HasColumnName("projectIsFavorite");
            entity.Property(e => e.ProjectLongDescription).HasColumnName("projectLongDescription");
            entity.Property(e => e.ProjectTitle)
                .HasMaxLength(200)
                .HasColumnName("projectTitle");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TargetAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("target_amount");
            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.DonateCategories).WithMany(p => p.DonateProjects)
                .HasForeignKey(d => d.DonateCategoriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_donateProjects_donateCategories");
        });

        modelBuilder.Entity<EBookCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("eBook_Categories");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK_eBook_Categories_Parent");
        });

        modelBuilder.Entity<EBookImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.ToTable("eBook_Images");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.EBookId).HasColumnName("eBookID");

            entity.HasOne(d => d.EBook).WithMany(p => p.EBookImages)
                .HasForeignKey(d => d.EBookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_eBook_Images_eBookID");
        });

        modelBuilder.Entity<EBookMain>(entity =>
        {
            entity.HasKey(e => e.EbookId).HasName("PK__eBookMai__C5457EF9EF4FD057");

            entity.ToTable("eBook_Main");

            entity.Property(e => e.EbookId).HasColumnName("ebookID");
            entity.Property(e => e.ActualPrice)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("actualPrice");
            entity.Property(e => e.Author)
                .HasMaxLength(100)
                .HasColumnName("author");
            entity.Property(e => e.BookDescription).HasColumnName("bookDescription");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Couponcode)
                .HasMaxLength(100)
                .HasColumnName("couponcode");
            entity.Property(e => e.Cumulativesales).HasColumnName("cumulativesales");
            entity.Property(e => e.Discount)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("discount");
            entity.Property(e => e.EBookDataType)
                .HasMaxLength(100)
                .HasColumnName("eBookDataType");
            entity.Property(e => e.EBookPosition).HasColumnName("eBookPosition");
            entity.Property(e => e.EBookType)
                .HasMaxLength(50)
                .HasColumnName("eBookType");
            entity.Property(e => e.EbookName)
                .HasMaxLength(200)
                .HasColumnName("ebookName");
            entity.Property(e => e.Eisbn)
                .HasMaxLength(100)
                .HasColumnName("EISBN");
            entity.Property(e => e.FixedPrice)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("fixedPrice");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("isAvailable");
            entity.Property(e => e.Isbn)
                .HasMaxLength(100)
                .HasColumnName("ISBN");
            entity.Property(e => e.Language)
                .HasMaxLength(100)
                .HasColumnName("language");
            entity.Property(e => e.MaturityRating).HasColumnName("maturityRating");
            entity.Property(e => e.Monthsales).HasColumnName("monthsales");
            entity.Property(e => e.Monthviews).HasColumnName("monthviews");
            entity.Property(e => e.PublishedCountry)
                .HasMaxLength(100)
                .HasColumnName("publishedCountry");
            entity.Property(e => e.PublishedDate).HasColumnName("publishedDate");
            entity.Property(e => e.Publisher)
                .HasMaxLength(100)
                .HasColumnName("publisher");
            entity.Property(e => e.PurchaseCountry)
                .HasMaxLength(100)
                .HasColumnName("purchaseCountry");
            entity.Property(e => e.SeriesId).HasColumnName("SeriesID");
            entity.Property(e => e.Totalsales).HasColumnName("totalsales");
            entity.Property(e => e.Totalviews).HasColumnName("totalviews");
            entity.Property(e => e.Translator)
                .HasMaxLength(100)
                .HasColumnName("translator");
            entity.Property(e => e.Weeksales).HasColumnName("weeksales");
            entity.Property(e => e.Weekviews).HasColumnName("weekviews");

            entity.HasOne(d => d.Category).WithMany(p => p.EBookMains)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_eBook_Main_CategoryID");

            entity.HasOne(d => d.Series).WithMany(p => p.EBookMains)
                .HasForeignKey(d => d.SeriesId)
                .HasConstraintName("FK_eBook_Main_SeriesID");

            entity.HasMany(d => d.Labels).WithMany(p => p.EBooks)
                .UsingEntity<Dictionary<string, object>>(
                    "EBookLabel",
                    r => r.HasOne<Label>().WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_eBook_Labels_LabelID"),
                    l => l.HasOne<EBookMain>().WithMany()
                        .HasForeignKey("EBookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_eBook_Labels_eBookID"),
                    j =>
                    {
                        j.HasKey("EBookId", "LabelId");
                        j.ToTable("eBook_Labels");
                        j.IndexerProperty<long>("EBookId").HasColumnName("eBookID");
                        j.IndexerProperty<int>("LabelId").HasColumnName("LabelID");
                    });
        });

        modelBuilder.Entity<EBookOrderMain>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("eBook_Order_Main");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("OrderID");
            entity.Property(e => e.BillingAddressId).HasColumnName("BillingAddressID");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("IPAddress");
            entity.Property(e => e.OrderStatusId).HasColumnName("OrderStatusID");
            entity.Property(e => e.PaymentGatewayTransactionId)
                .HasMaxLength(255)
                .HasColumnName("PaymentGatewayTransactionID");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalDiscountAmount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.UserAgent).HasMaxLength(512);

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.EBookOrderMains)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_eBook_Order_Main_OrderStatusID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.EBookOrderMains)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_eBook_Order_Main_UID");
        });

        modelBuilder.Entity<EBookOrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId);

            entity.ToTable("eBook_Order_Status");

            entity.Property(e => e.OrderStatusId)
                .ValueGeneratedNever()
                .HasColumnName("OrderStatusID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<EbookPurchased>(entity =>
        {
            entity.HasKey(e => e.PurchaseId);

            entity.ToTable("ebookPurchased");

            entity.Property(e => e.PurchaseId).HasColumnName("PurchaseID");
            entity.Property(e => e.EBookId).HasColumnName("eBookID");
            entity.Property(e => e.ReadingProgress).HasMaxLength(100);
            entity.Property(e => e.Uid).HasColumnName("UID");

            entity.HasOne(d => d.EBook).WithMany(p => p.EbookPurchaseds)
                .HasForeignKey(d => d.EBookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ebookPurchased_eBookID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.EbookPurchaseds)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ebookPurchased_UID");
        });

        modelBuilder.Entity<EbookRecommend>(entity =>
        {
            entity.HasKey(e => e.RecommendId);

            entity.ToTable("ebookRecommend");

            entity.Property(e => e.RecommendId).HasColumnName("recommendID");
            entity.Property(e => e.EbookId).HasColumnName("ebookID");
            entity.Property(e => e.RecTypeId).HasColumnName("RecTypeID");

            entity.HasOne(d => d.Ebook).WithMany(p => p.EbookRecommends)
                .HasForeignKey(d => d.EbookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ebookRecommend_ebookID");

            entity.HasOne(d => d.RecType).WithMany(p => p.EbookRecommends)
                .HasForeignKey(d => d.RecTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ebookRecommend_RecTypeID");
        });

        modelBuilder.Entity<ForumPost>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__ForumPos__AA1260385491050D");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FilterId).HasColumnName("FilterID");
            entity.Property(e => e.PostCategoryId).HasColumnName("PostCategoryID");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Uid).HasColumnName("UID");

            entity.HasOne(d => d.Filter).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.FilterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ForumPost__Filte__5DCAEF64");

            entity.HasOne(d => d.PostCategory).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.PostCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ForumPost__PostC__5CD6CB2B");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ForumPosts__UID__5BE2A6F2");
        });

        modelBuilder.Entity<ItemType>(entity =>
        {
            entity.Property(e => e.ItemTypeId)
                .ValueGeneratedNever()
                .HasColumnName("ItemTypeID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasIndex(e => e.LabelName, "UQ_Labels_LabelName").IsUnique();

            entity.Property(e => e.LabelId).HasColumnName("LabelID");
            entity.Property(e => e.LabelName).HasMaxLength(100);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Language__3214EC075FBBB677");

            entity.HasIndex(e => e.Name, "UQ_Language_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(5);
        });

        modelBuilder.Entity<LoginLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__LoginLog__5E54864820A87B61");

            entity.Property(e => e.LoginDate).HasColumnType("datetime");
            entity.Property(e => e.Uid).HasColumnName("UID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.LoginLogs)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoginLogs__UID__398D8EEE");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("Order_Items");

            entity.Property(e => e.OrderItemId)
                .ValueGeneratedNever()
                .HasColumnName("OrderItemID");
            entity.Property(e => e.ChapterId).HasColumnName("ChapterID");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.EBookId).HasColumnName("eBookID");
            entity.Property(e => e.ItemNameSnapshot).HasMaxLength(512);
            entity.Property(e => e.ItemTypeId).HasColumnName("ItemTypeID");
            entity.Property(e => e.LineItemTotal).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.UnitPriceAtPurchase).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.WorkId).HasColumnName("WorkID");

            entity.HasOne(d => d.EBook).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.EBookId)
                .HasConstraintName("FK_Order_Items_eBookID");

            entity.HasOne(d => d.ItemType).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ItemTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Items_ItemTypeID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Items_OrderID");

            entity.HasOne(d => d.Plan).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_Order_Items_PlanID");
        });

        modelBuilder.Entity<PostBookmark>(entity =>
        {
            entity.HasKey(e => e.BookmarkId).HasName("PK__PostBook__541A3A9188216F43");

            entity.HasIndex(e => new { e.PostId, e.Uid }, "UQ_PostBookmarks").IsUnique();

            entity.Property(e => e.BookmarkId).HasColumnName("BookmarkID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Uid).HasColumnName("UID");

            entity.HasOne(d => d.Post).WithMany(p => p.PostBookmarks)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostBookm__PostI__6A30C649");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.PostBookmarks)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostBookmar__UID__6B24EA82");
        });

        modelBuilder.Entity<PostCategory>(entity =>
        {
            entity.HasKey(e => e.PostCategoryId).HasName("PK__PostCate__FE61E369691A500D");

            entity.ToTable("PostCategory");

            entity.Property(e => e.PostCategoryId).HasColumnName("PostCategoryID");
            entity.Property(e => e.PostCategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__PostComm__C3B4DFAA12467387");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Uid).HasColumnName("UID");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__PostComme__Paren__656C112C");

            entity.HasOne(d => d.Post).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostComme__PostI__6477ECF3");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostComment__UID__66603565");
        });

        modelBuilder.Entity<PostFilter>(entity =>
        {
            entity.HasKey(e => e.PostFilterId).HasName("PK__PostFilt__2511E22B7AA6A416");

            entity.ToTable("PostFilter");

            entity.Property(e => e.PostFilterId).HasColumnName("PostFilterID");
            entity.Property(e => e.FilterName).HasMaxLength(200);
            entity.Property(e => e.PostCategoryId).HasColumnName("PostCategoryID");

            entity.HasOne(d => d.PostCategory).WithMany(p => p.PostFilters)
                .HasForeignKey(d => d.PostCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostFilte__PostC__5629CD9C");
        });

        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__PostImag__7516F4EC3C5F024D");

            entity.HasIndex(e => e.PostId, "IX_Unique_MainPic")
                .IsUnique()
                .HasFilter("([IsMainPic]=(1))");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.PostImage1)
                .HasColumnType("image")
                .HasColumnName("PostImage");

            entity.HasOne(d => d.Post).WithOne(p => p.PostImage)
                .HasForeignKey<PostImage>(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostImage__PostI__60A75C0F");
        });

        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__PostLike__A2922CF4C26A3DF3");

            entity.HasIndex(e => new { e.PostId, e.Uid }, "UQ_PostLikes").IsUnique();

            entity.Property(e => e.LikeId).HasColumnName("LikeID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Uid).HasColumnName("UID");

            entity.HasOne(d => d.Post).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostLikes__PostI__6EF57B66");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostLikes__UID__6FE99F9F");
        });

        modelBuilder.Entity<RecommendationType>(entity =>
        {
            entity.HasKey(e => e.RecTypeId);

            entity.HasIndex(e => e.TypeName, "UQ_RecommendationTypes_TypeName").IsUnique();

            entity.Property(e => e.RecTypeId).HasColumnName("RecTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Series>(entity =>
        {
            entity.Property(e => e.SeriesId).HasColumnName("SeriesID");
            entity.Property(e => e.SeriesName).HasMaxLength(200);
        });

        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId);

            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.DueTime)
                .HasPrecision(0)
                .HasColumnName("dueTime");
            entity.Property(e => e.LastPayTime)
                .HasPrecision(0)
                .HasColumnName("lastPayTime");
            entity.Property(e => e.NextPayTime)
                .HasPrecision(0)
                .HasColumnName("nextPayTime");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.Uid).HasColumnName("UID");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscribers)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscribers_PlanID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.Subscribers)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subscribers_UID");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId);

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Scope).HasMaxLength(50);
        });

        modelBuilder.Entity<UsedBook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsedBook__3214EC07E4BFAF92");

            entity.HasIndex(e => e.Slug, "UQ__UsedBook__BC7B5FB6EEFDDB88").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Authors).HasMaxLength(100);
            entity.Property(e => e.ConditionDescription).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Edition).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.Publisher).HasMaxLength(100);
            entity.Property(e => e.SalePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Binding).WithMany(p => p.UsedBooks)
                .HasForeignKey(d => d.BindingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__UsedBooks__Bindi__1975C517");

            entity.HasOne(d => d.Category).WithMany(p => p.UsedBooks)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsedBook_BookCategory");

            entity.HasOne(d => d.ConditionRating).WithMany(p => p.UsedBooks)
                .HasForeignKey(d => d.ConditionRatingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsedBooks__Condi__1881A0DE");

            entity.HasOne(d => d.ContentRating).WithMany(p => p.UsedBooks)
                .HasForeignKey(d => d.ContentRatingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsedBooks__Conte__1B5E0D89");

            entity.HasOne(d => d.Language).WithMany(p => p.UsedBooks)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__UsedBooks__Langu__1A69E950");

            entity.HasOne(d => d.SellerDistrict).WithMany(p => p.UsedBooks)
                .HasForeignKey(d => d.SellerDistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsedBooks__Selle__178D7CA5");

            entity.HasOne(d => d.Seller).WithMany(p => p.UsedBooks)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsedBooks__Selle__1699586C");

            entity.HasMany(d => d.Conditions).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "UsedBookConditionDetail",
                    r => r.HasOne<BookConditionDetail>().WithMany()
                        .HasForeignKey("ConditionId")
                        .HasConstraintName("FK__UsedBookC__Condi__2B947552"),
                    l => l.HasOne<UsedBook>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("FK__UsedBookC__BookI__2AA05119"),
                    j =>
                    {
                        j.HasKey("BookId", "ConditionId").HasName("PK__UsedBook__DE9F9E0B214C7AD2");
                        j.ToTable("UsedBookConditionDetails");
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "UsedBookSaleTag",
                    r => r.HasOne<BookSaleTag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK__UsedBookS__TagId__3429BB53"),
                    l => l.HasOne<UsedBook>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("FK__UsedBookS__BookI__3335971A"),
                    j =>
                    {
                        j.HasKey("BookId", "TagId").HasName("PK__UsedBook__EBB70D9D2021EBB2");
                        j.ToTable("UsedBookSaleTags");
                    });
        });

        modelBuilder.Entity<UsedBookImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsedBook__3214EC0756533187");

            entity.HasIndex(e => new { e.StorageProvider, e.ObjectKey }, "UQ__UsedBook__19B1FA10C8C16175").IsUnique();

            entity.Property(e => e.ObjectKey).HasMaxLength(300);
            entity.Property(e => e.Sha256)
                .HasMaxLength(32)
                .IsFixedLength();
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Book).WithMany(p => p.UsedBookImages)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__UsedBookI__BookI__3BCADD1B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PK__Users__C5B1960222CAFA0D");

            entity.Property(e => e.Uid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("UID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.AvatarUrl).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RegisterDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
