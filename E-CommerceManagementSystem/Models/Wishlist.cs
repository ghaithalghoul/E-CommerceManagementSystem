namespace E_CommerceManagementSystem.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; } = null!;
        public ICollection<WishlistItem> wishlistItems { get; set; } = new List<WishlistItem>();
    }
}
