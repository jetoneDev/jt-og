using AppHelpers;
using Jetone.OrganizationalStructure.Model;
using System;
using System.Collections.Generic;

namespace Jetone.OrganizationalStructure
{
    public static partial class Organization
    {
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
        /// 修改公司基本信息，去除token
        /// </summary>
        /// <param name="data"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static bool UpdateCompanyInfo(UpdateCompanyInfoData data, int companyId)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateCompanyInfo, new { data.CompanyName, companyId }) <= 0 ? false : true;

            }
        }

        /// <summary>
        /// 修改公司基本信息
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static bool UpdateCompanyInfo(string companyName, int companyId)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateCompanyInfo, new { companyName, companyId }) <= 0 ? false : true;

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
        /// 修改账号信息（去除token）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateUserInfo(UpdateUserInfoData data)
        {

            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateUserInfo, new { data.UserName, data.Tel, data.Password, data.UserId }) == 0 ? false : true;
            }
        }

        /// <summary>
        /// 修改部门信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool UpdateDepartmentInfo(UpdateDepartmentData data, string token)
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
        /// 修改部门信息（去除token）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateDepartmentInfo(UpdateDepartmentData data)
        {
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
        /// 修改组内人员信息（去除Token）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static object UpdateDepartmentMember(UpdateDepartmentMember data, int userId)
        {

            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.ChangeDepartmentMember, new { data.DepartmentId, userId }) == 0 ? false : true;
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
        /// 修改账号角色信息（去除token）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateUserRole(UpdateUserRoleData data, int userId)
        {


            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            {

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
                var updateRoleDetail = t.Update(Sql.UpdateRoleDetail, new { data.RoleId }, data.Url.ConvertAll(d => new { Url = d.Url }));
                if (deleteRoleDetail > 0 & updateRoleDetail >= 1)
                {
                    t.Commit();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 修改角色资源（取消Token)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateRoleDetail(UpdateRoleDetailData data)
        {
            if (data == null)
            {
                throw new ArgumentException("data 参数不能为空");
            }
            using (var c = Sql.CreateConnection())
            using (var t = c.OpenTransaction())
            {
                var deleteRoleDetail = t.Update(Sql.DeleteRoleDetail, new { data.RoleId });
                var updateRoleDetail = t.Update(Sql.UpdateRoleDetail, new { data.RoleId }, data.Url.ConvertAll(d => new { Url = d.Url }));
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

        /// <summary>
        /// 修改角色在菜单上的权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="b">true 为开启权限，false 为关闭权限</param>
        /// <returns></returns>
        public static bool UpdateRoleMenuAuth(List<RoleMenuAuth> data)
        {
            var d = new List<RoleMenuAuth>();
            using (var c = Sql.CreateConnection())
            {

                return c.Update(Sql.UpdateRoleMenuAuth, d) == d.Count ? false : true;
            }
        }




        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static bool UpdateRole(RoleData data)
        {
            using (var c = Sql.CreateConnection())
            {
                return c.Update(Sql.UpdateRoleBasicInfo, data) == 0 ? false : true;
            }
        }
        //TODO 当管理员停用资源组时，再次重新创建相同名称的资源组时候发生错误



        public class RoleMenuAuth
        {
            public int Role { get; set; }
            public int AuthId { get; set; }
            public int Status { get; set; }
            public int MenuId { get; set; }
        }

    }
}
