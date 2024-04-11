using BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrewSupportDemo.Controllers
{
    public class CrewSupportController : Controller
    {
        // GET: CrewSupport
        public ActionResult CrewSupportDetails()
        {

            if (!System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains("UserName"))
            {
                System.Web.HttpContext.Current.Response.Redirect("/Login/Login", false);
                return null;
            }
            else
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("FirstName");
                HttpCookie cookie2 = HttpContext.Request.Cookies.Get("LastName");
                ViewBag.UserFullName = cookie.Value + " " + cookie2.Value;
                CrewSupportBLL objcrewsupportBLL = new CrewSupportBLL();
                DataSet ds = objcrewsupportBLL.BindGrid("");
                List<GridDetail> ltGrid = new List<GridDetail>();
                var Griddetail = ds.Tables[1].AsEnumerable()
                .Select(dataRow => new GridDetail
                {
                    pkPilotId = dataRow.Field<int>("pkPilotId"),
                    PilotFname = dataRow.Field<string>("PilotFname"),
                    PilotLName = dataRow.Field<string>("PilotLName"),
                    cellNumber = dataRow.Field<string>("cellNumber"),
                    CuLocCountry = dataRow.Field<string>("CuLocCountry"),
                    EmailId = dataRow.Field<string>("EmailId"),
                    MembershipType = dataRow.Field<string>("MembershipType"),
                    PilotGender = dataRow.Field<string>("PilotGender"),
                }).ToList();
                objcrewsupportBLL.OwnerCount = Convert.ToString(ds.Tables[0].Rows[0]["OwnerCount"]);
                objcrewsupportBLL.PilotCount = Convert.ToString(ds.Tables[0].Rows[0]["PilotCount"]);
                objcrewsupportBLL.SICCount = Convert.ToString(ds.Tables[0].Rows[0]["SICCount"]);
                objcrewsupportBLL.FACount = Convert.ToString(ds.Tables[0].Rows[0]["FACount"]);
                objcrewsupportBLL.InstructorCount = Convert.ToString(ds.Tables[0].Rows[0]["InstructorCount"]);

                objcrewsupportBLL.lstgrid = Griddetail;
                return View(objcrewsupportBLL);
            }

        }

        [HttpPost]
        public JsonResult BindGrid(string status)
        {
            try
            {
                CrewSupportBLL objcrewsupportBLL = new CrewSupportBLL();
                DataSet ds = objcrewsupportBLL.BindGrid(status);

                List<GridDetail> ltGrid = new List<GridDetail>();
                var Griddetail = ds.Tables[1].AsEnumerable()
                .Select(dataRow => new GridDetail
                {
                    pkPilotId = dataRow.Field<int>("pkPilotId"),
                    PilotFname = dataRow.Field<string>("PilotFname"),
                    PilotLName = dataRow.Field<string>("PilotLName"),
                    cellNumber = dataRow.Field<string>("cellNumber"),
                    CuLocCountry = dataRow.Field<string>("CuLocCountry"),
                    EmailId = dataRow.Field<string>("EmailId"),
                    MembershipType = dataRow.Field<string>("MembershipType"),
                    PilotGender = dataRow.Field<string>("PilotGender"),
                    IsVoid = dataRow.Field<bool>("IsVoid"),
                }).ToList();
                objcrewsupportBLL.OwnerCount = Convert.ToString(ds.Tables[0].Rows[0]["OwnerCount"]);
                objcrewsupportBLL.PilotCount = Convert.ToString(ds.Tables[0].Rows[0]["PilotCount"]);
                objcrewsupportBLL.SICCount = Convert.ToString(ds.Tables[0].Rows[0]["SICCount"]);
                objcrewsupportBLL.FACount = Convert.ToString(ds.Tables[0].Rows[0]["FACount"]);
                objcrewsupportBLL.InstructorCount = Convert.ToString(ds.Tables[0].Rows[0]["InstructorCount"]);

                objcrewsupportBLL.lstgrid = Griddetail;

                return Json(objcrewsupportBLL);
            }
            catch (Exception ex)
            {
                return Json("Error");
            }
        }

        [HttpPost]
        public JsonResult GetPilotFullDetail(string pkPilotId)
        {
            try
            {
                CrewSupportBLL objcrewsupportBLL = new CrewSupportBLL();
                DataSet ds = objcrewsupportBLL.GetPilotFullDetail(pkPilotId);

                List<GridDetail> ltGrid = new List<GridDetail>();
                var Griddetail = ds.Tables[0].AsEnumerable()
                .Select(dataRow => new GridDetail
                {
                    pkPilotId = dataRow.Field<int>("pkPilotId"),
                    PilotFname = dataRow.Field<string>("PilotFname"),
                    PilotLName = dataRow.Field<string>("PilotLName"),
                    cellNumber = dataRow.Field<string>("cellNumber"),
                    CuLocCountry = dataRow.Field<string>("CuLocCountry"),
                    EmailId = dataRow.Field<string>("EmailId"),
                    MembershipType = dataRow.Field<string>("MembershipType"),
                    PilotGender = dataRow.Field<string>("PilotGender"),
                    IsVoid = dataRow.Field<bool>("IsVoid"),
                }).ToList();

                objcrewsupportBLL.lstgrid = Griddetail;

                return Json(objcrewsupportBLL);
            }
            catch (Exception ex)
            {
                return Json("Error");
            }
        }

        [HttpPost]
        public JsonResult P_WebsiteActiveDeActivePilot(string status, string PilotId)
        {
            try
            {
                bool st = (status.ToString() == "true" ? true : false);
                CrewSupportBLL objcrewsupportBLL = new CrewSupportBLL();
                int res = objcrewsupportBLL.P_WebsiteActiveDeActivePilot(st, Convert.ToInt32(PilotId));

                return Json(objcrewsupportBLL);
            }
            catch (Exception ex)
            {
                return Json("Error");
            }
        }


    }
}