using System.Data.SqlClient;

namespace Jetone.OrganizationalStructure
{

    public static class Sql
    {
        public static SqlConnection CreateConnection()
        {
            return new SqlConnection("data source=T105;initial catalog=Organization2;persist security info=True;user id=REP;password=SQL+Server;");
        }


        public const string GetCompanyUser = @"SELECT
	u.Id AS UserId
   ,u.Name AS UserName
   ,u.CompanyId
FROM Organization2..[User] u
WHERE u.CompanyId IN (@companyId) and u.Status=1";

        public const string GetRoleMember = @"SELECT
	u.Name AS UserName
   ,u.Id AS UserId
   ,c.Name AS Company
   ,u.Description AS Job
,c.Id as CompanyId
FROM Organization2..[User] u
INNER JOIN Organization2..Company c
	ON c.Id = u.CompanyId
INNER JOIN Organization2..GroupMember gm
	ON gm.UserId = u.Id
INNER JOIN Organization2..ResourceGroup rg
	ON rg.Id = gm.GroupId
		AND rg.SystemId = @systemId
WHERE u.CompanyId in(@companyId)";
        public const string GetOneRoleMember = @"SELECT
	t.UserName
   ,t.UserId
   ,t.Company
   ,t.Job
   ,t.CompanyId
   ,t.GroupId
FROM (SELECT
		u.Name AS UserName
	   ,u.Id AS UserId
	   ,c.Name AS Company
	   ,u.Description AS Job
	   ,c.Id AS CompanyId
	   ,gm.GroupId
	FROM Organization2..[User] u
	INNER JOIN Organization2..Company c
		ON c.Id = u.CompanyId
	LEFT JOIN Organization2..GroupMember gm
		ON gm.UserId = u.Id) t
LEFT JOIN Organization2..ResourceGroup rg
	ON rg.Id = t.GroupId
		AND rg.SystemId = 6
WHERE t.CompanyId IN (@companyId)
AND (t.GroupId != @RoleId
OR t.GroupId IS NULL) 
";
        public const string GetRoleUrlbyId = @"
SELECT
	t.MenuId
   ,t.Name
   ,t.Href
   ,t.ParentMenuId
   ,App.dbo.Concatenate(ISNULL(t.NotOperateAuth, '') + ISNULL(t.OperateAuth, ''), '/') AS Auth
FROM (SELECT
		m.Id AS MenuId
	   ,m.Name
	   ,m.Url AS Href
	   ,m.ParentMenuId
	   ,CASE
			WHEN rgd.Status = 0 THEN App.dbo.Concatenate(oa.Name, ',')
		END AS NotOperateAuth
	   ,CASE
			WHEN rgd.Status = 1 THEN App.dbo.Concatenate(oa.Name, ',')
		END AS OperateAuth
	   ,rgd.Status
	FROM [Organization2].[dbo].[ResourceGroup] rg
	INNER JOIN Organization2..ResourceGroupDetail rgd
		ON rgd.ResourceGroupId = rg.Id
	INNER JOIN Organization2..MenuUrl m
		ON m.Id = rgd.MenuUrlId
	INNER JOIN Organization2..OperationAuthority oa
		ON oa.Id = rgd.OperationAuthId
	WHERE rg.Id = @roleId
	AND m.SystemId = @systemId
	AND rg.CompanyId = @companyId
	GROUP BY m.Id
			,m.Name
			,m.Url
			,m.ParentMenuId
			,rgd.Status) t
GROUP BY t.MenuId
		,t.Name
		,t.Href
		,t.ParentMenuId  ";
        public const string DelRoleMember = @"DELETE Organization2..GroupMember  where GroupId=@roleId and User in(@userId)";
        public const string BingRoleWithUser = @"insert Organization2..GroupMember (GroupId,UserId) VALUES(@RoleId,@UserId);";
        public const string UpdateRoleMenuAuth = @"  update Organization2..ResourceGroupDetail set status=@status where ResourceGroupId=@roleId and MenuUrlId=@menuId and OperationAuthId=@authId";
        public const string InserRoleDetial = @"insert Organization2..ResourceGroupDetail(ResourceGroupId,MenuUrlId,OperationAuthId) VALUES(@roleId,@MenuId,@AuthId)";
        public const string GetMenuUrlId = @"SELECT Id
  FROM [Organization2].[dbo].[MenuUrl]
  where CompanyId=@companyId";
        public const string InsertAuth = @"insert Organization2..OperationAuthority (Name,SystemId,Description)VALUES(@Name,@SystemId,@Description);SELECT
	CAST(SCOPE_IDENTITY() AS int);";
        public const string InsertMenuUrl = @"insert Organization2..MenuUrl (Url,Name,ParentMenuId,Decription,companyId) VALUES(@Url,@Name,@ParentMenuId,@Description,@companyId)";
        public const string UpdateRoleBasicInfo = @"UPDATE Organization2..ResourceGroup set Name=@RoleName,Description=@Description where Id=@roleId";
        public const string GetSystemAllCompanyInfo = @"SELECT
	c.Id
   ,c.Name,
   c.ParentCompanyId
FROM Organization2..Company c
inner join Organization2..CompanySystemList cl on cl.CompanyId=c.Id
where cl.SystemId=@systemId";
        public const string GetCompanyInfo = @"  SELECT
	c.Id
   ,c.Name,
   c.ParentCompanyId
FROM Organization2..Company c
WHERE ParentCompanyId = @companyId
AND c.Status = 1";
        public const string GetChildMenuData = @"   ";
        public const string GetMainMenuData = @"SELECT
	m.Id as MenuId
   ,m.Name AS Menu
   ,m.Url AS Href
   ,m.ParentMenuId 
FROM [Organization2].[dbo].[ResourceGroup] rg
INNER JOIN Organization2..ResourceGroupDetail rgd
	ON rgd.ResourceGroupId = rg.Id
INNER JOIN Organization2..MenuUrl m
	ON m.Id = rgd.MenuUrlId
	where rg.Name=@role and m.ParentMenuId=0 and rg.CompanyId=@companyId";

        public const string GetAllOperateAuth = @"SELECT
	[Name] 
FROM [Organization2].[dbo].[OperationAuthority]
WHERE SystemId = @systemId";
        public const string GetRoleUrl = @"SELECT
	m.Id AS MenuId
   ,m.Name
   ,m.Url AS Href
   ,m.ParentMenuId
   ,CASE
		WHEN rgd.Status = 0 THEN App.dbo.Concatenate(oa.Name, ',')
	END AS NotOperateAuth
   ,CASE
		WHEN rgd.Status = 1 THEN App.dbo.Concatenate(oa.Name, ',')
	END AS OperateAuth
FROM [Organization2].[dbo].[ResourceGroup] rg
INNER JOIN Organization2..ResourceGroupDetail rgd
	ON rgd.ResourceGroupId = rg.Id
INNER JOIN Organization2..MenuUrl m
	ON m.Id = rgd.MenuUrlId
INNER JOIN Organization2..OperationAuthority oa
	ON oa.Id = rgd.OperationAuthId
WHERE rg.Name = @role
AND m.SystemId = @systemId 
AND rg.CompanyId = @companyId
GROUP BY m.Id
		,m.Name
		,m.Url
		,m.ParentMenuId
		,rgd.Status";
        public const string GetSystemInfo = @" 	select s.Id as SystemId,s.Name as SystemName
	from Organization2..SystemInfo s
	where s.Name=@systemName";
        public const string GetAllOrOneDepartment = @"SELECT
	d.Id as DepartmentId
   ,d.Name as DepartmentName
FROM Organization2..Department d
WHERE d.CompanyId = @companyId /*condition*/";
        public const string DeleteRoleDetail = @"UPDATE Organization2..ResourceGroup set Status=1 where Id=@RoleId";
        public const string CreateRoleData = @"insert Organization2..ResourceGroup(CompanyId,ResourceType,SystemId,Name,Description) VALUES(@CompanyId,'RoleResource',@SystemId,@RoleName,@Description);SELECT
	CAST(SCOPE_IDENTITY() AS int);";
        public const string UpdateRoleStatus = @"update Organization2..ResourceGroup set Status=0 where Id=@roleId and CompanyId=@companyId and SystemId=@SystemId";
        public const string UpdateUserStatus = @"update Organization2..[User] set Status=1 where Id=@UserId and CompanyId=@CompanyId";
        public const string UpdateCompanyDepartment = @"update Organization2..ResourceGroup set Status=1 where  CompanyId=@CompanyId";
        public const string UpdateCompanySystem = @"update Organization2..ResourceGroup set Status=1 where SystemId=@systemId and CompanyId=@CompanyId";
        public const string UpdateRoleDetail = @"insert Organization2..RoleResourceDetail(ResourceGroupId,Url,Type) VALUES(@RoleId,@Url,@Type)";
        public const string UpdateUserRole = @"update Organization2..GroupMember set GroupId=@NewRoleId where UserId=@UserId and GroupId=@OldRoleId";
        public const string ChangeDepartmentMember = @"Update Organization2..[User] set DepartmentId=@DepartmentId where Id=@UserId";
        public const string UpdateDepartmentInfo = @"update Organization2..Department set Name=@departmentName where Id=@DepartmentId";
        public const string UpdateUserInfo = @"update Organization2..[User] set Name=@userName,Tel=@Tel,Password=@password where Id=@userId";
        public const string UpdateCompanyInfo = @"update Organization2..Company set Name=@companyName where Id=@CompanyId
";
        public const string OpenCompany = @"UPDATE Organization2..ResourceGroup Set Status=0 where Status=1 and CompanyId=@companyId  and SystemId=@SystemId";
        public const string BingGroup = @"insert Organization2..GroupMember (GroupId,UserId) VALUES(@RoleId,@UserId);  ";
        public const string CreateRoleDetail = @"insert Organization2..ResourceGroupDetail(ResourceGroupId,MenuUrlId,OperationAuthId)VALUES(@RoleId,@Url,@AuthId)";
        public const string CreateResourceGroup = @"insert Organization2..ResourceGroup(CompanyId,ResourceType,SystemId,Name) VALUES(@companyId,'RoleResource',@SystemId,@RoleName);SELECT
	CAST(SCOPE_IDENTITY() AS int); ";
        public const string CreateUserSql = @" insert into Organization2..[User](Name,Password,CompanyId,DepartmentId) VALUES(@UserName,@Password,@CompanyId,@DepartmentId);SELECT
	CAST(SCOPE_IDENTITY() AS int); ";
        public const string CreateDepartmentSql = @"insert Organization2..Department(Name,CompanyId) VALUES(@DepartmentName,@CompanyId);SELECT
	CAST(SCOPE_IDENTITY() AS int); ";
        public const string CreateCompanySql = @"insert Organization2..Company(Name,ParentCompanyId) VALUES(@CompanyName,@ParentCompanyId);SELECT
	CAST(SCOPE_IDENTITY() AS int);";
        public const string GetRoleDetail = @"SELECT
	rrd.ResourceGroupId AS RoleId
   ,u.Url
FROM Organization2..ResourceGroupDetail rrd
INNER JOIN Organization2..MenuUrl u
	ON u.Id = rrd.MenuUrlId and rrd.Status=1
WHERE rrd.ResourceGroupId IN (@RoleId) ";
        public const string GetUserRoleData = @"SELECT
	rg.Id AS RoleId
   ,rg.Name AS RoleName
   ,rg.SystemId
FROM Organization2..GroupMember gm
INNER JOIN Organization2..ResourceGroup rg
	ON rg.Id = gm.GroupId 
WHERE gm.UserId = @UserId and rg.Status=1";

        public const string GetSystemRole = @"SELECT
	rg.Id as RoleId
   ,rg.Name as RoleName
FROM Organization2..ResourceGroup rg
WHERE   rg.SystemId = @systemId 
AND rg.Status = 0 ";
        public const string GetCompanyRoleWithName = @"SELECT
	rg.Id as RoleId
   ,rg.Name as RoleName
FROM Organization2..ResourceGroup rg
WHERE rg.CompanyId = @companyId
AND rg.SystemId = @systemId 
AND rg.Status = 0 and rg.Name=@name";
        public const string GetCompanyRole = @"SELECT
	rg.Id as RoleId
   ,rg.Name as RoleName
FROM Organization2..ResourceGroup rg
WHERE rg.CompanyId = @companyId
AND rg.SystemId = @systemId 
AND rg.Status = 0 ";
        public const string GetCompanyDepartment = @"select d.Id AS DepartmentId,d.Name AS DepartmentName
from Organization2..Department d
where d.CompanyId=@CompanyId";
        public const string LoginSql = @"SELECT
	u.Id AS UserId
   ,u.Name AS UserName
   ,u.CompanyId
FROM Organization2..[User] u
INNER JOIN Organization2..Company c
	ON c.Id = u.CompanyId
WHERE u.Name = @UserName
AND u.Password = @Password
AND c.Status = 1
AND u.Status = 1";
        public const string GetCompanyStructure = @"select d.Id as CompanyId,d.Name as CompanyName
from Organization2..Company d
where d.Id=@companyId and d.Status=1";
        public const string GetDepartmentMember = @"SELECT
	u.Id AS UserId
   ,u.Name AS UserName
   ,u.Tel
FROM Organization2..Department d
INNER JOIN Organization2..[User] u
	ON u.DepartmentId = d.Id
WHERE d.Id = @DepartmentId";
        public const string GetCompanySystem = @"select si.Id,si.Name
from  Organization2..ResourceGroup rg 
inner join Organization2..SystemInfo si on si.Id=rg.SystemId
where  rg.CompanyId=@CompanyId and rg.Status=0";
    }

}
