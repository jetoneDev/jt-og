using AppHelpers;
using Jetone.OrganizationalStructure.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Jetone.OrganizationalStructure
{
    internal static class Operator
    {
        public static int Create(string tableName, object data)
        {
            using (SqlConnection connection = Sql.CreateConnection())
            {
                StringBuilder tableArgum = new StringBuilder();
                StringBuilder valueArgum = new StringBuilder();
                var properties = data.GetType().GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i != (properties.Length - 1))
                    {

                        tableArgum.Append(properties[i].Name + ",");
                        valueArgum.Append(properties[i].GetValue(data) + ",");
                    }
                    else
                    {
                        tableArgum.Append(properties[i].Name);
                        valueArgum.Append(properties[i].GetValue(data));
                    }
                }
                string query = "Insert Organization.." + tableName + " (" + tableArgum + ")" + " VALUES (" + "'" + valueArgum + "'" + ")";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
        public static int Delete(string tableName, object condition = null)
        {
            using (SqlConnection connection = Sql.CreateConnection())
            {
                StringBuilder conditionString = new StringBuilder(" where ");
                var properties = condition.GetType().GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (i == (properties.Length - 1))

                        conditionString.Append(properties[i].Name + "=" + properties[i].GetValue(condition));

                    else
                        conditionString.Append(properties[i].Name + "=" + properties[i].GetValue(condition) + " and ");
                }
                string query = "Delete Organization.." + tableName + conditionString;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int Update(string tableName, object data, object condition)
        {
            using (SqlConnection connection = Sql.CreateConnection())
            {
                StringBuilder dataString = new StringBuilder(" set ");
                var dataProp = data.GetType().GetProperties();
                for (int i = 0; i < dataProp.Length; i++)
                {
                    if (i == (dataProp.Length - 1))

                        dataString.Append(dataProp[i].Name + "=" + "'" + dataProp[i].GetValue(data) + "'");

                    else
                        dataString.Append(dataProp[i].Name + "=" + "'" + dataProp[i].GetValue(data) + "'" + " and ");
                }
                string query = "Update Organization.." + tableName + dataString;
                if (condition != null)
                {
                    var conditionProp = condition.GetType().GetProperties();
                    StringBuilder conditionString = new StringBuilder(" where ");
                    for (int i = 0; i < conditionProp.Length; i++)
                    {
                        if (i == (conditionProp.Length - 1))

                            conditionString.Append(conditionProp[i].Name + "=" + "'" + conditionProp[i].GetValue(condition) + "'");

                        else
                            conditionString.Append(conditionProp[i].Name + "=" + "'" + conditionProp[i].GetValue(condition) + "'" + " and ");
                    }
                    query += conditionString;
                }
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static List<string> Retrieve(string tableName, object condition = null, string[] data = null)
        {
            //TODO 调用方式优化

            StringBuilder tableArgum = new StringBuilder("Select ");
            StringBuilder conditionArgum = new StringBuilder(" Where ");
            //var properties = data.GetType().GetProperties();

            for (int i = 0; i < data.Length; i++)
            {
                if (i != (data.Length - 1))
                {
                    tableArgum.Append(data[i] + ",");
                }
                else
                {
                    tableArgum.Append(data[i]);
                }

            }
            string query = tableArgum + " FROM " + tableName;

            if (condition != null)
            {
                var conditonProp = condition.GetType().GetProperties();
                for (int i = 0; i < conditonProp.Length; i++)
                {
                    if (i != (conditonProp.Length - 1))
                    {
                        conditionArgum.Append(conditonProp[i].Name + ",");
                    }
                    else
                    {
                        conditionArgum.Append(conditonProp[i].Name + "=" + conditonProp[i].GetValue(condition));
                    }
                }
                query += conditionArgum;
            }

            using (SqlConnection connection = Sql.CreateConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    List<string> result = new List<string>();
                    while (reader.Read())
                    {
                        StringBuilder s = new StringBuilder();
                        for (int i = 0; i < data.Length; i++)
                        {
                            string jointString = "\"" + data[i] + "\"" + ":" + "\"" + reader[i] + "\"";
                            if (i != (data.Length - 1))
                            {

                                s.Append(jointString + ",");
                            }
                            else
                            {
                                s.Append(jointString);
                            }
                        }
                        result.Add("{" + s + "}");
                    }

                    return result;
                }
                catch (System.Exception e)
                {

                    throw new System.Exception(e.Message);
                }
            }
        }
        public static object Login(string userName, string password, int systemId)
        {
            using (SqlConnection connection = Sql.CreateConnection())
            {

                string loginQuery = "SELECT 'ok'  FROM Organization..[User]  where UserName=@userName and Password=@password and Status=0";
                string systemTokenConfig = "";

                SqlCommand command = new SqlCommand(loginQuery, connection);
                SqlCommand configCommand = new SqlCommand(systemTokenConfig, connection);

                connection.Open();
                var value = command.ExecuteReader();
                if (value == null)
                {
                    return new { Status = false, Message = "登陆失败", Result = default(object) };
                }
                return null;
            }
        }
    }

    public static partial class Organization
    {
        /// <summary>
        /// 登陆系统
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static LoginData Login(string userName, string password, int systemId)
        {

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("用户名不能为空");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("密码不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                var result = c.Select<LoginData>(Sql.LoginSql, new { userName, password, systemId });

                if (result == null || result.Count > 1 || result.Count == 0)
                {
                    throw new Exception("账户或密码错误");
                }
                var r = result.GetFirst();
                var roleBasicInfo = c.Select<RoleData>(Sql.GetUserRoleData, new { r.UserId });
                var roleDetail = c.Select<RoleDetail>(Sql.GetRoleDetail, new { RoleId = roleBasicInfo.ConvertAll(d => d.RoleId) });
                foreach (var rbi in roleBasicInfo)
                {
                    foreach (var rd in roleDetail)
                    {
                        if (rd.RoleId == rbi.RoleId)
                        {
                            rbi.UrlDetail.Add(rd.Url);
                        }
                    }
                }
                r.Data = roleBasicInfo;
                //获取所有RoleId字符串
                StringBuilder role = new StringBuilder();

                for (int i = 0; i < roleBasicInfo.Count; i++)
                {
                    if (i == (roleBasicInfo.Count - 1))
                    {
                        role.Append(roleBasicInfo[i].RoleName);

                    }
                    else
                    {
                        role.Append(roleBasicInfo[i].RoleName + ",");
                    }
                }
                r.Token = (r.CompanyId + "," + r.UserId + "," + systemId + "," + role + "," + DateTime.Now.ToString("yyyyMMddHHmmss")).Encode();
                return r;
            }
        }

        /// <summary>
        /// 获取公司基本信息(公司名称，公司部门)
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static CompanyInfo GetCompanyInfo(string token)
        {
            //TODO 权限标签化
            var tokenData = DisassembleProtocol(token);
            var companyId = Convert.ToInt32(tokenData["CompanyId"]);
            return GetCompanyInfo(companyId);
        }

        /// <summary>
        /// 获取公司基本信息（公司名称， 公司部门），去除token验证
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static CompanyInfo GetCompanyInfo(int companyId)
        {
            if (companyId == 0)
            {
                throw new ArgumentException("公司ID不能为空");
            }

            using (var c = Sql.CreateConnection())
            {
                var companyInfo = c.Select<CompanyInfo>(Sql.GetCompanyStructure, new { companyId });
                var departmentInfo = c.Select<DepartmentData>(Sql.GetCompanyDepartment, new { companyId });
                if (companyInfo.Count == 0)
                {
                    throw new ArgumentException("公司ID错误");
                }
                var result = companyInfo.GetFirst();
                result.DepartmentInfo = departmentInfo;
                return result;
            }
        }

        /// <summary>
        /// 获取部门的所有人员
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public static IList<UserData> GetDepartmentMember(int departmentId, string token)
        {
            if (departmentId == 0)
            {
                throw new ArgumentException("用户名不能为空");
            }

            using (var c = Sql.CreateConnection())
            {
                return c.Select<UserData>(Sql.GetDepartmentMember, new { departmentId });
            }
        }

        /// <summary>
        /// 获取部门所有人员，去除token
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public static IList<UserData> GetDepartmentMember(int departmentId)
        {
            return GetDepartmentMember(departmentId, null);

        }

        /// <summary>
        /// 获取公司绑定的平台
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static object GetCompanySystem(string token)
        {

            //TODO 权限标签化
            var tokenData = DisassembleProtocol(token);
            var companyId = Convert.ToInt32(tokenData["CompanyId"]);
            if (companyId == 0)
            {
                throw new ArgumentException("用户名不能为空");
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }

            return GetCompanySystem(companyId);
        }

        /// <summary>
        /// 获取公司绑定的平台
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static object GetCompanySystem(int companyId)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Select<object>(Sql.GetCompanySystem, new { companyId = companyId });
            }
        }

        /// <summary>
        /// 获取某个平台某个公司所有角色
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IList<RoleDataInfo> GetCompanyRole(string token)
        {
            //TODO 权限标签化
            var tokenData = DisassembleProtocol(token);
            var companyId = tokenData["CompanyId"];
            var systemId = tokenData["SystemId"];
            if (companyId == null)
            {
                throw new ArgumentException("用户名不能为空");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                return c.Select<RoleDataInfo>(Sql.GetCompanyRole, new { companyId, systemId });
            }
        }

        /// <summary>
        /// 获取某某公司平台所有角色
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IList<RoleDataInfo> GetCompanyRole(string token, string name)
        {
            //TODO 权限标签化
            var tokenData = DisassembleProtocol(token);
            var companyId = tokenData["CompanyId"];
            var systemId = tokenData["SystemId"];
            if (companyId == null)
            {
                throw new ArgumentException("用户名不能为空");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                return c.Select<RoleDataInfo>(Sql.GetCompanyRoleWithName, new { companyId, systemId, name });
            }
        }

        /// <summary>
        /// 获取平台所有角色，去除token
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static IList<RoleDataInfo> GetCompanyRole(int systemId)
        {
            if (systemId == 0)
            {
                throw new ArgumentException("系统Id不能为空");
            }

            using (var c = Sql.CreateConnection())
            {
                return c.Select<RoleDataInfo>(Sql.GetCompanyRole, new { systemId });
            }
        }

        /// <summary>
        /// 获取某个公司的所有部门信息或某个特定部门的信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="departmentName"></param>
        /// <returns></returns>
        public static IList<DepartmentData> GetDepartment(string token, string departmentName = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            var tokenData = DisassembleProtocol(token);
            var companyId = Convert.ToInt32(tokenData["CompanyId"]);

            return GetDepartment(companyId, departmentName);
        }

        /// <summary>
        /// 获取某个公司所有部门信息或某个特定部门的信息，去除token
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="departmentName"></param>
        /// <returns></returns>
        public static IList<DepartmentData> GetDepartment(int companyId, string departmentName = null)
        {
            if (companyId == 0)
            {
                throw new ArgumentException("用户名不能为空");
            }

            using (var c = Sql.CreateConnection())
            {
                return c.Select<DepartmentData>(departmentName == null ? Sql.GetAllOrOneDepartment : Sql.GetAllOrOneDepartment.Replace("/*condition*/", " and d.Name=@DepartmentName"), new { companyId, departmentName });
            }
        }

        /// <summary>
        /// 获取现有平台信息
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns></returns>
        public static IList<SystemInfo> GetSystemInfo(string systemName = null)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Select<SystemInfo>(Sql.GetSystemInfo, new { systemName });
            }
        }

        /// <summary>
        /// 获取系统所有主菜单跟子菜单的资源
        /// </summary>
        /// <returns></returns>
        public static List<MenuData> GetMuneUrl(string role, int companyId, int systemId)
        {
            using (var c = Sql.CreateConnection())
            {
                var mainMenu = c.Select<MenuData>(Sql.GetRoleUrl, new { role, systemId, companyId }).ToList();

                foreach (var item in mainMenu)
                {
                    var r = item.Auth.Split('/');
                    item.PowerOption = r[0].Split(',').ConvertAll(d => new AuthData { Name = d, Value = true }).ToList();
                    if (r.Count() == 2)
                    {

                        item.PowerOption.AddRange(r[1].Split(',').ConvertAll(d => new AuthData { Name = d, Value = false }));
                    }
                }
                return mainMenu;
            }
        }

        /// <summary>
        /// 获取系统所有主菜单跟子菜单的资源通过角色ID
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="companyId"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static List<MenuData> GetMuneUrl(int roleId, int companyId, int systemId)
        {
            using (var c = Sql.CreateConnection())
            {
                var mainMenu = c.Select<MenuData>(Sql.GetRoleUrlbyId, new { roleId, systemId, companyId }).ToList();
                foreach (var item in mainMenu)
                {
                    var r = item.Auth.Split('/');
                    item.PowerOption = r[0].Split(',').ConvertAll(d => new AuthData { Name = d, Value = true }).ToList();
                    if (r.Count() == 2)
                    {

                        item.PowerOption.AddRange(r[1].Split(',').ConvertAll(d => new AuthData { Name = d, Value = false }));
                    }
                }
                return mainMenu;
            }
        }

        /// <summary>
        /// 获取公司的从属信息
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static List<CompanyData> GetChildCompanyInfo(int companyId)
        {

            using (var c = Sql.CreateConnection())
            {
                return c.Select<CompanyData>(Sql.GetCompanyInfo, new
                {
                    companyId
                }).ToList();
            }
        }

        /// <summary>
        /// 获取系统所有公司信息
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static List<CompanyData> GetSystemCompanyInfo(int systemId)
        { using (var c = Sql.CreateConnection()) { return c.Select<CompanyData>(Sql.GetSystemAllCompanyInfo, new { systemId }).ToList(); } }

        /// <summary>
        /// 获取角色的是所有成员，当roleId等于0时获取公司所有人员
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="systemId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static List<UserData> GetRoleMember(int companyId, int systemId, int roleId = 0)
        {
            using (var c = Sql.CreateConnection())
            {
                var childCompanys = GetChildCompanyInfo(companyId);
                var companyIds = childCompanys.ConvertAll(d => d.Id);
                companyIds.Add(companyId); if (roleId == 0)
                {//获取公司以及子公司所有的角色成员

                    return c.Select<UserData>(Sql.GetRoleMember, new { systemId, companyId = companyIds }).ToList();
                }
                return c.Select<UserData>(Sql.GetOneRoleMember, new { roleId, AllCompanyId = companyIds, CompanyId = companyId, systemId }).ToList();
            }
        }


        /// <summary>
        ///通过公司ID获取公司所有用户
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static IList<UserInfo> GetUserByCompanyId(int[] companyId)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Select<UserInfo>(Sql.GetCompanyUser, new { companyId = companyId });

            }
        }
        public class UserInfo
        {
            public int UserId { get; set; }
            public int CompanyId { get; set; }
            public string UserName { get; set; }
        }

        public class UserData
        {
            public int CompanyId { get; set; }
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string Company { get; set; }
            public string Job { get; set; }
        }

        public class CompanyData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ParentCompanyId { get; set; }
        }
        public class MenuData
        {
            public int ParentMenuId { get; set; }
            public int MenuId { get; set; }
            public int OrderNum { get; set; }
            public string Name { get; set; }
            public string Href { get; set; }
            public string Auth { get; set; }
            public List<AuthData> PowerOption { get; set; } = new List<AuthData>();
            public List<MenuData> Value { get; set; } = new List<MenuData>();
        }

        public class AuthData
        {
            public string Name { get; set; }
            public bool Value { get; set; }
        }

        /// <summary>
        /// 获取系统的所有操作权限
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static List<AuthData> GetOperateAuth(int systemId)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Select<AuthData>(Sql.GetAllOperateAuth, new { systemId }).ToList();
            }
        }

        /// <summary>
        /// 解析Token协议
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Dictionary<string, string> DisassembleProtocol(string token)
        {
            var t = token.Replace(' ', '+');
            var tp = t.Decode().Split(',');
            var dict = new Dictionary<string, string>();
            //result.CompanyId + "," + result.UserId + "," + systemId + "," + role + "," + DateTime.Now.ToString("yyyyMMddHHmmss")
            dict.Add("CompanyId", tp[0]);
            dict.Add("UserId", tp[1]);
            dict.Add("SystemId", tp[2]);
            dict.Add("Role", tp[3]);
            dict.Add("DateTime", tp[4]);
            return dict;
        }

    }

}
