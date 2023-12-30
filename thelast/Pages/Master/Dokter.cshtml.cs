using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace thelast.Pages.Master
{
    public class DokterModel : PageModel
    {
		public List<DokterInfo> listDokter = new List<DokterInfo>();

		public void OnGet()
		{
			try
			{
				string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string sql = "SELECT * FROM TM_DOKTER";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								DokterInfo dokterInfo = new DokterInfo();
								dokterInfo.id = "" + reader.GetInt32(0).ToString();
								dokterInfo.nama = reader.GetString(1);
								dokterInfo.spesialis = reader.GetString(2);
								dokterInfo.jenis_kelamin = reader.GetString(3) == "L" ? "Laki-laki" : "Perempuan";
								dokterInfo.nik = reader.GetInt64(4).ToString();
								dokterInfo.status = !reader.IsDBNull(5) && reader.GetBoolean(5);

								listDokter.Add(dokterInfo);
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

	public class DokterInfo
	{
		public string id;
		public string nama;
		public string spesialis; // Ubah tipe data ke DateTime jika sesuai dengan data pada tabel
		public string jenis_kelamin;
		public string nik; // Tambah properti nik jika diperlukan
		public bool status; // Tambah properti status jika diperlukan
	}
}
