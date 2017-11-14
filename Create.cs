using AppHelpers;
using Jetone.OrganizationalStructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jetone.OrganizationalStructure
{
    public static partial class Organization
    {

        /// <summary>
        /// 新建公司（默认创建一个管理部门，一个管理账号，一个菜单资源的资源组）
        /// </summary>
        /// <param name="data"></param>
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
                int companyId = t.SelectScalar<int>(Sql.CreateCompanySql, new { data.CompanyName, data.ParentCompanyId });//创建公司
                int departmentId = t.SelectScalar<int>(Sql.CreateDepartmentSql, new { data.DepartmentName, companyId });//创建部门
                int userId = t.SelectScalar<int>(Sql.CreateUserSql, new { data.UserName, data.Password, departmentId, companyId });//创建人员
                int resourceGroupId = t.SelectScalar<int>(Sql.CreateResourceGroup, new { data.SystemId, companyId, data.RoleName });//创建角色
                int insertGroupMember = t.SelectScalar<int>(Sql.BingGroup, new { RoleId = resourceGroupId, userId });//角色与人员绑定
                int resultMenu = t.Update(Sql.InsertMenuUrl, new { companyId }, data.Url);//创建菜单
                List<int> menuUrls = t.Select<int>(Sql.GetMenuUrlId, new { companyId }).ToList();
                int insertOperateAuth = t.SelectScalar<int>(Sql.InsertAuth, new { Name = "可读可写", Description = "建立公司自动生成的权限", systemId = data.SystemId, companyId });//创建操作权限
                int result = t.Update(Sql.CreateRoleDetail, new { RoleId = resourceGroupId, AuthId = insertOperateAuth }, menuUrls.ConvertAll(d => new { Url = d }));//绑定角色菜单操作权限关系
                if (companyId == 0 || departmentId == 0 || userId == 0 || resourceGroupId == 0 || insertGroupMember == 0 || result == 0 || resultMenu == 0 || menuUrls.Count == 0 || insertOperateAuth == 0)
                {
                    return 0;
                }
                else
                {
                    t.Commit();
                    return companyId;
                };
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
            var companyId = Convert.ToInt32(tokenData["CompanyId"]);

            return CreateDepartment(data, companyId);
        }

        /// <summary>
        /// 新增部门，去除token
        /// </summary>
        /// <param name="data"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static int CreateDepartment(CreateDepartmentData data, int companyId)
        {
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
        /// 给资源权限添加人员
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public static bool BingRoleWithUsers(int roleId, List<int> userIds)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.BingRoleWithUser, new { roleId }, userIds.ConvertAll(d => new { userId = d })) > 0 ? true : false;
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
        /// <summary>
        /// 新增员工，去除Token
        /// </summary>
        /// <param name="data"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static int CreateUser(CreateUserData data, int companyId)
        {

            if (companyId == 0)
            {
                throw new ArgumentNullException("公司Id不能为空");
            }
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }


            using (var c = Sql.CreateConnection())
            using (var t = c.OpenTransaction())
            {

                //TODO CompanyId 换成从token获取
                int userId = t.SelectScalar<int>(Sql.CreateUserSql, new
                {
                    data.UserName,
                    data.Tel,
                    data.Password,
                    data.DepartmentId,
                    companyId
                });
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
                return c.Update(Sql.CreateRoleDetail, new { data.RoleId }, data.Url.ConvertAll(d => new { Url = d.Url })) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 增加系统角色权限资源，去除token
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CreateRoleDetail(CreateRoleDetailData data)
        {
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.CreateRoleDetail, new { data.RoleId }, data.Url.ConvertAll(d => new { Url = d.Url })) == 0 ? false : true;
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
            var tokenData = DisassembleProtocol(token);
            var companyId = Convert.ToInt32(tokenData["CompanyId"]);
            var systemId = Convert.ToInt32(tokenData["SystemId"]);
            return CreateRole(data, companyId, systemId);
        }

        /// <summary>
        /// 新增公司系统角色，去除token
        /// </summary>
        /// <param name="data"></param>
        /// <param name="companyId"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static int CreateRole(CreateRoleData data, int companyId, int systemId)
        {
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {

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
        /// 绑定角色与菜单权限的关系
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool BingRoleWihtMenu(List<MenuAuthData> data, int roleId)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.InserRoleDetial, new { roleId }, data) == data.Count ? true : false;
            }
        }

        /// <summary>
        /// 系统增加权限
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CreateAuth(List<CreateAuthData> data)
        {

            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.InsertAuth, data) == data.Count ? false : true;

            }
        }


        public class MenuAuthData
        {
            public int MenuId { get; set; }
            public int AuthId { get; set; }
        }
        public class CreateAuthData
        {
            public int SystemId { get; set; }
            public string Name { get; set; }
        }
    }
}
