using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
namespace GroceryAPP
{
    public partial class Product1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCategories();
                BindProducts();
            }
        }

        private void BindCategories()
        {
            using (var context = new GroceryDBEntities())
            {
                var categories = context.Categories.ToList();
                categories.Insert(0, new Category { CategoryId = 0, CategoryName = "Select Category" });

                CategoryDropDown.DataSource = categories;
                CategoryDropDown.DataTextField = "CategoryName"; // Assuming CategoryName is the field for category name
                CategoryDropDown.DataValueField = "CategoryId";  // Assuming CategoryId is the field for category ID
                CategoryDropDown.DataBind();
            }
        }

        protected void CategoryDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindProducts(CategoryDropDown.SelectedValue);
        }

        private void BindProducts(string category = null)
        {
            using (var context = new GroceryDBEntities())
            {
                List<Product> products;

                if (!string.IsNullOrEmpty(category))
                {
                    if (int.TryParse(category, out int categoryId))
                    {
                        products = context.Products.Where(p => p.CategoryId == categoryId).ToList();
                    }
                    else
                    {
                        // Handle invalid category ID (e.g., log an error or display a message)
                        products = new List<Product>(); // Empty list or handle as appropriate
                    }
                }
                else
                {
                    products = context.Products.ToList();
                }

                if (products.Any())
                {
                    ProductRepeater.DataSource = products;
                    ProductRepeater.DataBind();
                }
            }
        }

        //For carting
        protected void AddToCartButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int productId = Convert.ToInt32(button.CommandArgument);
            AddToCart(productId);
        }




        private void AddToCart(int productId)
        {
            using (var context = new GroceryDBEntities())
            {
                Product product = context.Products.FirstOrDefault(p => p.ProductId == productId);

                if (product != null)
                {
                    Cart cartItem = new Cart
                    {
                        ProductId = productId,
                        Quantity = 1,
                        TotalCost = product.Price
                    };

                    int? userId = GetUserId(); // Ensure GetUserId() returns a nullable int

                    if (userId.HasValue)
                    {
                        cartItem.UserId = userId.Value;
                    }

                    context.Carts.Add(cartItem);
                    context.SaveChanges();
                }
            }
        }

        private int? GetUserId()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)HttpContext.Current.User.Identity;
                var userIdClaim = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            return null;
        }


    }


}
