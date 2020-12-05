using Prova_Encripitar.Context;
using Prova_Encripitar.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Prova_Encripitar.Controllers
{
    public class EncriptarController : Controller
    {

        private readonly Contexto db = new Contexto();
        private static string AesIV256BD = @"%j?TmFP6$B45lk$@";
        private static string AesKey256BD = @"rxmBUJy]&,;3jKwDTzf(cui$<nc2EQr)";

        #region Inicio

        public ActionResult Home()
        {
            return View();
        }

        #endregion

        #region Index
        // GET: Encriptar
        public ActionResult Index()
        {
            return View(db.encriptar.ToList());
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            return View();
        }
        #endregion

        #region Create - Post

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(EncriptarModel encriptarModel) 
        {
            if (ModelState.IsValid)
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
                aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] src = Encoding.Unicode.GetBytes(encriptarModel.Mensagem);

                using (ICryptoTransform encrypt = aes.CreateEncryptor())
                {
                    byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                    //Converte byte array para string de base 64
                    encriptarModel.Mensagem = Convert.ToBase64String(dest);
                }
                db.encriptar.Add(encriptarModel);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(encriptarModel);
        }

        #endregion

        #region Edit

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            EncriptarModel encriptarModel = db.encriptar.Find(id);
            if (encriptarModel == null)
            {
                return HttpNotFound();
            }

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            //Converter string para um byte array 64bits
            byte[] src = Convert.FromBase64String(encriptarModel.Mensagem);

            //Decripitar
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                encriptarModel.Mensagem = Encoding.Unicode.GetString(dest);
            }

            return View(encriptarModel);
        }

        #endregion

        #region Edit - Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EncriptarModel encriptarModel)
        {
            EncriptarModel encriptar = db.encriptar.Find(encriptarModel.Id);
            encriptarModel.Mensagem = encriptar.Mensagem;


            db.Entry(encriptar).State = EntityState.Detached;

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] src = Encoding.Unicode.GetBytes(encriptarModel.Mensagem);

            using (ICryptoTransform encrypt = aes.CreateEncryptor())
            {
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                //Converte byte array para string de base 64
                encriptarModel.Mensagem = Convert.ToBase64String(dest);
            }


            db.Entry(encriptarModel).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion
    }
}