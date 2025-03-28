using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

public partial class VerifyEmail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string token = Request.QueryString["token"];
        if (!string.IsNullOrEmpty(token))
        {
            VerifyEmailToken(token);
        }
        else
        {
            lblMsg.Text = "Invalid verification link.";
        }
    }

    private void VerifyEmailToken(string token)
    {
        string constr = ConfigurationManager.ConnectionStrings["GroceryDB"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            string query = "UPDATE Users SET IsEmailVerified = 1 WHERE EmailVerificationToken = @Token";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Token", token);
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    lblMsg.Text = "Email verification successful! You can now log in.";
                }
                else
                {
                    lblMsg.Text = "Invalid verification link.";
                }
            }
        }
    }
}