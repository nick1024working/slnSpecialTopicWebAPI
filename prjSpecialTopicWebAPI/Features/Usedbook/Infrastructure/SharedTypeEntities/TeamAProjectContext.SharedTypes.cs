using Microsoft.EntityFrameworkCore;

namespace prjSpecialTopicWebAPI.Models
{
    public partial class TeamAProjectContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.SharedTypeEntity<Dictionary<string, object>>("UsedBookSaleTag", b =>
            {
                b.ToTable("UsedBookSaleTags");

                b.IndexerProperty<Guid>("BookId");
                b.IndexerProperty<int>("TagId");

                b.HasKey("BookId", "TagId");

                b.HasOne<BookSaleTag>().WithMany().HasForeignKey("TagId");
                b.HasOne<UsedBook>().WithMany().HasForeignKey("BookId");
            });
        }
    }
}
