using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_AddAdmin : System.Web.UI.Page
{
    public static string show = string.Empty;
    DataSet ds;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if the session is valid
        if (Session["Admin"] != null) // Admin is logged in
        {
            HandleNotifications();
            if (!IsPostBack)
            {
                fillgrid();
            }
        }
        else
        {
            // Redirect to the login page if the session is null
            Response.Redirect("~/Online Grocery Shop/Admin/Login.aspx");
        }
    }

    private void HandleNotifications()
    {
        switch (show)
        {
            case "Delete":
                lblMsg.Text = "Admin removed Successfully!";
                break;
            case "Add":
                lblMsg.Text = "Admin Added Successfully!";
                break;
            case "Exists":
                lblMsg.Text = "Admin UserName Already Exists!";
                break;
        }
        show = string.Empty;
    }

    protected void gvAdmin_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow row = gvAdmin.Rows[e.RowIndex];
        Label lblAid = (Label)row.FindControl("lblAid");

        using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["GroceryDB"].ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM AdminUsers WHERE AdminID = @AdminID", cn);
            cmd.Parameters.AddWithValue("@AdminID", lblAid.Text.Trim());

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        show = "Delete";
        Response.Redirect(Request.RawUrl);
    }

    protected void fillgrid()
    {
        using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["GroceryDB"].ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM AdminUsers", cn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            gvAdmin.DataSource = ds;
            gvAdmin.DataBind();
        }
    }

    protected void btnAddAdmin_Click(object sender, EventArgs e)
    {
        using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["GroceryDB"].ConnectionString))
        {
            SqlCommand cmd0 = new SqlCommand("SELECT COUNT(1) FROM AdminUsers WHERE UserName = @UserName", cn);
            cmd0.Parameters.AddWithValue("@UserName", txtAuser.Text.Trim());

            SqlCommand cmd = new SqlCommand("INSERT INTO AdminUsers (UserName, Password) VALUES (@UserName, @Password)", cn);
            cmd.Parameters.AddWithValue("@UserName", txtAuser.Text.Trim());
            cmd.Parameters.AddWithValue("@Password", txtApwd.Text.Trim());

            cn.Open();
            int i = (int)cmd0.ExecuteScalar();
            if (i == 1)
            {
                show = "Exists";
            }
            else
            {
                cmd.ExecuteNonQuery();
                show = "Add";
            }
        }

        Response.Redirect(Request.RawUrl);
    }
}
