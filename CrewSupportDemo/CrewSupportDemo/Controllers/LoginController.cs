using BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrewSupportDemo.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        public void LoginCommand(string username, string password)
        {
            try
            {
                LoginBLL objLogin = new LoginBLL();
                objLogin.UserName = username;
                objLogin.Password = password;
                DataSet dsLogin = objLogin.CheckLogin();
                if (dsLogin != null)
                {
                    if (dsLogin.Tables[0].Rows.Count > 0)
                    {
                        HttpCookie UserName = new HttpCookie("UserName");
                        UserName.Value = Convert.ToString(dsLogin.Tables[0].Rows[0]["UserName"]);
                        UserName.Expires = System.DateTime.Now.AddDays(1);
                        Response.Cookies.Add(UserName);

                        HttpCookie UserId = new HttpCookie("UserId");
                        UserId.Value = Convert.ToString(dsLogin.Tables[0].Rows[0]["PkId"]);
                        UserId.Expires = System.DateTime.Now.AddDays(1);
                        Response.Cookies.Add(UserId);

                        HttpCookie FirstName = new HttpCookie("FirstName");
                        FirstName.Value = Convert.ToString(dsLogin.Tables[0].Rows[0]["FirstName"]);
                        FirstName.Expires = System.DateTime.Now.AddDays(1);
                        Response.Cookies.Add(FirstName);


                        HttpCookie LastName = new HttpCookie("LastName");
                        LastName.Value = Convert.ToString(dsLogin.Tables[0].Rows[0]["LastName"]);
                        LastName.Expires = System.DateTime.Now.AddDays(1);
                        Response.Cookies.Add(LastName);

                        if (!string.IsNullOrEmpty(Convert.ToString(dsLogin.Tables[0].Rows[0]["PkId"])))
                            Response.Redirect("/CrewSupport/CrewSupportDetails", false);
                        else
                            Response.Redirect("/Login/Login", false);

                    }
                    else
                    {
                        Session["Message"] = "<div class='alert alert-error'><button type='button' class='close' data-dismiss='alert'>x</button><strong>Login failed.</strong> Please enter valid username and password.</div>";
                        Response.Redirect("/Login/Login", false);
                    }
                }
                else
                {
                    Session["Message"] = "<div class='alert alert-error'><button type='button' class='close' data-dismiss='alert'>x</button><strong>Login failed.</strong> Please enter valid username and password.</div>";
                    Response.Redirect("/Login/Login", false);
                }
            }

            catch (Exception ex)
            {
                throw;
            }
            finally
            { }
        }
        public void Logout()
        {
            try
            {
                if (Request.Cookies["UserName"] != null)
                {
                    HttpCookie UserName = Request.Cookies["UserName"];
                    UserName.Value = null;
                    UserName.Expires = System.DateTime.Now.AddDays(-1);
                    Response.SetCookie(UserName);
                    Response.Cookies.Add(UserName);
                }

                if (Request.Cookies["UserId"] != null)
                {
                    HttpCookie UserId = Request.Cookies["UserId"];
                    UserId.Value = null;
                    UserId.Expires = System.DateTime.Now.AddDays(-1);
                    Response.SetCookie(UserId);
                    Response.Cookies.Add(UserId);
                }
                if (Request.Cookies["FirstName"] != null)
                {
                    HttpCookie FirstName = Request.Cookies["FirstName"];
                    FirstName.Value = null;
                    FirstName.Expires = System.DateTime.Now.AddDays(-1);
                    Response.SetCookie(FirstName);
                    Response.Cookies.Add(FirstName);
                }
                if (Request.Cookies["LastName"] != null)
                {
                    HttpCookie LastName = Request.Cookies["LastName"];
                    LastName.Value = null;
                    LastName.Expires = System.DateTime.Now.AddDays(-1);
                    Response.SetCookie(LastName);
                    Response.Cookies.Add(LastName);
                }
                Response.Redirect("/Login/Login", false);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}