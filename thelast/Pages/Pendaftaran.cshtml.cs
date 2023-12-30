using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace thelast.Pages
{
	public class PendaftaranModel : PageModel
	{
		public ClientInfo clientInfo = new ClientInfo();
		public string errorMessage = "";
		public string successMessage = "";
		public List<(int id, string nama, int biaya)> ListIDDokter { get; set; }

		public PendaftaranModel()
		{
			ListIDDokter = new List<(int id, string nama, int biaya)>();
		}

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

			ListIDDokter = new List<(int id, string nama, int biaya)>();
			try
			{
				string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "SELECT id, nama, biaya FROM TM_DOKTER";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								ListIDDokter.Add((reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
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
			string id_pasien = Request.Form["id_pasien"];
			string tgl_datangString = Request.Form["tgl_datang"];
			string id_dokter = Request.Form["id_dokter"];
			string biaya = Request.Form["biaya"];

			DateTime tgl_datang;
			if (!DateTime.TryParse(tgl_datangString, out tgl_datang))
			{
				errorMessage = $"Format tanggal lahir tidak valid. Input yang diterima: {tgl_datangString}";
				return;
			}

			try
			{
				string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();

					string checkStatusQuery = "SELECT status FROM TM_PASIEN WHERE id = @id_pasien";
					using (SqlCommand checkStatusCommand = new SqlCommand(checkStatusQuery, connection))
					{
						checkStatusCommand.Parameters.AddWithValue("@id_pasien", id_pasien);
						bool status = (bool)(checkStatusCommand.ExecuteScalar() ?? false);

						if (status)
						{
							errorMessage = "Pasien sedang dalam kunjungan. Tidak dapat mendaftar kembali.";
							return;
						}
					}

					string insertQuery = "INSERT INTO TT_PASIENIGD " +
										 "(id_pasien, tgl_datang, id_dokter, biaya, status_bayar) VALUES " +
										 "(@id_pasien, @tgl_datang, @id_dokter, @biaya, @status_bayar)";

					using (SqlCommand command = new SqlCommand(insertQuery, connection))
					{
						command.Parameters.AddWithValue("@id_pasien", id_pasien);
						command.Parameters.AddWithValue("@tgl_datang", tgl_datang);
						command.Parameters.AddWithValue("@id_dokter", id_dokter);
						command.Parameters.AddWithValue("@biaya", biaya);
						command.Parameters.AddWithValue("@status_bayar", 0);

						command.ExecuteNonQuery();
					}

					string updateStatusQuery = "UPDATE TM_PASIEN SET status = 1 WHERE id = @id_pasien";
					using (SqlCommand statusCommand = new SqlCommand(updateStatusQuery, connection))
					{
						statusCommand.Parameters.AddWithValue("@id_pasien", id_pasien);
						statusCommand.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				return;
			}

			successMessage = "Pendaftaran IGD berhasil ditambahkan";
			Response.Redirect("/Pelayanan/Antrian");
		}
	}
}