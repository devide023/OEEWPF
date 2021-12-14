using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebOEE.Models;
using WebOEE.Services;
namespace WebOEE.Controllers
{
    public class BZJPController : Controller
    {
        private sbbzjpService _service = null;
        public BZJPController()
        {
            _service = new sbbzjpService();
        }
        // GET: BZJP
        public ActionResult Index()
        {
            var list = _service.SBXX_List();
            List<string> qylist = new List<string>();
            qylist.Add("A");
            qylist.Add("B");
            qylist.Add("C");
            qylist.Add("D");
            qylist.Add("E");
            qylist.Add("F");
            ViewBag.list = qylist;
            return View(list) ;
        }
        [HttpPost]
        public ActionResult Save(List<sbbzjp> entitys)
        {
            try
            {
                var ok = _service.Save_SBBZJP(entitys);
                if (ok)
                {
                    return Json(new { code = 1, msg = "数据保存成功" });
                }
                else
                {
                    return Json(new { code = 0, msg = "数据保存失败" });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}