using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EFW_MVC_APP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Item()
        {
            using (var db = new ABBASI_FURNITURE_Entities())
            {
                ViewBag.Message = "Your application description page.";
                ViewBag.model = db.Items.ToList();
            }
            return View();
        }

        [HttpGet]
        public ActionResult ItemEntry()
        {
            return View("ItemEntry", "_Layout");
        }

        [HttpPost]
        public ActionResult ItemEntry(Item per, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ABBASI_FURNITURE_Entities())
                {

                    var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", ".jpeg" };
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = name + "_" + System.Guid.NewGuid().ToString() + ext; //appending the name with id  
                        var path1 = Path.Combine(Server.MapPath("~/Image"), myfile);
                        file.SaveAs(path1);
                        var path = Path.Combine("../../Image/", myfile);
                        per.Image_URL = path;
                        db.Items.Add(per);
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.message = "Please choose only Image file";
                    }
                }

                ViewBag.Message = "Your application description page.";

                return RedirectToAction("Index");
            }

            ViewBag.Message = "Your application description page.";
            return View(per);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (var db = new ABBASI_FURNITURE_Entities())
            {
                Item student = db.Items.Where(s => s.ItemId == id).FirstOrDefault();
                db.Items.Remove(student);
                db.SaveChanges();
            }

            ViewBag.Message = "Your application description page.";

            using (var db = new ABBASI_FURNITURE_Entities())
            {
                ViewBag.Message = "Your application description page.";
                ViewBag.model = db.Items.ToList();
            }

            return View("Item");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var db = new ABBASI_FURNITURE_Entities())
            {
                Item student = db.Items.Where(s => s.ItemId == id).FirstOrDefault();

                ViewBag.Message = "Your application description page.";
                ViewBag.model = student;
            }

            return View("Edit");
        }

        [HttpPost]
        public ActionResult Edit(Item per, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ABBASI_FURNITURE_Entities())
                {
                    if ( file == null && db.Items.Where(s => s.ItemId == per.ItemId).FirstOrDefault().Image_URL != null && per.ItemName != null && per.Price > 0)
                    {
                        Item student = db.Items.Where(s => s.ItemId == per.ItemId).FirstOrDefault();
                        student.ItemName = per.ItemName;
                        student.Price = per.Price;
                        student.Image_URL = db.Items.Where(s => s.ItemId == per.ItemId).FirstOrDefault().Image_URL;

                        db.Entry(per).State = EntityState.Detached;
                        db.Entry(student).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (file != null && per.ItemName != null && per.Price > 0)
                        {
                            var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", ".jpeg" };
                            var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                            var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                            if (allowedExtensions.Contains(ext)) //check what type of extension  
                            {
                                string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                                string myfile = name + "_" + System.Guid.NewGuid().ToString() + ext; //appending the name with id  
                                var path1 = Path.Combine(Server.MapPath("~/Image"), myfile);
                                file.SaveAs(path1);
                                var path = Path.Combine("../../Image/", myfile);

                                Item student = db.Items.Where(s => s.ItemId == per.ItemId).FirstOrDefault();
                                student.ItemName = per.ItemName;
                                student.Price = per.Price;
                                student.Image_URL = path;

                                db.Entry(per).State = EntityState.Detached;
                                db.Entry(student).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                ViewBag.message = "Please choose only Image file";
                            }
                        }
                    }
                }

                ViewBag.Message = "Your application description page.";

                return RedirectToAction("Index");
            }

            ViewBag.Message = "Your application description page.";
            return View(per);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserProfile objUser)
        {
            if (ModelState.IsValid)
            {
                using (ABBASI_FURNITURE_Entities db = new ABBASI_FURNITURE_Entities())
                {
                    string pass = Encryption_Decryption.Encrypt(objUser.Password, "/A?D(G+KbPeShVmYq3s6v9y$B&E)H@Mc");
                    var obj = db.UserProfiles.Where(a => a.UserName.Equals(objUser.UserName) && a.Password.Equals(pass)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.UserId.ToString();
                        Session["UserName"] = obj.UserName.ToString();
                        ViewBag.Message = "Your application description page.";
                        ViewBag.model = db.Items.ToList();

                        return View("Item");
                    }
                }
            }
            return View(objUser);
        }

        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session.Abandon();
            return View("Index");
        }

        public ActionResult ContactUs()
        {
            return View();
        }
    }
}