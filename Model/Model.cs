using System.Collections.Generic;

namespace Jetone.OrganizationalStructure.Model
{

    public class RoleUrlDetail
    {
        public int Id { get; set; }
        public string Menu { get; set; }
        public string Url { get; set; }
        public int ParentMenuId { get; set; }
        public string OperateAuth { get; set; }
    }


    public class SystemInfo
    {
        public int SystemId { get; set; }
        public string SystemName { get; set; }
    }
    public class UserData
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Tel { get; set; }
    }
    public class DepartmentData
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
    public class BingSystemToUserData
    {

        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int SystemId { get; set; }
        public string RoleName { get; set; }
    }
    public class CreateRoleData
    {
        public string RoleName { get; set; }
    }

    public class CreateRoleDetailData
    {
        public int RoleId { get; set; }
        public List<UrlData> Url { get; set; }
    }
    public class UpdateRoleDetailData
    {
        public int RoleId { get; set; }
        public List<UrlData> Url { get; set; }
    }
    public class UpdateUserRoleData
    {
        public int NewRoleId { get; set; }
        public int OldRoleId { get; set; }
    }
    public class UpdateDepartmentMember
    {
        public int DepartmentId { get; set; }
    }
    public class UpdateCompanyInfoData
    {
        public string CompanyName { get; set; }
    }



    public class UpdateUserInfoData
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Tel { get; set; }
        public string Password { get; set; }
    }
    public class UpdateDepartmentData
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
    //public class CreateUserData
    //{
    //    public string UserName { get; set; }
    //    public string Password { get; set; }
    //    public string Tel { get; set; }
    //    public int DepartmentId { get; set; }
    //}
    public class CreateDepartmentData
    {
        public string DepartmentName { get; set; }
    }
    public class CreateUserData
    {
        public int RoleId { get; set; }
        public string UserName { get; set; }
        public string Tel { get; set; }
        public string Password { get; set; }
        public int DepartmentId { get; set; }
    }
    public class CreateCompanyData
    {
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public int SystemId { get; set; }
        public int ParentCompanyId { get; set; }
        public List<UrlData> Url { get; set; } = new List<UrlData>();
    }
    public class UrlData
    {
        public string Url { get; set; }
        public string ParentMenuId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OrderNum { get; set; }
    }
    internal class RoleDetail
    {
        public int RoleId { get; set; }
        public string Url { get; set; }
    }
    public class LoginData
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public IList<RoleData> Data { get; set; }
    }
    public class RoleDataInfo
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
    public class RoleData
    {
        public string RoleName { get; set; }
        public string Decription { get; set; }
        public int RoleId { get; set; }
        public int SystemId { get; set; }
        public List<string> UrlDetail { get; set; } = new List<string>();
    }

    public class CompanyInfo
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public IList<DepartmentData> DepartmentInfo { get; set; }
    }
    internal class TokenConfig
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int SystemId { get; set; }
        public string RoleId { get; set; }//ResourceGroupId  undetermined
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
