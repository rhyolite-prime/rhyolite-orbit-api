using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Folders;

public interface IFolderManager
{
    Task<DmsFolder> CreateAsync(DmsFolder folder);

    Task<DmsFolder> UpdateAsync(DmsFolder folder);

    Task MoveAsync(Guid folderId, Guid? newParentId, long movedBy);

    Task<DmsFolder> CopyAsync(Guid folderId, Guid? targetParentId, long copiedBy);

    Task DeleteAsync(Guid folderId, long deletedBy);

    Task RestoreAsync(Guid folderId, long restoredBy);

    Task<List<DmsFolder>> GetChildrenAsync(Guid? parentId, bool includeDeleted = false);

    Task<List<DmsFolder>> GetTreeAsync();

    Task<DmsFolder> GetAsync(Guid folderId);

    Task<DmsFolder> GetRootFolderAsync();

    Task<int> CountDocumentsAsync(Guid folderId);

    Task RebuildPathsAsync();
}
