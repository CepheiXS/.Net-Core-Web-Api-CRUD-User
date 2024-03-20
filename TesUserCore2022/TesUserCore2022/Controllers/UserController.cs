using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TesUserCore2022.Models;

namespace TesUserCore2022.Controllers
{
    [Route("api/userAgent")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get(JObject jSonreq)
        {
            string query = @"
                select userid as ""userid"",
                       namalengkap ""namalengkap"",
                       username ""username"",
                       password ""password"",
                       status ""status""
                from tbl_user Order By userid ASC
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("tbl_userAppCon");
            string userId = jSonreq["userid"].ToString();
            NpgsqlDataReader myReader;
            if (userId.Equals("all"))
            {
                using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();

                    }
                }
                return new JsonResult(table);
            }
            else
            {
                return new JsonResult("Request anda tidak sesuai");
            }

        }

        [HttpPost]
        public JsonResult Post(User dep)
        {
            string query = @"
                insert into tbl_user (namalengkap, username, password, status)
                values 	(@namalengkap, @username, @password, @status);
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("tbl_userAppCon");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@namalengkap", dep.namalengkap);
                    myCommand.Parameters.AddWithValue("@username", dep.username);
                    myCommand.Parameters.AddWithValue("@password", dep.password);
                    myCommand.Parameters.AddWithValue("@status", dep.status);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();

                }
            }

            return new JsonResult("Add Berhasil");
        }

        [HttpPut]
        public JsonResult Put(JObject jSonreq)
        {
            string query = @"
                update tbl_user 
                set namalengkap = @namalengkap,
                    username = @username, 
                    password = @password, 
                    status = @status
                where userid = @userid
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("tbl_userAppCon");
            NpgsqlDataReader myReader;

            //try
            //{

                using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@userid", Int32.Parse(jSonreq["userid"].ToString()));
                        myCommand.Parameters.AddWithValue("@namalengkap", jSonreq["namalengkap"].ToString());
                        myCommand.Parameters.AddWithValue("@username", jSonreq["username"].ToString());
                        myCommand.Parameters.AddWithValue("@password", jSonreq["password"].ToString());
                        myCommand.Parameters.AddWithValue("@status", jSonreq["status"].ToString());

                        myReader = myCommand.ExecuteReader();
                        
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();

                    //int rowsAffected = myCommand.ExecuteNonQuery();
                    //if (rowsAffected > 0)
                    //{
                    //    return new JsonResult("Update Berhasil");
                    //}
                    //else
                    //{
                    //    return new JsonResult("Update gagal: User ID not found");
                    //}

                    }
                }
                return new JsonResult("Update Data Berhasil");
            //}
            //catch (System.Exception e) {
            //    return new JsonResult(e.Message);
            //}



        }

        [HttpDelete]
        public JsonResult Delete(JObject jSonreq)
        {
            string query = @"
                delete from tbl_user 
                where userid = @userid
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("tbl_userAppCon");
            int reqJsonId = Int32.Parse(jSonreq["id"].ToString());
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@userid", reqJsonId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();

                }
            }

            return new JsonResult("Delete Berhasil");
        }

    }
}
