using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace thelast.Pages.Master
{
    public class EditModel : PageModel
    {
		public DokterInfo dokterInfo = new DokterInfo();
		public string errorMessage = "";
		public string successMessage = "";

		public void OnGet()
		{
			string id = Request.Query["id"];
			try
			{
				string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "SELECT * FROM TM_DOKTER WHERE id=@id";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@id", id);
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								dokterInfo.id = "" + reader.GetInt32(0).ToString();
								dokterInfo.nama = reader.GetString(1);
								dokterInfo.spesialis = reader.GetString(2);
								dokterInfo.jenis_kelamin = reader.GetString(3);
								dokterInfo.nik = reader.GetInt64(4).ToString();
								dokterInfo.status = reader.GetBoolean(5);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}
		}

		public void OnPost()
		{
			dokterInfo.id = Request.Form["id"];
			dokterInfo.nama = Request.Form["nama"];
			dokterInfo.nik = Request.Form["nik"];
			dokterInfo.spesialis = Request.Form["spesialis"];
			dokterInfo.jenis_kelamin = Request.Form["jenis_kelamin"];

			

			if (dokterInfo.nama.Length == 0 ||
				dokterInfo.jenis_kelamin.Length == 0 || dokterInfo.nik.Length == 0)
			{
				errorMessage = "Semua kolom harus diisi";
				return;
			}

			try
			{
				string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();

					string sql = "UPDATE TM_DOKTER " +
								 "SET nama=@nama, nik=@nik, spesialis=@spesialis, jenis_kelamin=@jenis_kelamin " +
								 "WHERE id=@id";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@nama", dokterInfo.nama);
						command.Parameters.AddWithValue("@nik", dokterInfo.nik);
						command.Parameters.AddWithValue("@spesialis", dokterInfo.spesialis);
						command.Parameters.AddWithValue("@jenis_kelamin", dokterInfo.jenis_kelamin);
						command.Parameters.AddWithValue("@id", dokterInfo.id);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				return;
			}
			Response.Redirect("/Master/Dokter");
		}
	}
}