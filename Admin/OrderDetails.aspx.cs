using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

public partial class OrderHistory : System.Web.UI.Page
{
    DataSet ds;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            fillgrid();
        }
    }

    protected void fillgrid()
    {
        if (!string.IsNullOrEmpty(Request.QueryString["invo"]))
        {
            int odid;
            if (int.TryParse(Request.QueryString["invo"], out odid))
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["GroceryDB"].ConnectionString);
                SqlCommand cmd = new SqlCommand(@"SELECT * FROM [Order] WHERE Order_Code=@OrderCode", cn);
                cmd.Parameters.AddWithValue("@OrderCode", odid);
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                cn.Close();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    lblOrderCode.Text = ds.Tables[0].Rows[0]["order_code"].ToString();
                    lblOrderTime.Text = ds.Tables[0].Rows[0]["order_time"].ToString();
                    lblCustomerName.Text = ds.Tables[0].Rows[0]["Name"].ToString();
                    lblAddress.Text = ds.Tables[0].Rows[0]["Address"].ToString();
                    lblCity.Text = ds.Tables[0].Rows[0]["City"].ToString();
                    lblState.Text = ds.Tables[0].Rows[0]["State"].ToString();
                    lblPostalCode.Text = ds.Tables[0].Rows[0]["PostalCode"].ToString();
                    lblGrandTotal.Text = ds.Tables[0].Rows[0]["GrandTotal"].ToString();

                    SqlConnection cn1 = new SqlConnection(ConfigurationManager.ConnectionStrings["GroceryDB"].ConnectionString);
                    SqlCommand cmd1 = new SqlCommand(@"SELECT Productname, ImageURL, Unit, CategoryType, Qty, OrderedProducts.Price, TotalAmount 
                                                       FROM Product, CategoryMaster, OrderedProducts 
                                                       WHERE CategoryMaster.CategoryID = Product.CategoryID 
                                                       AND OrderedProducts.Prod_ID = Product.ProductID 
                                                       AND OrderCode = @OrderCode", cn1);
                    cmd1.Parameters.AddWithValue("@OrderCode", odid);
                    cn1.Open();
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    ds = new DataSet();
                    da1.Fill(ds);
                    cn1.Close();

                    gvorderHistory.DataSource = ds;
                    gvorderHistory.DataBind();
                }
                else
                {
                    // Handle case where no order is found
                    lblMessage.Text = "No order found with the provided order code.";
                }
            }
            else
            {
                // Handle case where the query string parameter is not a valid integer
                lblMessage.Text = "Invalid order code.";
            }
        }
        else
        {
            // Handle case where the query string parameter is missing or empty
            lblMessage.Text = "Order code is missing.";
        }
    }
}