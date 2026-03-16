using Application.DTOs;

namespace HemaBazaar.MVC.Models
{
    public class ItemCategoryListModel
    {
        public IEnumerable<ItemDTO> ItemList { get; set; }
        public IEnumerable<CategoryDTO> CategoryList { get; set; }


    }
}
