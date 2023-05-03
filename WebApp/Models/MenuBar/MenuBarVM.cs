using WebApp.Models.Cart;
using WebApp.Models.Store;

namespace WebApp.Models.MenuBar
{
    public class MenuBarVM
    {
        public IEnumerable<CartVM> CartVM { get; set; }
        public IEnumerable<StoreVM> StoreVM { get; set; }
    }
}
