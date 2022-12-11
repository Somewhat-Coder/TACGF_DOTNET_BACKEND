using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ProjectBackend.Models;
using System.Web.UI.WebControls;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using System.Collections;
using ProjectBackend.ServiceReference1;


namespace ProjectBackend.Controllers
{
     [RoutePrefix("api")]

     public class AppController : ApiController
     {
          SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
          SqlCommand cmd = new SqlCommand();
          SqlDataAdapter da = new SqlDataAdapter();
          SqlDataReader dataReader;

          static string role = " ";
          static admins AData;
          static teacher TData;

          /// Web Service 
          WebService1SoapClient obj;

          [HttpPost]
          [Route("RegisterStudent")]
          public string RegisterStudent(registerStudent regisdata)
          {
               string response = String.Empty;
               string subjects = String.Empty;


               foreach(string sub in regisdata.courses)
               {
                    subjects = sub +","+ subjects;
               }
               try
               {
                    string commandText = "insert into registrations values(@name,@number,@gender,@mode,@courses,@hrs,@fees);";
                    cmd = new SqlCommand(commandText, conn);
                    cmd.Parameters.Add("@name", regisdata.fullName);
                    cmd.Parameters.Add("@number", regisdata.phoneNumber);
                    cmd.Parameters.Add("@gender", regisdata.gender);
                    cmd.Parameters.Add("@mode", regisdata.mode);
                    cmd.Parameters.Add("@courses", subjects);
                    cmd.Parameters.Add("@hrs", regisdata.hours);
                    cmd.Parameters.Add("@fees", regisdata.expFees);

                    conn.Open();
                    int r = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (r > 0)
                    {
                         response = "Success";
                    }
                    else
                    {
                         response = "Faield";
                    }
               }

               catch(Exception e)
               {
                    response = "Failed";
               }

               return response;
          }

          [HttpPost]
          [Route("SignUp")]
          public string SignUp(teacher teacherData)
          {
               
               string resp = String.Empty;

               try
               {
                    string commandText = "select firstName from teachers where email = @email;";
                    da = new SqlDataAdapter(commandText, conn);
                    da.SelectCommand.Parameters.Add("@email", teacherData.email);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    commandText = "select firstName from admins where email = @email;";
                    da = new SqlDataAdapter(commandText, conn);
                    da.SelectCommand.Parameters.Add("@email", teacherData.email);

                    DataTable dt2 = new DataTable();
                    da.Fill(dt2);

                    if (dt.Rows.Count == 0 && dt2.Rows.Count == 0)
                    {
                         try
                         {
                              commandText = "insert into teachers values(@fname,@lname,@email,@phone,@subject,@password);";
                              cmd = new SqlCommand(commandText, conn);
                              cmd.Parameters.Add("@fname", teacherData.firstName);
                              cmd.Parameters.Add("@lname", teacherData.lastName);
                              cmd.Parameters.Add("@phone", teacherData.phoneNumber);
                              cmd.Parameters.Add("@email", teacherData.email);
                              cmd.Parameters.Add("@password", teacherData.password);
                              cmd.Parameters.Add("@subject", teacherData.subjects);
   
                              conn.Open();
                              int r = cmd.ExecuteNonQuery();
                              conn.Close();

                              if (r > 0)
                              {
                                   resp = "Success";
                              }
                              else
                              {
                                   resp = "Failed";
                              }
                         }

                         catch(Exception e)
                         {
                              resp = "Failed";
                         }


                    }
                    else
                    {
                         resp = "Failed";
                    }


               }

               catch (Exception e)
               {
                    resp = "Failed";
              
               }
         

               return resp;
          }

          [HttpPost]
          [Route("Login")]
          public string Login(teacher teacherData)
          {

               string resp = String.Empty;

               try
               {
                    string commandText = "select * from admins where email = @email and password = @password;";
                    cmd = new SqlCommand(commandText, conn);
                    cmd.Parameters.Add("@email", teacherData.email);
                    cmd.Parameters.Add("@password", teacherData.password);

                    conn.Open();

                    dataReader = cmd.ExecuteReader();

                    AData = new admins();
                    TData = new teacher();

                    while (dataReader.Read())
                    {
                         role = "admin";
                         

                         AData.id = dataReader.GetValue(0).ToString();
                         AData.firstName = dataReader.GetValue(1).ToString();
                         AData.lastName = dataReader.GetValue(2).ToString();
                         AData.email = dataReader.GetValue(3).ToString();
                         AData.password = dataReader.GetValue(4).ToString();
                         AData.phoneNumber = dataReader.GetValue(5).ToString();

                         return "Success";
                    }

                    conn.Close();


                    commandText = "select * from teachers where email = @email and password = @password;";
                    cmd = new SqlCommand(commandText, conn);
                    cmd.Parameters.Add("@email", teacherData.email);
                    cmd.Parameters.Add("@password", teacherData.password);

                    conn.Open();
                    dataReader = cmd.ExecuteReader();
           

                    while (dataReader.Read())
                    {

                         role = "teacher";
                         

                         TData.id = dataReader.GetValue(0).ToString();
                         TData.firstName = dataReader.GetValue(1).ToString();
                         TData.lastName = dataReader.GetValue(2).ToString();
                         TData.email = dataReader.GetValue(3).ToString();
                         TData.phoneNumber = dataReader.GetValue(4).ToString();
                         TData.subjects = dataReader.GetValue(5).ToString();
                         TData.password = dataReader.GetValue(6).ToString();
                         

                         return "Success";
                       
                    }

                    conn.Close();


                    resp = "Failed";
                    
               }

               catch (Exception e)
               {
                    resp = "Failed";

               }


               return resp;
          }

          [HttpPost]
          [Route("Logout")]
          public string Logout(Response respo)
          {

               string resp = String.Empty;
               
               if (role == "admin")
               {
                    role = " ";
                    resp = "Success";
               }
               else if (role == "teacher")
               {
                    role = " ";
                    resp = "Success";
               }

               else
               {
                    resp = "Failed";
               }


               return resp;
          }

          [HttpPost]
          [Route("UpdatePersonalInfo")]
          public string UpdatePersonalInfo(teacher teacherData)
          {
               string commandText = String.Empty;
               string resp = String.Empty;

               try
               {
                    if (role == "admin")
                    {
                         commandText = "UPDATE admins SET firstName = @fname, lastName = @lname, phoneNumber = @num WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", AData.id);
                    }

                    else if (role == "teacher")
                    {
                         commandText = "UPDATE teachers SET firstName = @fname, lastName = @lname, phoneNumber = @num, subjects = @subj WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", TData.id);
                         cmd.Parameters.Add("@subj", teacherData.subjects);
                    }
                    else
                    {
                         return "Failed";
                    }

                    cmd.Parameters.Add("@fname", teacherData.firstName);
                    cmd.Parameters.Add("@lname", teacherData.lastName);
                    cmd.Parameters.Add("@num", teacherData.phoneNumber);

                    WebService1SoapClient obj = new WebService1SoapClient();
                string asd = obj.HelloWorld();


                conn.Open();
                    int r = cmd.ExecuteNonQuery();
                    conn.Close();


                    if (r > 0)
                    {
                         if (role == "admin")
                         {
                              AData.firstName = teacherData.firstName;
                              AData.lastName = teacherData.lastName;
                              AData.phoneNumber = teacherData.phoneNumber;
                         }
                         else
                         {
                              TData.firstName = teacherData.firstName;
                              TData.lastName = teacherData.lastName;
                              TData.phoneNumber = teacherData.phoneNumber;
                              TData.subjects = teacherData.subjects;
                         }

                         resp = "Success";
                    }

                    else
                    {
                         resp = "Failed";
                    }

               }

               catch (Exception e)
               {
                    resp = "Failed";

               }


               return resp;
          }

          [HttpPost]
          [Route("UpdateAccountInfo")]
          public string UpdateAccountInfo(admins adminData)
          {

               string resp = String.Empty;
               string commandText = String.Empty;
               int r = 0;

               try
               {
                    commandText = "select firstName from teachers where email = @email;";
                    da = new SqlDataAdapter(commandText, conn);
                    da.SelectCommand.Parameters.Add("@email", adminData.email);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    commandText = "select firstName from admins where email = @email;";
                    da = new SqlDataAdapter(commandText, conn);
                    da.SelectCommand.Parameters.Add("@email", adminData.email);

                    DataTable dt2 = new DataTable();
                    da.Fill(dt2);

                    if (dt.Rows.Count != 0 || dt2.Rows.Count != 0)
                    {
                         return "Taken";

                    }
               }

               catch (Exception e)
               {
                    return "Failed";
               }




               try
               {
                    
                    if (role == "admin")
                    {
                         commandText = "UPDATE admins SET email = @email, password = @password WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", AData.id);

                    }

                    else if (role == "teacher")
                    {
                         commandText = "UPDATE teachers SET email = @email, password = @password WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", TData.id);

                    }
                    else
                    {
                         return "Failed";
                    }

                    cmd.Parameters.Add("@email", adminData.email);
                    cmd.Parameters.Add("@password", adminData.password);
                    

                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (r > 0)
                    {

                         if (role == "admin")
                         {
                              AData.email = adminData.email;
                              AData.password = adminData.password;

                         }

                        else
                         {
                              TData.email = adminData.email;
                              TData.password = adminData.password;

                         }

                         resp = "Success";
                    }

                    else
                    {
                         resp = "Failed";
                    }

               }

               catch (Exception e)
               {
                    resp = "Failed";

               }


               return resp;
          }


          [HttpPost]
          [Route("UpdatePassword")]
          public string UpdatePassword(admins adminData)
          {

               string resp = String.Empty;
               string commandText = String.Empty;
               int r = 0;


               try
               {

                    if (role == "admin")
                    {
                         commandText = "UPDATE admins SET password = @password WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", AData.id);

                    }

                    else if (role == "teacher")
                    {
                         commandText = "UPDATE teachers SET password = @password WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", TData.id);

                    }
                    else
                    {
                         return "Failed";
                    }

                    cmd.Parameters.Add("@password", adminData.password);


                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (r > 0)
                    {

                         if (role == "admin")
                         {
                              AData.password = adminData.password;

                         }

                         else
                         {
                              TData.password = adminData.password;

                         }

                         resp = "Success";
                    }

                    else
                    {
                         resp = "Failed";
                    }

               }

               catch (Exception e)
               {
                    resp = "Failed";

               }


               return resp;
          }

          [HttpPost]
          [Route("RemoveTeacher")]
          public string RemoveTeacher(Response resp)
          {
               string res = string.Empty;

               try
               {

                    for (int i = 0; i < resp.ids.Count; i++)
                    {
                         string commandText = "delete from teachers where id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", resp.ids[i].ToString());

                         conn.Open();
                         int r = cmd.ExecuteNonQuery();
                         conn.Close();

                         if (r > 0)
                         {
                              res = "Success";
                         }

                         else
                         {
                              return "Failed";
                         }


                    }
               }
               catch(Exception e)
               {
                    res = "Failed";
               }

               return res;

          }

          [HttpPost]
          [Route("RemoveRegistration")]
          public string RemoveRegistration(Response resp)
          {
               string res = string.Empty;

               try
               {

                    for (int i = 0; i < resp.ids.Count; i++)
                    {
                         string commandText = "delete from registrations where id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", resp.ids[i].ToString());

                         conn.Open();
                         int r = cmd.ExecuteNonQuery();
                         conn.Close();

                         if (r > 0)
                         {
                              res = "Success";
                         }

                         else
                         {
                              return "Failed at updating";
                         }


                    }
               }
               catch (Exception e)
               {
                    res = "Failed";
               }

               return res;

          }








          [HttpGet]
          [Route("GetRole")]
          public string GetRole()
          {
               return role;
          }

          [HttpGet]
          [Route("GetProfileInfo")]
          public Response GetProfileInfo()
          {
               Response res = new Response();

               res.role = role;

               if (role == "admin")
               {
                    res.firstName = AData.firstName;
                    res.phoneNumber = AData.phoneNumber; 
                    res.email = AData.email;
               }
               else if (role == "teacher")
               {
                    res.firstName = TData.firstName;
                    res.phoneNumber = TData.phoneNumber;
                    res.email = TData.email;
               }
               else
               {
                    res.firstName = " ";
               }
               
               return res;
          }

          [HttpGet]
          [Route("GetTeachers")]
          public ArrayList GetTeachers()
          {
               ArrayList regis = new ArrayList();
               ArrayList rw;

               try
               {

                    string commandText = "select id,firstName,lastName,phoneNumber,subjects from teachers;";
                    da = new SqlDataAdapter(commandText, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {    
                         rw = new ArrayList();
                         string id = row["id"].ToString();
                         string firstName = row["firstName"].ToString();
                         string lastName = row["lastName"].ToString();
                         string phoneNumber = row["phoneNumber"].ToString();
                         string subjects = row["subjects"].ToString();

                         rw.Add(id);
                         rw.Add(firstName);
                         rw.Add(lastName);
                         rw.Add(phoneNumber);
                         rw.Add(subjects);

                         regis.Add(rw);
                    }
               }

               catch(Exception e)
               {
                    Console.WriteLine(e);
               }

               return regis;
          }

          [HttpGet]
          [Route("GetRegistrations")]
          public ArrayList GetRegistrations()
          {
               ArrayList regis = new ArrayList();
               ArrayList rw;

               obj = new WebService1SoapClient();

               try
               {

                    string commandText = "select * from registrations;";
                    da = new SqlDataAdapter(commandText, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                         rw = new ArrayList();
                         string id = row["id"].ToString();
                         string fullName = row["fullName"].ToString();
                         string phoneNumber = row["phoneNumber"].ToString();
                         string gender = row["gender"].ToString();
                         string mode = row["mode"].ToString();
                         string courses = row["courses"].ToString();
                         string hours = row["hours"].ToString();
                         string expFees = row["expFees"].ToString();


                         // web service
                         //courses = obj.RemoveCharWebService(courses);

                         rw.Add(id);
                         rw.Add(fullName);
                         rw.Add(phoneNumber);
                         rw.Add(gender);
                         rw.Add(mode);
                         rw.Add(courses);
                         rw.Add(hours);
                         rw.Add(expFees);

                         regis.Add(rw);
                    }
               }

               catch (Exception e)
               {
                    Console.WriteLine(e);
               }

               return regis;
          }

          [HttpGet]
          [Route("GetRegistrationsTeacher")]
          public ArrayList GetRegistrationsTeacher()
          {
               ArrayList regis = new ArrayList();
               ArrayList rw;

               try
               {

                    string commandText = "select * from registrations;";
                    da = new SqlDataAdapter(commandText, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                         rw = new ArrayList();
                         string id = row["id"].ToString();
                         string fullName = row["fullName"].ToString();
                         string phoneNumber = row["phoneNumber"].ToString();
                         string gender = row["gender"].ToString();
                         string mode = row["mode"].ToString();
                         string courses = row["courses"].ToString();
                         string hours = row["hours"].ToString();
                         string expFees = row["expFees"].ToString();

                         List<string> Subjects = courses.Split(',').ToList();

                         if (Subjects.Contains(TData.subjects))
                         {
                              rw.Add(id);
                              rw.Add(fullName);
                              rw.Add(phoneNumber);
                              rw.Add(gender);
                              rw.Add(mode);
                              rw.Add(courses.Remove(courses.Length - 1, 1));
                              rw.Add(hours);
                              rw.Add(expFees);

                              regis.Add(rw);

                         }

                    }
               }

               catch (Exception e)
               {
                    Console.WriteLine(e);
               }

               return regis;
          }
     }
}
