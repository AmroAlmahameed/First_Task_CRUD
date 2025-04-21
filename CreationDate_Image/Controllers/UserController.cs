using CreationDate_Image.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CreationDate_Image.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<User> users = new List<User>();
            try
            {

           
            string connectionString = _configuration.GetConnectionString("MyAppDB");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_ManageUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "SELECT");
                    command.Parameters.AddWithValue("@Id", DBNull.Value);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var user = new User
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserName = reader["UserName"].ToString(),
                                CreationDate = Convert.ToDateTime(reader["CreationDate"]),
                                Image = reader["Image"] != DBNull.Value
                                ? Convert.ToBase64String((byte[])reader["Image"])
                                : null
                            };
                            users.Add(user);
                        }

                    }
                }
            }

            }
            catch (Exception)
            {

                throw;
            }
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                byte[] imageBytes = null;
                //string base64Image = null;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.ImageFile.CopyToAsync(ms);
                        imageBytes = ms.ToArray();
                        //base64Image = Convert.ToBase64String(imageBytes);
                    }
                }

                string connectionString = _configuration.GetConnectionString("MyAppDB");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_ManageUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Action", "INSERT");
                        command.Parameters.AddWithValue("@UserName", model.UserName);

                        if (imageBytes == null)
                        {
                            command.Parameters.AddWithValue("@Image", DBNull.Value);
                        }
                        else
                        {
                            // Otherwise, pass the byte array (imageBytes) to the SQL parameter
                            command.Parameters.AddWithValue("@Image", imageBytes);
                        }
                        //if (string.IsNullOrEmpty(base64Image))
                        //    command.Parameters.AddWithValue("@Image", DBNull.Value);
                        //else
                        //    command.Parameters.AddWithValue("@Image", base64Image);

                        connection.Open();
                        var newId = command.ExecuteScalar();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = new User();
            string connectionString = _configuration.GetConnectionString("MyAppDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_ManageUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "SELECT");
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Id = Convert.ToInt32(reader["Id"]);
                            user.UserName = reader["UserName"].ToString();
                            user.CreationDate = Convert.ToDateTime(reader["CreationDate"]);
                            user.Image = reader["Image"] != DBNull.Value
                                ? Convert.ToBase64String((byte[])reader["Image"])
                                : null;
                        }
                    }
                }
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                byte[] imageBytes = null;
                //string base64Image = null;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.ImageFile.CopyToAsync(ms);
                       imageBytes = ms.ToArray();
                        //base64Image = Convert.ToBase64String(imageBytes);
                    }
                }
                string connectionString = _configuration.GetConnectionString("MyAppDB");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_ManageUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Action", "UPDATE");
                        command.Parameters.AddWithValue("@Id", model.Id);
                        command.Parameters.AddWithValue("@UserName", model.UserName);
                        if (imageBytes == null || imageBytes.Length == 0)
                            command.Parameters.AddWithValue("@Image", DBNull.Value);
                        else
                            command.Parameters.AddWithValue("@Image", imageBytes);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("MyAppDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_ManageUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "DELETE");
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    
                }
            }
            return RedirectToAction("Index");

        }

    }
}