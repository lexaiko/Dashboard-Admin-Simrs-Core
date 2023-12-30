using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace thelast.Pages.Pelayanan.Kasir
{
    public class KasirModel : PageModel
    {
        public List<PasienInfo> listPasien = new List<PasienInfo>();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnPost(string id)
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";

                if (!string.IsNullOrEmpty(id))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Memperbarui status_bayar menjadi true pada tabel TT_PASIENIGD
                        string updateTglPulangQuery = "UPDATE TT_PASIENIGD SET status_bayar = 1 WHERE id = @id";

                        using (SqlCommand tglPulangCommand = new SqlCommand(updateTglPulangQuery, connection))
                        {
                            tglPulangCommand.Parameters.AddWithValue("@id", id);
                            tglPulangCommand.ExecuteNonQuery();
                        }
                    }

                    successMessage = "Status dan tgl_pulang berhasil diperbarui";
                    // Redirect ke halaman Antrian setelah berhasil diperbarui
                    Response.Redirect("/Pelayanan/Antrian");
                }
                else
                {
                    // Tangani kasus ketika id_pasien kosong atau null
                    errorMessage = "id kosong atau null";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        // Sisanya adalah kode OnGet dan fungsi bantuan lainnya...

        // (Kode GetNamaPasien dan GetNamaDokter tetap sama)

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM TT_PASIENIGD";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PasienInfo pasienInfo = new PasienInfo();
                                pasienInfo.id = reader.IsDBNull(0) ? string.Empty : reader.GetInt32(0).ToString();
                                pasienInfo.id_pasien = reader.IsDBNull(1) ? string.Empty : reader.GetInt32(1).ToString();
                                pasienInfo.tgl_datang = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2);
                                pasienInfo.id_dokter = reader.IsDBNull(4) ? string.Empty : reader.GetInt32(4).ToString();
                                pasienInfo.biaya = reader.IsDBNull(5) ? string.Empty : reader.GetInt32(5).ToString();
                                pasienInfo.status_bayar = !reader.IsDBNull(6) && reader.GetBoolean(6);

                                // Menggunakan koneksi baru untuk memanggil GetNamaPasien
                                string namaPasien = GetNamaPasien(connectionString, pasienInfo.id_pasien);
                                pasienInfo.nama_pasien = namaPasien;
                                string namaDokter = GetNamaDokter(connectionString, pasienInfo.id_dokter);
                                pasienInfo.nama_dokter = namaDokter;

                                listPasien.Add(pasienInfo);
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

        // Fungsi untuk mendapatkan nama pasien berdasarkan id_pasien dari tabel TM_PASIEN
        private string GetNamaPasien(string connectionString, string id_pasien)
        {
            string namaPasien = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT nama FROM TM_PASIEN WHERE id = @id_pasien";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_pasien", id_pasien);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            namaPasien = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                        }
                    }
                }
            }

            return namaPasien;
        }
        private string GetNamaDokter(string connectionString, string id_dokter)
        {
            string namaDokter = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT nama FROM TM_DOKTER WHERE id = @id_dokter";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_dokter", id_dokter);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            namaDokter = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                        }
                    }
                }
            }

            return namaDokter;
        }
    }

    public class PasienInfo
    {
        public string id;
        public string id_pasien;
        public DateTime tgl_datang;
        public string id_dokter;
        public string biaya;
        public bool status_bayar;
        public string nama_pasien; // Menambah properti untuk menyimpan nama pasien
        public string nama_dokter; // Menambah properti untuk menyimpan nama pasien
    }
}
