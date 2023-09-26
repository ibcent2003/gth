using Classification.DAL;
using Elmah;
using sw= GNSW.DAL;
using Newtonsoft.Json.Linq;
using PAARS.DAL;
using Project.Areas.Setup.Models;
using Project.DAL;
using Project.Properties;
using Semantics3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Project.Areas.Setup.Controllers
{
    public class ClasssificationToolController : Controller
    {      
        // GET: /Setup/ClasssificationTool/
        private sw.GNSWEntities swdb = new sw.GNSWEntities();
        private PROEntities db = new PROEntities();
        private PAARv2Entities paardb = new PAARv2Entities();
        private ClassificationToolsEntities dbclass = new ClassificationToolsEntities();

        public ActionResult Index()
        {
            return base.View(new ClasssificationToolViewModel());
        }

        [HttpPost]
        public ActionResult Index(ClasssificationToolViewModel model)
        {
            ActionResult action;
            try
            {
                if (base.ModelState.IsValid)
                {
                    Products product = new Products(Settings.Default.VUsername, Settings.Default.VPassword);
                    JObject jObjects = new JObject();
                    jObjects["name"] = model.classificationForm.Description;
                    JObject jObjects1 = product.run_query("categorize/hscodes", jObjects, "POST");
                    if ((int)jObjects1["results_count"] > 0)
                    {
                        JArray item = (JArray)jObjects1["results"];
                        int num = 0;
                        while (num < item.Count)
                        {
                            JArray jArrays = (JArray)item[num]["codes"];
                            int num1 = 0;
                            if (num1 < jArrays.Count)
                            {
                                string str = (string)jArrays[num1]["code"];
                                string str1 = str.Substring(0, 4);
                                string str2 = str.Substring(4, 2);
                                string str3 = string.Concat(str1, ".", str2);
                                List<HSCodes> list = (
                                    from x in this.dbclass.HSCodes
                                    where x.Code.StartsWith(str3)
                                    select x).Take<HSCodes>(10).Distinct<HSCodes>().ToList<HSCodes>();
                                model.ClassificationList = list;
                                model.JSonFormat = (JArray)jObjects1["results"];
                                model.HasSearch = true;
                                action = base.View(model);
                                return action;
                            }
                            else
                            {
                                num++;
                            }
                        }
                    }
                }
                action = base.View(model);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                base.TempData["messageType"] = "danger";
                base.TempData["message"] = "There is an error in the application. Please try again or contact the system administrator";
                ErrorSignal.FromCurrentContext().Raise(exception);
                action = base.RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return action;
        }

    }
}
