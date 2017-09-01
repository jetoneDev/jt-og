using System.Data.SqlClient;

namespace Jetone.OrganizationalStructure
{

    public static class Sql
    {
        public static SqlConnection CreateConnection()
        {
            return new SqlConnection("data source=T105;initial catalog=Organization;persist security info=True;user id=REP;password=SQL+Server;");
        }

        public const string GetSystemInfo = @" 	select s.Id as SystemId,s.Name as SystemName
	from Organization..SystemInfo s
	where s.Name=@systemName";
        public const string GetAllOrOneDepartment = @"SELECT
	d.Id as DepartmentId
   ,d.Name as DepartmentName
FROM Organization..Department d
WHERE d.CompanyId = @companyId /*condition*/";
        public const string DeleteRoleDetail = @"delete Organization..RoleResourceDetail where ResourceGroupId=@roleId";
        public const string CreateRoleData = @"insert Organization..ResourceGroup(CompanyId,ResourceType,SystemId,Name) VALUES(@CompanyId,'RoleResource',@SystemId,@RoleName);SELECT
	CAST(SCOPE_IDENTITY() AS int);";
        public const string UpdateRoleStatus = @"update Organization..ResourceGroup set Status=1 where Id=@roleId and CompanyId=@companyId";
        public const string UpdateUserStatus = @"update Organization..[User] set Status=1 where Id=@UserId and CompanyId=@CompanyId";
        public const string UpdateCompanyDepartment = @"update Organization..ResourceGroup set Status=1 where  CompanyId=@CompanyId";
        public const string UpdateCompanySystem = @"update Organization..ResourceGroup set Status=1 where SystemId=@systemId and CompanyId=@CompanyId";
        public const string UpdateRoleDetail = @"insert Organization..RoleResourceDetail(ResourceGroupId,Url,Type) VALUES(@RoleId,@Url,@Type)";
        public const string UpdateUserRole = @"update Organization..GroupMember set GroupId=@NewRoleId where UserId=@UserId and GroupId=@OldRoleId";
        public const string ChangeDepartmentMember = @"Update Organization..[User] set DepartmentId=@DepartmentId where Id=@UserId";
        public const string UpdateDepartmentInfo = @"update Organization..Department set Name=@departmentName where Id=@DepartmentId";
        public const string UpdateUserInfo = @"update Organization..[User] set Name=@userName,Tel=@Tel,Password=@password where Id=@userId";
        public const string UpdateCompanyInfo = @"update Organization..Company set Name=@companyName where Id=@CompanyId
";
        public const string OpenCompany = @"UPDATE Organization..ResourceGroup Set Status=0 where Status=1 and CompanyId=@companyId  and SystemId=@SystemId";
        public const string BingGroup = @"insert Organization..GroupMember (GroupId,UserId) VALUES(@RoleId,@UserId);  ";
        public const string CreateRoleDetail = @"insert Organization..RoleResourceDetail(ResourceGroupId,Url,Type) VALUES(@roleId,@url,@Type);SELECT
	CAST(SCOPE_IDENTITY() AS int);";
        public const string CreateResourceGroup = @"insert Organization..ResourceGroup(CompanyId,ResourceType,SystemId,Name) VALUES(@companyId,'RoleResource',@SystemId,@RoleName);SELECT
	CAST(SCOPE_IDENTITY() AS int); ";
        public const string CreateUserSql = @" insert into Organization..[User](Name,Tel,Password,CompanyId,DepartmentId) VALUES(@UserName,@Tel,@Password,@CompanyId,@DepartmentId);SELECT
	CAST(SCOPE_IDENTITY() AS int); ";
        public const string CreateDepartmentSql = @"insert Organization..Department(Name,CompanyId) VALUES(@DepartmentName,@CompanyId);SELECT
	CAST(SCOPE_IDENTITY() AS int); ";
        public const string CreateCompanySql = @"insert Organization..Company(Name) VALUES(@CompanyName);SELECT
	CAST(SCOPE_IDENTITY() AS int);";
        public const string GetRoleDetail = @"select rrd.ResourceGroupId as RoleId, rrd.Url
from Organization..RoleResourceDetail rrd
where rrd.ResourceGroupId in(@RoleId);";
        public const string GetUserRoleData = @"SELECT
	rg.Id AS RoleId
   ,rg.Name AS RoleName
   ,rg.SystemId
FROM Organization..GroupMember gm
INNER JOIN Organization..ResourceGroup rg
	ON rg.Id = gm.GroupId 
WHERE gm.UserId = @UserId and rg.Status=0";

        public const string GetCompanyRole = @"SELECT
	rg.Id as RoleId
   ,rg.Name as RoleName
FROM Organization..ResourceGroup rg
WHERE rg.CompanyId = @companyId
AND rg.SystemId = @systemId 
AND rg.Status = 0 and rg.Name=@roleName";
        public const string GetCompanyDepartment = @"select d.Id AS DepartmentId,d.Name AS DepartmentName
from Organization..Department d
where d.CompanyId=@CompanyId";
        public const string LoginSql = @"SELECT
	u.Id as UserId
   ,u.Name AS UserName
   ,u.CompanyId
FROM Organization..[User] u
INNER JOIN Organization..Company c
	ON c.Id = u.CompanyId
WHERE u.Name = @UserName
AND u.Password = @Password
AND c.Status = 0
AND u.Status = 0";
        public const string GetCompanyStructure = @"select d.Id,d.Name
from Organization..Company c
inner join Organization..Department d on d.CompanyId=c.Id";
        public const string GetDepartmentMember = @"SELECT
	u.Id AS UserId
   ,u.Name AS UserName
   ,u.Tel
FROM Organization..Department d
INNER JOIN Organization..[User] u
	ON u.DepartmentId = d.Id
WHERE d.Id = @DepartmentId";
        public const string GetCompanySystem = @"select si.Id,si.Name
from  Organization..ResourceGroup rg 
inner join Organization..SystemInfo si on si.Id=rg.SystemId
where  rg.CompanyId=@CompanyId and rg.Status=0";
    }

}
