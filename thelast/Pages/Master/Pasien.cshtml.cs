using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace thelast.Pages.Master
{
    public class PasienModel : PageModel
	{
		public List<ClientInfo> listClients = new List<ClientInfo>();

		public void OnGet()
		{
			try
			{
				string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "SELECT * FROM TM_PASIEN";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								ClientInfo clientInfo = new ClientInfo();
								clientInfo.id = "" + reader.GetInt32(0).ToString();
								clientInfo.nama = reader.GetString(1);
								clientInfo.tgl_lahir = reader.GetDateTime(2);
								clientInfo.jenis_kelamin = reader.GetString(3) == "L" ? "Laki-laki" : "Perempuan";
								clientInfo.address = reader.GetString(4);
								clientInfo.nik = reader.GetInt64(5).ToString();
								clientInfo.status = !reader.IsDBNull(6) && reader.GetBoolean(6);

								listClients.Add(clientInfo);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.ToString());
				// Biasanya, di sini Anda akan menangani atau melacak kesalahan secara lebih tepat
			}
		}
	}

	public class ClientInfo
	{
		public string id;
		public string nama;
		public DateTime tgl_lahir; // Ubah tipe data ke DateTime jika sesuai dengan data pada tabel
		public string jenis_kelamin;
		public string address;
		public string nik; // Tambah properti nik jika diperlukan
		public bool status; // Tambah properti status jika diperlukan
	}
}
