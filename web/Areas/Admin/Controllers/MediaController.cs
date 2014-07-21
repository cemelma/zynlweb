﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using ImageUploadAndCrop.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using BLL.PhotoBL;
using web.Models;

namespace web.Areas.Admin.Controllers
{
    public class MediaController : Controller
    {
        #region static variables

        private static string ImageCacheKey = "ImageKey_";
        private static int W_FixedSize = 200;
        private static int H_FixedSize = 200;
        
        #endregion

        #region local properties

        private string WorkingImageCacheKey
        {
            get { return string.Format("{0}{1}", ImageCacheKey, WorkingImageId); }
        }
        public string ModifiedImageCacheKey
        {
            get { return string.Format("{0}{1}", ImageCacheKey, ModifiedImageId); }
        }
        
        #endregion

        #region session properties

        private Guid WorkingImageId
        {
            get
            {
                if (Session["WorkingImageId"] != null)
                    return (Guid)Session["WorkingImageId"];
                else
                    return new Guid();
            }
            set { Session["WorkingImageId"] = value; }
        }

        public Guid ModifiedImageId
        {
            get
            {
                if (Session["ModifiedImageId"] != null)
                    return (Guid)Session["ModifiedImageId"];
                else
                    return new Guid();
            }
            set { Session["ModifiedImageId"] = value; }
        }

        public string WorkingImageExtension
        {
            get
            {
                if (Session["WorkingImageExtension"] != null)
                    return Session["WorkingImageExtension"].ToString();
                else
                    return string.Empty;
            }
            set { Session["WorkingImageExtension"] = value; }
        }
        
        #endregion

        #region cached properties

        private byte[] WorkingImage
        {
            get
            {
                byte[] img = null;

                if (HttpContext.Cache[WorkingImageCacheKey] != null)
                    img = (byte[])HttpContext.Cache[WorkingImageCacheKey];

                return img;
            }
            set
            {
                HttpContext.Cache.Add(WorkingImageCacheKey,
                  value,
                  null,
                  System.Web.Caching.Cache.NoAbsoluteExpiration,
                  new TimeSpan(0, 40, 0),
                  System.Web.Caching.CacheItemPriority.Low,
                  null);
            }
        }

        private byte[] ModifiedImage
        {
            get
            {
                byte[] img = null;
                if (HttpContext.Cache[ModifiedImageCacheKey] != null)
                    img = (byte[])HttpContext.Cache[ModifiedImageCacheKey];
                return img;
            }
            set
            {
                HttpContext.Cache.Add(ModifiedImageCacheKey,
                    value,
                    null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    new TimeSpan(0, 40, 0),
                    System.Web.Caching.CacheItemPriority.Low,
                    null);
            }
        }

        #endregion

        private enum ImageModificationType
        {
            Crop,
            Resize
        };        

        /// <summary>
        /// Files the upload.
        /// </summary>
        /// <param name="uploadedFileMeta">The uploaded file meta.</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult FileUpload(MediaAssetUploadModel uploadedFileMeta)
        {
            Guid newImageId = Guid.NewGuid();
            try
            {
                Session["UploadType"] = "Tek Fotoğraf";
                newImageId = ProcessUploadedImage(uploadedFileMeta);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error processing image: {0}", ex.Message);
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

            return Json(new { Id = newImageId, Status = "OK" });
        }

        /// <summary>
        /// Processes the uploaded image.
        /// </summary>
        /// <param name="uploadedFileMeta">The uploaded file meta.</param>
        /// <returns>Image Id</returns>
        private Guid ProcessUploadedImage(MediaAssetUploadModel uploadedFileMeta)
        {
            // Get the file extension
            WorkingImageExtension = Path.GetExtension(uploadedFileMeta.Filename).ToLower();
            string[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif" }; // Make sure it is an image that can be processed
            if (allowedExtensions.Contains(WorkingImageExtension))
            {
                WorkingImageId = Guid.NewGuid();
                Image workingImage = new Bitmap(uploadedFileMeta.fileData.InputStream);
                WorkingImage = ImageHelperNew.ImageToByteArray(workingImage);
            }
            else
            {
                throw new Exception("Cannot process files of this type.");
            }

            return WorkingImageId;
        }
        
        public byte[] GetImageFromServer(string name)
        {
            using (var stream = new FileStream(Server.MapPath(name), FileMode.Open))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public JsonResult ReadImageFromPath(string path)
        {
            try
            {
                WorkingImageExtension = Path.GetExtension(path).ToLower();

                byte[] image;
                image = GetImageFromServer(path);

                WorkingImageId = Guid.NewGuid();
                MemoryStream ms = new MemoryStream(image);
                Image workingImage = Image.FromStream(ms);

                WorkingImage = ImageHelperNew.ImageToByteArray(workingImage);

                return Json(WorkingImageId);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error cropping image: {0}", ex.Message);
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }


        /// <summary>
        /// Crops the image.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        /// <returns>Image Id</returns>
        public JsonResult CropImage(int x, int y, int w, int h)
        {
            try
            {
                if (w == 0 && h == 0) // Make sure the user selected a crop area
                    throw new Exception("A crop selection was not made.");

                string imageId = ModifyImage(x, y, w, h, ImageModificationType.Crop);
                return Json(imageId);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error cropping image: {0}", ex.Message);
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <returns>Image Id</returns>
        public JsonResult ResizeImage()
        {
            try
            {
                string imageId = ModifyImage(0, 0, W_FixedSize, H_FixedSize, ImageModificationType.Resize);
                return Json(imageId);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error resizing image: {0}", ex.Message);
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// Modifies an image image.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        /// <param name="modType">Type of the mod. Crop or Resize</param>
        /// <returns>New Image Id</returns>
        private string ModifyImage(int x, int y, int w, int h, ImageModificationType modType)
        {
            ModifiedImageId = Guid.NewGuid();
            Image img = ImageHelperNew.ByteArrayToImage(WorkingImage);
            int genislik, yukseklik;

            if (Session["UploadType"] == null)
            {
                genislik = Convert.ToInt32(Session["_minwidth2"].ToString());
                yukseklik = Convert.ToInt32(Session["_minheight2"].ToString());
            }
            else
            {
                genislik = Convert.ToInt32(Session["_minwidth"].ToString());
                yukseklik = Convert.ToInt32(Session["_minheight"].ToString());
            }

            using (System.Drawing.Bitmap _bitmap = new System.Drawing.Bitmap(genislik, yukseklik))
            {
                _bitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                using (Graphics _graphic = Graphics.FromImage(_bitmap))
                {
                    _graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    _graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    _graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    _graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    if (modType == ImageModificationType.Crop)
                    {
                        //_graphic.DrawImage(img, 0, 0, w, h);
                        //_graphic.DrawImage(img, new Rectangle(0, 0, w, h), x, y, w, h, GraphicsUnit.Pixel);

                        // Aşağıdaki kodu ben ekliyorum. Crop işleminden sonra resize etmek için..
                        

                        _graphic.DrawImage(img, new Rectangle(0, 0, genislik, yukseklik), x, y, w, h, GraphicsUnit.Pixel);
                        
                    }
                    else if (modType == ImageModificationType.Resize)
                    {
                        _graphic.DrawImage(img, 0, 0, img.Width, img.Height);
                        _graphic.DrawImage(img, new Rectangle(0, 0, W_FixedSize, H_FixedSize), 0, 0, img.Width, 0, GraphicsUnit.Pixel);
                        
                    }

                    string extension = WorkingImageExtension;

                    // If the image is a gif file, change it into png
                    if (extension.EndsWith("gif", StringComparison.OrdinalIgnoreCase))
                    {
                        extension = ".png";
                    }

                    using (EncoderParameters encoderParameters = new EncoderParameters(1))
                    {
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                        ModifiedImage = ImageHelperNew.ImageToByteArray(_bitmap, extension, encoderParameters);
                    }
                }
            }
            
            Image CoreppedImg = ImageHelperNew.ByteArrayToImage(ModifiedImage);
            //CoreppedImg.Save(Server.MapPath("/Content/images/userfiles/newbig/" + ModifiedImageId + WorkingImageExtension));
            CoreppedImg.Save(Server.MapPath("/Content/images/userfiles/news/" + ModifiedImageId));
            return ModifiedImageId.ToString();
        }

        public ActionResult imageUploadAndCrop()
        {
            ImageHelperNew.DestroyImageCashAndSession(0, 0);            
            return View("_imageUploadAndCrop");
        }
    }
}
