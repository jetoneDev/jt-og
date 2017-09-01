using AppHelpers;
using Jetone.OrganizationalStructure.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

    public static class Organization
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
                var result = c.Select<LoginData>(Sql.LoginSql, new { userName, password }).GetFirst();
                if (result == null)
                {
                    throw new Exception("账户或密码错误");
                }
                var roleBasicInfo = c.Select<RoleData>(Sql.GetUserRoleData, new { result.UserId });
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
                result.Data = roleBasicInfo;
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
                result.Token = (result.CompanyId + "," + result.UserId + "," + systemId + "," + role + "," + DateTime.Now.ToString("yyyyMMddHHmmss")).Encode();
                return result;
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
            var companyId = tokenData["CompanyId"];
            if (companyId == null)
            {
                throw new ArgumentException("用户名不能为空");
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
        /// 获取公司绑定的平台
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static object GetCompanySystem(string token)
        {

            //TODO 权限标签化
            var tokenData = DisassembleProtocol(token);
            var companyId = tokenData["CompanyId"];
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
                return c.Select<object>(Sql.GetCompanySystem, new { companyId = companyId });
            }
        }

        /// <summary>
        /// 获取某个平台所有角色
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IList<RoleDataInfo> GetCompanyRole(string token, string roleName = null)
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
                return c.Select<RoleDataInfo>(Sql.GetCompanyRole, new { companyId, systemId, roleName });
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
            var companyId = tokenData["CompanyId"];
            if (companyId == null)
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
        /// 新建公司（默认创建一个管理部门，一个管理账号，一个菜单资源的资源组）
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static int CreateCompany(CreateCompanyData data)
        {

            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            using (var t = c.OpenTransaction())
            {
                //新增公司
                int companyId = t.SelectScalar<int>(Sql.CreateCompanySql, new { data.CompanyName });
                int departmentId = t.SelectScalar<int>(Sql.CreateDepartmentSql, new { data.DepartmentName, companyId });
                int userId = t.SelectScalar<int>(Sql.CreateUserSql, new { data.UserName, data.Tel, data.Password, departmentId, companyId });
                int resourceGroupId = t.SelectScalar<int>(Sql.CreateResourceGroup, new { data.SystemId, companyId, data.RoleName });
                int insertGroupMember = t.SelectScalar<int>(Sql.BingGroup, new { RoleId = resourceGroupId, userId });
                int result = t.Update(Sql.CreateRoleDetail, new { RoleId = resourceGroupId }, data.Url.ConvertAll(d => new { Url = d.Url, Type = d.Type }));
                if (companyId != 0 & departmentId != 0 & userId != 0 & resourceGroupId != 0 & insertGroupMember != 0 & result != 0)
                {
                    t.Commit();
                    return companyId;
                }
                return 0;
            }

        }

        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int CreatDepartment(CreateDepartmentData data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            var tokenData = DisassembleProtocol(token);
            var companyId = tokenData["CompanyId"];

            using (var c = Sql.CreateConnection())
            {

                return c.SelectScalar<int>(Sql.CreateDepartmentSql, new { data.DepartmentName, companyId });
            }
        }

        /// <summary>
        /// 给人员添加资源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIdList"></param>
        /// <returns></returns>
        public static bool BingUserRole(int userId, List<int> roleIdList)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.BingGroup, new { userId }, roleIdList.ConvertAll(d => new { roleId = d })) > 0 ? true : false;
            }
        }

        /// <summary>
        /// 新增员工 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int CreateUser(CreateUserData data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            var companyId = DisassembleProtocol(token)["CompanyId"];

            using (var c = Sql.CreateConnection())
            using (var t = c.OpenTransaction())
            {

                //TODO CompanyId 换成从token获取
                int userId = t.SelectScalar<int>(Sql.CreateUserSql, new { data.UserName, data.Tel, data.Password, data.DepartmentId, companyId });
                // int bingGroup = t.SelectScalar<int>(Sql.BingGroup, new { data.RoleId, userId });
                if (userId != 0)
                {
                    t.Commit();
                    return userId;
                }
                return 0;
            }
        }

        ///// <summary>
        ///// 新增账号
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //public object CreateUser(CreateUserData data, string token)
        //{
        //    if (string.IsNullOrWhiteSpace(token))
        //    {
        //        throw new ArgumentException("token不能为空");
        //    }
        //    if (data == null)
        //    {
        //        throw new ArgumentException("data 参数不能为空");
        //    }
        //    using (var c = Sql.CreateConnection())
        //    {
        //        int companyId = 0;
        //        //TODO companyId 的值从token中获取
        //        return c.Update(Sql.CreateUserSql, new { data.UserName, data.Tel, data.Password, data.DepartmentId, companyId }) == 0 ? false : true;
        //    }
        //}

        /// <summary>
        /// 增加系统角色权限资源
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool CreateRoleDetail(CreateRoleDetailData data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.CreateRoleDetail, new { data.RoleId }, data.Url.ConvertAll(d => new { Url = d.Url, Type = d.Type })) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 新增公司系统角色
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int CreateRole(CreateRoleData data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                var tokenData = DisassembleProtocol(token);
                var companyId = tokenData["CompanyId"];
                var systemId = tokenData["SystemId"];
                return c.SelectScalar<int>(Sql.CreateRoleData, new { companyId, systemId, data.RoleName });
            }
        }


        /// <summary>
        /// 给某个账号添加系统(待测)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool BingSystemToUser(BingSystemToUserData data)
        {

            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            using (var t = c.OpenTransaction())
            {
                var roleId = t.SelectScalar<int>(Sql.CreateRoleData, new { data.CompanyId, data.SystemId, data.RoleName });
                var bingUserId = t.Update(Sql.BingGroup, new { roleId, data.UserId });
                if (roleId != 0 & bingUserId > 0)
                {
                    t.Commit();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 修改公司基本信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool UpdateCompanyInfo(UpdateCompanyInfoData data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                var dict = DisassembleProtocol(token);

                return c.Update(Sql.UpdateCompanyInfo, new { data.CompanyName, companyId = dict["CompanyId"] }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 修改账号信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool UpdateUserInfo(UpdateUserInfoData data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                string userId = DisassembleProtocol(token)["UserId"];
                return c.Update(Sql.UpdateUserInfo, new { data.UserName, data.Tel, data.Password, userId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 修改部门信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static object UpdateDepartmentInfo(UpdateDepartmentData data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateDepartmentInfo, new { data.DepartmentName, data.DepartmentId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 修改组内人员
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static object UpdateDepartmentMember(UpdateDepartmentMember data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                var userId = DisassembleProtocol(token)["UserId"];
                return c.Update(Sql.ChangeDepartmentMember, new { data.DepartmentId, userId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 修改账号角色
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateUserRole(UpdateUserRoleData data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                var userId = DisassembleProtocol(token)["UserId"];
                return c.Update(Sql.UpdateUserRole, new { data.NewRoleId, data.OldRoleId, userId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 修改角色资源
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool UpdateRoleDetail(UpdateRoleDetailData data, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("token不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            using (var t = c.OpenTransaction())
            {
                var deleteRoleDetail = t.Update(Sql.DeleteRoleDetail, new { data.RoleId });
                var updateRoleDetail = t.Update(Sql.UpdateRoleDetail, new { data.RoleId }, data.Url.ConvertAll(d => new { Url = d.Url, Type = d.Type }));
                if (deleteRoleDetail > 0 & updateRoleDetail >= 1)
                {
                    t.Commit();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 重新开启某个公司某个系统的资源
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static bool UpdateCompanyResource(int companyId, int systemId)
        {
            if (companyId == 0)
            {
                throw new ArgumentException("companyId不能为空");
            }
            if (systemId == 0)
            {
                throw new ArgumentException("systemId不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.OpenCompany, new { companyId, systemId }) == 0 ? false : true;
            }

        }
        //TODO 当管理员停用资源组时，再次重新创建相同名称的资源组时候发生错误





        /// <summary>
        /// 停用某公司的某个平台
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static bool DeleteCompany(int companyId, int systemId)
        {

            if (companyId == 0)
            {
                throw new ArgumentException("companyId不能为空");
            }
            if (systemId == 0)
            {
                throw new ArgumentException("systemId不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateCompanySystem, new { companyId, systemId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="token"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public static object DeleteDepartment(string token, int departmentId)
        {
            if (string.IsNullOrWhiteSpace(token))
            {

                throw new ArgumentException("token不能为空");
            }
            if (departmentId == 0)
            {
                throw new ArgumentException("departmentId不能为空");
            }

            var tokenData = DisassembleProtocol(token);
            var companyId = tokenData["CompanyId"];
            if (companyId == null)
            {
                throw new ArgumentException("companyId不能为空");
            }

            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateCompanyDepartment, new { companyId, departmentId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 删除或禁用某账号
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static object DeleteUser(string token, int userId)
        {
            if (string.IsNullOrWhiteSpace(token))
            {

                throw new ArgumentException("token不能为空");
            }
            if (userId == 0)
            {
                throw new ArgumentException("userId不能为空");
            }
            var tokenData = DisassembleProtocol(token);
            var companyId = tokenData["CompanyId"];
            if (companyId == null)
            {
                throw new ArgumentException("companyId不能为空");
            }

            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateUserStatus, new { companyId, userId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 删除自定义角色
        /// </summary>
        /// <param name="token"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static object DeleteRole(string token, int roleId)
        {
            if (string.IsNullOrWhiteSpace(token))
            {

                throw new ArgumentException("token不能为空");
            }
            if (roleId == 0)
            {
                throw new ArgumentException("roleId不能为空");
            }

            var tokenData = DisassembleProtocol(token);
            var companyId = tokenData["CompanyId"];
            if (companyId == null)
            {
                throw new ArgumentException("companyId不能为空");
            }

            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateRoleStatus, new { companyId, roleId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 删除角色资源链接
        /// </summary>
        /// <param name="token"></param>
        /// <param name="roleDetailId"></param>
        /// <returns></returns>
        public static object DeleteRoleDetail(string token, int roleDetailId)
        {
            return "待完善";

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
