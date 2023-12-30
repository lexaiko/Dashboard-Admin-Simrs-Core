using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace thelast.Pages
{
    public class CreateModel : PageModel
    {
        public ClientInfo clientInfo = new ClientInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnPost()
        {
            try
            {
                string nama = Request.Form["nama"];
                string jenis_kelamin = Request.Form["jenis_kelamin"];
                string address = Request.Form["address"];
                string nik = Request.Form["nik"];
                string tgl_lahirString = Request.Form["tgl_lahir"];

                if (string.IsNullOrEmpty(nama) || string.IsNullOrEmpty(jenis_kelamin) || string.IsNullOrEmpty(address))
                {
                    errorMessage = "Semua kolom harus diisi";
                    return;
                }

                DateTime tgl_lahir;
                if (!DateTime.TryParse(tgl_lahirString, out tgl_lahir))
                {
                    errorMessage = $"Format tanggal lahir tidak valid. Input yang diterima: {Request.Form["tgl_lahir"]}";
                    return;
                }

                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=simrs;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the NIK already exists in the database
                    string checkIfExistsQuery = "SELECT COUNT(*) FROM TM_PASIEN WHERE nik = @nik";
                    using (SqlCommand checkIfExistsCommand = new SqlCommand(checkIfExistsQuery, connection))
                    {
                        checkIfExistsCommand.Parameters.AddWithValue("@nik", nik);
                        int existingCount = (int)checkIfExistsCommand.ExecuteScalar();

                        if (existingCount > 0)
                        {
                            errorMessage = "Nomor Identitas (NIK) sudah ada dalam database. Data tidak dapat ditambahkan.";
                            return; // Stop further execution
                        }
                    }

                    // Perform insertion if NIK doesn't exist
                    string sql = "INSERT INTO TM_PASIEN " +
                                 "(nama, tgl_lahir, jenis_kelamin, address, nik, status) VALUES " +
                                 "(@nama, @tgl_lahir, @jenis_kelamin, @address, @nik, @status)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@nama", nama);
                        command.Parameters.AddWithValue("@tgl_lahir", tgl_lahir);
                        command.Parameters.AddWithValue("@jenis_kelamin", jenis_kelamin);
                        command.Parameters.AddWithValue("@address", address);
                        command.Parameters.AddWithValue("@nik", nik);
                        command.Parameters.AddWithValue("@status", 0); // Default status is set to 0

                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "Pasien baru berhasil ditambahkan";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
