using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net; 
public partial class Registration : System.Web.UI.Page
{
    public static string exi = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (exi == "Exists")
        {
            lblMsg.Text = "UserEmail Already Exists, Please register with Other Email !!";
            exi = string.Empty;
        }
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
      
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["GroceryDB"].ConnectionString);
        SqlCommand cmd0 = new SqlCommand(@"select case when (select 1 from Registration where Email='"+txtemail.Text.Trim()+"')=1 then 1 else 0 end", cn);
        SqlCommand cmd = new SqlCommand(@"Insert into Registration values ('" + txtname.Text.Trim() + "','" + txtemail.Text.Trim() + "','" + txtpass.Text.Trim() + "','" + rblGender.SelectedValue + "'," + txtphone.Text.Trim() + ",'"+ txtclendar.Text +"','"
        +txtstreet.Text.Trim()+"','"+ddlcountry.SelectedValue+"','"+txtcity.Text.Trim()+"','"+country.SelectedValue+"','"+txtPostal.Text.Trim()+"',1)", cn);
        SqlCommand cmd1 = new SqlCommand(@"insert into Login Values('" + txtemail.Text.Trim() + "','" + txtpass.Text.Trim() + "',getdate())", cn);
        cn.Open();
        int exists = (int)cmd0.ExecuteScalar();
        if (exists == 1)
        {
            exi = "Exists";
            Response.Redirect(Request.RawUrl);
        }
        else
        {
            Session["UserEmail"] = txtemail.Text.Trim();
            cmd.ExecuteNonQuery();
            cmd1.ExecuteNonQuery();
             SendMail(); 
            Response.Redirect("~/Online Grocery Shop/RegistrationSuccessfull.aspx");
        }
        cn.Close();
        
    }
    private void SendMail()
    {
       
         using (MailMessage msg = new MailMessage())
        {
            msg.From = new MailAddress(ConfigurationManager.AppSettings["SmtpUser"]);
            msg.To.Add(txtemail.Text);
            msg.Subject = "Confirmation";
            msg.Body = "Successfully registered with Fresho Fruit Mart";
            msg.IsBodyHtml = true;

            using (SmtpClient smtp = new SmtpClient())
            {
                 smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["SmtpUser"],
                    ConfigurationManager.AppSettings["SmtpPassword"]
                );
                smtp.Send(msg);
            }
    }
    }
    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }
}