using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
[Authorize]
public class CartController : Controller
{
    OrganicContext context;
    VnPaymentService service;
    public CartController(OrganicContext context, VnPaymentService service)
    {
        this.context = context;
        this.service= service;
    }

    public IActionResult Checkout(){
     ViewBag.Departments = context.Departments.ToList();

        ViewBag.Categories = context.Categories.ToList();
string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(memberId)) {
            return Redirect("/auth/login");
        }
        List<Cart> carts = context.Carts.Include(p => p.Product).Where(p => p.MemberId == memberId).ToList();
ViewBag.Total = carts.Sum(p => p.Quantity * (p.Product?.Price ?? 0)); // Ensure Price has a default value
ViewBag.Carts = carts;
        return View(
            new Invoice{
                GivenName = User.FindFirstValue(ClaimTypes.GivenName)!,
                Surname = User.FindFirstValue(ClaimTypes.Surname)

            }
        );
    }


    public IActionResult VnPaymentResponse(VnPayment obj)
    {
            return Json(obj);
    }
    [HttpPost]
    public IActionResult Checkout(Invoice obj){
        string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(memberId)) {
            return Redirect("/auth/login");
        }
        Random random = new Random();
        obj.InvoiceId = random.Next(999999, 99999999);
        obj.InvoiceDate = DateTime.Now;
        obj.MemberId = memberId;
                List<Cart> carts = context.Carts.Include(p => p.Product).Where(p => p.MemberId == memberId).ToList();
        obj.Amount = carts.Sum(p => p.Quantity * p.Product!.Price) * 1000;        
        obj.InvoiceDetails = new List<InvoiceDetail>(carts.Count);
        foreach(var item in carts)
        {
            obj.InvoiceDetails.Add(
                new InvoiceDetail
                {
                    InvoiceId = obj.InvoiceId,
                    ProductId = item.ProductId,
                    Price = item.Product!.Price,
                    Quantity = item.Quantity
                }
            );
        }
        context.Invoices.Add(obj);
        int ret = context.SaveChanges();
        if(ret> 0){
            string url = service.ToUrl(obj, HttpContext);
            System.Console.WriteLine("********************");
            System.Console.WriteLine(url);
            return Redirect(url);
        }
        ViewBag.Total = carts.Sum(p => p.Quantity * (p.Product?.Price ?? 0)); // Ensure Price has a default value
ViewBag.Carts = carts;
        return View(obj);
    }

    [HttpPost]
    public IActionResult Add(Cart obj)
{
string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
if (string.IsNullOrEmpty(memberId)) {
    return Redirect("/auth/login");
}

obj.MemberId = memberId;

// Kiểm tra nếu sản phẩm đã tồn tại trong giỏ hàng của thành viên này
Cart? cart = context.Carts.FirstOrDefault(p => p.MemberId == obj.MemberId && p.ProductId == obj.ProductId);

if (cart != null)
{
    // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng và cập nhật thời gian
    cart.Quantity += obj.Quantity;
    cart.UpdatedDate = DateTime.Now;
    context.Carts.Update(cart);  // Cập nhật giỏ hàng
}
else
{
    // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới vào
    obj.CreatedDate = DateTime.Now;
    obj.UpdatedDate = DateTime.Now;
    context.Carts.Add(obj);  // Thêm sản phẩm mới
}

// Lưu thay đổi vào cơ sở dữ liệu
context.SaveChanges();

return Redirect("/cart");
}
    public IActionResult Index()
    {
        ViewBag.Departments = context.Departments.ToList();
        ViewBag.Categories = context.Categories.ToList();

        
        string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(memberId)) {
            return Redirect("/auth/login");
        }
        return View(context.Carts.Include(p => p.Product).Where(p => p.MemberId == memberId).ToList());
    }

    [HttpPost]
public IActionResult Remove(int productId)
{
    string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(memberId))
    {
        return Redirect("/auth/login");
    }

    // Tìm kiếm sản phẩm trong giỏ hàng dựa trên ProductId và MemberId
    var cartItem = context.Carts.FirstOrDefault(c => c.ProductId == productId && c.MemberId == memberId);
    
    if (cartItem != null)
    {
        context.Carts.Remove(cartItem); // Xoá sản phẩm khỏi giỏ hàng
        context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
    }

    return RedirectToAction("Index"); // Trở lại trang giỏ hàng sau khi xoá
}



    [HttpPost]
    public async Task<IActionResult> Edit(int cartId, Cart updatedCart)
    {
        string? memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(memberId))
        {
            return Redirect("/auth/login");
        }

        // Tìm cart bằng cartId và kiểm tra xem nó có thuộc về thành viên hiện tại không
        var cart = await context.Carts
                                .FirstOrDefaultAsync(c => c.CartId == cartId && c.MemberId == memberId);

        if (cart == null)
        {
            return NotFound();
        }

        // Cập nhật thông tin của Cart
        cart.ProductId = updatedCart.ProductId;
        cart.Quantity = updatedCart.Quantity;
        cart.UpdatedDate = DateTime.Now; // Cập nhật thời gian chỉnh sửa

        // Lưu thay đổi
        context.Carts.Update(cart);
        await context.SaveChangesAsync();

        // Sau khi cập nhật thành công, chuyển hướng về trang giỏ hàng hoặc trang khác
        return RedirectToAction("Index");
    }

}
}
