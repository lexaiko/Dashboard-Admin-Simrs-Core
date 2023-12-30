using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace thelast.Pages
{
	public class EditModel : PageModel
	{
		public ClientInfo clientInfo = new ClientInfo();
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
					string sql = "SELECT * FROM TM_PASIEN WHERE id=@id";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@id", id);
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								clientInfo.id = "" + reader.GetInt32(0).ToString();
								clientInfo.nama = reader.GetString(1);
								clientInfo.tgl_lahir = reader.GetDateTime(2);
								clientInfo.jenis_kelamin = reader.GetString(3);
								clientInfo.address = reader.GetString(4);
								clientInfo.nik = reader.GetInt64(5).ToString();
								clientInfo.status = reader.GetBoolean(6);
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
			clientInfo.id = Request.Form["id"];
			clientInfo.nama = Request.Form["nama"];
			clientInfo.jenis_kelamin = Request.Form["jenis_kelamin"];
			clientInfo.address = Request.Form["address"];
			clientInfo.nik = Request.Form["nik"];

			// Konversi tgl_lahir dari StringValues ke DateTime
			if (DateTime.TryParse(Request.Form["tgl_lahir"], out DateTime tgl_lahir))
			{
				clientInfo.tgl_lahir = tgl_lahir; // Tanggal langsung disimpan sebagai DateTime
			}
			else
			{
				errorMessage = $"Format tanggal lahir tidak valid. Input yang diterima: {Request.Form["tgl_lahir"]}";
				return;
			}

			if (clientInfo.nama.Length == 0 ||
				clientInfo.jenis_kelamin.Length == 0 || clientInfo.address.Length == 0)
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

					string sql = "UPDATE TM_PASIEN " +
								 "SET nama=@nama, tgl_lahir=@tgl_lahir, jenis_kelamin=@jenis_kelamin, address=@address, nik=@nik " +
								 "WHERE id=@id";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@nama", clientInfo.nama);
						command.Parameters.AddWithValue("@tgl_lahir", clientInfo.tgl_lahir);
						command.Parameters.AddWithValue("@jenis_kelamin", clientInfo.jenis_kelamin);
						command.Parameters.AddWithValue("@address", clientInfo.address);
						command.Parameters.AddWithValue("@nik", clientInfo.nik);
						command.Parameters.AddWithValue("@id", clientInfo.id);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				return;
			}
			successMessage = "Data berhasil diedit";
		}
	}
}
