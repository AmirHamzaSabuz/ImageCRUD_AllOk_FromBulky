using Experiment_Image_Bulky.Data;
using Experiment_Image_Bulky.Models;
using Experiment_Image_Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System;

namespace Experiment_Image_Bulky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            var objProductList = _context.Products.ToList();
            return View(objProductList);
        }



        //GET
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),

            };
            if (id == null || id == 0)
            {
                //create product

                return View(productVM);
            }
            else
            {
                productVM.Product = _context.Products.FirstOrDefault(u => u.Id == id);
                return View(productVM);
                //update product
            }


        }



        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (obj.Product.Id == 0) // Upload or Create
            {
                if (ModelState.IsValid && file != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    //if (file != null)
                    //{
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);


                    //if (obj.Product.ImageUrl != null) // For Create ImageURL always Null
                    //{
                    //    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    //    if (System.IO.File.Exists(oldImagePath))
                    //    {
                    //        System.IO.File.Delete(oldImagePath);
                    //    }
                    //}

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;

                    //}

                    //if (obj.Product.Id == 0)  // For Create Id always 0
                    //{
                    _context.Products.Add(obj.Product);
                    //}
                    //else
                    //{
                    //    _context.Products.Update(obj.Product);
                    //}

                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            else
            {
                if (ModelState.IsValid) // Edit
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(wwwRootPath, @"images\products");
                        var extension = Path.GetExtension(file.FileName);


                        //if (obj.Product.ImageUrl != null) // for update always imageUrl Exist or Not null
                        //{
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        // }

                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            file.CopyTo(fileStreams);
                        }
                        obj.Product.ImageUrl = @"\images\products\" + fileName + extension;

                    }

                    //if (obj.Product.Id == 0)
                    //{
                    //    _context.Products.Add(obj.Product);
                    //}
                    //else // For Update Id always Not 0.
                    //{
                    _context.Products.Update(obj.Product);
                    //}

                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(obj);

        }

        /*
        * For Create If File Exist, No Url Exist
        * For Create If File Not Exist. No URL Exist
        * For Create URL Always Null. So No URL "Delete" Code.
        * 
        * For Create If File Exist, No URL Delete Code, Because There is No Previous URL. Write New URL.
        * For Create If File Not Exist, Not Valid.
        * 
        * For Edit/Update If File Exist, Url Exist
        * For Edit/Update If File Not exist, Url Exist.
        * For Edit/Update URL Always Exist. 
        * 
        * For Edit/Update All Url Code Works When File Exist. Or Not Null
        * For Edit/Update All Url Code Will Not Work When File Not Exist. Or Null. 
        * 
        * For Edit/Update If File Exist or Not Null, All URL Code will work. Delete Previous URL. Make New URL.
        * For Edit/Update If File Not Exist or Null, All URL Code will not work. Update/Save with previous URL. No Delete No Write.
        * 
        * */


        /*
        * For Create, If File Not Exist, Invalid
        * For Create, If File Exist, Don't Delete URL, Because there is No Previus URL. Write New URL.
        * 
        * 
        * For Edit, If New File Not Exist, Previous Url Exist. Don't Delete Previous Url. Don't Write New Url. Save/Update with Previous Url.
        * For Edit, If New File Exist, Previous Url Exist. Delete Previous Url.  Write New Url. Save/Update with New Url.
        * */




        [HttpGet]
        public IActionResult Delete(int id)
        {
            var image = _context.Products.Find(id);

            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = _context.Products.Find(id);

            //delete image from wwwroot/image
            var path = _hostEnvironment.WebRootPath;
            var filePath = "images/products/" + image.ImageUrl;
            var imagePath = Path.Combine(path, filePath);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
            //delete the record
            _context.Products.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
