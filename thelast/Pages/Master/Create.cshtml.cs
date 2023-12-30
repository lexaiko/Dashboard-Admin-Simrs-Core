using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace thelast.Pages.Master
{
    public class CreateModel : PageModel
    {
		public DokterInfo dokterInfo = new DokterInfo();
		public string errorMessage = "";
		public string successMessage = "";

		public void OnPost()
		{
			try
			{
				string nama = Request.Form["nama"];
				string nik = Request.Form["nik"];
				string spesialis = Request.Form["spesialis"];
				string jenis_kelamin = Request.Form["jenis_kelamin"];
				

				if (string.IsNullOrEmpty(nama) || string.IsNullOrEmpty(jenis_kelamin) ||
					string.IsNullOrEmpty(nik) || string.IsNullOrEmpty(spesialis))
				{
					errorMessage = "Semua kolom harus diisi";
					return;
				}



				string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();

					string sql = "INSERT INTO TM_DOKTER " +
								 "(nama, nik, spesialis, jenis_kelamin, status) VALUES " +
								 "(@nama, @nik, @spesialis, @jenis_kelamin, @status)";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@nama", nama);
						command.Parameters.AddWithValue("@nik", nik);
						command.Parameters.AddWithValue("@jenis_kelamin", jenis_kelamin);
						command.Parameters.AddWithValue("@spesialis", spesialis);
						command.Parameters.AddWithValue("@status", 0); // Default status is set to 0

						command.ExecuteNonQuery();
					}
				}

				successMessage = "Data Dokter baru berhasil ditambahkan";
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}
		
		}
	}
}