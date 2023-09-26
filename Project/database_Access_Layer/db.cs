using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace Project.database_Access_Layer
{
    public class db
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public DataSet GetName(string prefix)
        {
            SqlCommand com = new SqlCommand("Select distinct VehicleType from ValuationTPD where VehicleType like '%'+@prefix+'%'", con);
            com.Parameters.AddWithValue("@prefix", prefix);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(com);
            da.Fill(ds);
            return ds;
        }


        public DataSet GetVname(string vname)
        {
            SqlCommand com = new SqlCommand("Select distinct Make from ValuationTPD where Make like '%'+@vname+'%'", con);
            com.Parameters.AddWithValue("@vname", vname);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(com);
            da.Fill(ds);
            return ds;
        }


        public DataSet GetModel(string modelname)
        {
            SqlCommand com = new SqlCommand("Select distinct Model from ValuationTPD where Model like '%'+@modelname+'%'", con);
            com.Parameters.AddWithValue("@modelname", modelname);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(com);
            da.Fill(ds);
            return ds;
        }


        public DataSet GetManufactureYear(string myear)
        {
            SqlCommand com = new SqlCommand("Select distinct ManufactureYear  from ValuationTPD where ManufactureYear  like '%'+@myear+'%'", con);
            com.Parameters.AddWithValue("@myear", myear);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(com);
            da.Fill(ds);
            return ds;
        }


        public DataSet GetEngineCapacity(string cc)
        {
            SqlCommand com = new SqlCommand("Select distinct EngineCapacity  from ValuationTPD where EngineCapacity  like '%'+@cc+'%'", con);
            com.Parameters.AddWithValue("@cc", cc);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(com);
            da.Fill(ds);
            return ds;
        }

        public DataSet GetHDV(decimal hdv)
        {
            SqlCommand com = new SqlCommand("Select distinct HDV  from ValuationTPD where HDV  like '%'+@hdv+'%'", con);
            com.Parameters.AddWithValue("@hdv", hdv);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(com);
            da.Fill(ds);
            return ds;
        }


        public DataSet GetSearch(string VehicleTypeName, string VehicleMake, string VehicleModel)
        {
            SqlCommand com = new SqlCommand("Select distinct ManufactureYear,model,VehicleType,Make  from ValuationTPD where(ManufactureYear=@VehicleTypeName) and (Make=@VehicleMake) and (model like '%'+@VehicleModel+'%')", con);
            com.Parameters.AddWithValue("@VehicleTypeName", VehicleTypeName);
            com.Parameters.AddWithValue("@VehicleMake", VehicleMake);
            //com.Parameters.AddWithValue("@VehicleYear", VehicleYear);
            com.Parameters.AddWithValue("@VehicleModel", VehicleModel);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(com);
            da.Fill(ds);
            return ds;
        }


      //  SELECT distinct     Make,   ,ManufactureYear, model
      //FROM            ValuationTPD
      //WHERE(VehicleType = 'saloon') AND(Make = 'HONDA') and ManufactureYear = '2015' and(Model LIKE '%Accord%')


    }
}