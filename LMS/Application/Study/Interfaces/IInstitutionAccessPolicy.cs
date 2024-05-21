﻿using LMS.Domain.Study.Entities;
using LMS.Domain.User.Enums;

namespace LMS.Application.Study.Interfaces
{
    public interface IInstitutionAccessPolicy
    {
        Task EnforceRole(InstitutionRolesEntity role, InstitutionMemberEntity member);
        Task EnforceRole(string roleName, InstitutionMemberEntity member);
        Task EnforcePermission(string permissionAcl, InstitutionMemberEntity member);
        Task EnforcePermission(PermissionEnum action, string subjectName, InstitutionMemberEntity member);
        Task<InstitutionMemberEntity> GetMember(Guid userId, Guid institutionId); 
    }
}
