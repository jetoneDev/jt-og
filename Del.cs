using AppHelpers;
using System;
using System.Collections.Generic;

namespace Jetone.OrganizationalStructure
{
    public static partial class Organization
    {
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
        /// 删除部门（去除token）
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static object DeleteDepartment(int departmentId, int companyId)
        {

            if (departmentId == 0)
            {
                throw new ArgumentException("departmentId不能为空");
            }
            if (companyId == 0)
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
        /// 删除或禁用某账号（去除token）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static object DeleteUser(int userId, int companyId)
        {
            if (userId == 0)
            {
                throw new ArgumentException("userId不能为空");
            }

            if (companyId == 0)
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
        /// <param name="companyId"></param>
        /// <param name="roleId"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static bool DeleteRole(int companyId, int roleId, int systemId)
        {
            if (roleId == 0)
            {
                throw new ArgumentException("roleId不能为空");
            }
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
                return c.Update(Sql.UpdateRoleStatus, new { companyId, roleId, systemId }) == 0 ? false : true;
            }
        }
        /// <summary>
        /// 删除自定义角色
        /// </summary>
        /// <param name="token"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static bool DeleteRole(string token, int roleId, int systemId)
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
                return c.Update(Sql.UpdateRoleStatus, new { companyId, roleId, systemId }) == 0 ? false : true;
            }
        }
        /// <summary>
        /// 删除自定义角色（删除token）
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static object DeleteRole(int roleId, int companyId)
        {

            if (roleId == 0)
            {
                throw new ArgumentException("roleId不能为空");
            }
            if (companyId == 0)
            {
                throw new ArgumentException("companyId不能为空");
            }

            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateRoleStatus, new { companyId, roleId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 删除角色成员
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static bool DeleteRoleMember(int roleId, List<int> userId)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.DelRoleMember, new { userId, roleId }) == 0 ? false : true;
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
        public static object DeleteRoleDetail(int roleDetailId)
        { return "待完善"; }
    }
}
