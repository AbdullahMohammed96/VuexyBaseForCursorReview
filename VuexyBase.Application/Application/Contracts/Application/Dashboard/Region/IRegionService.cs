using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VuexyBase.Application.Application.Common.Results;
using VuexyBase.Application.Application.ViewModels.Region;

namespace VuexyBase.Application.Application.Contracts.Application.Dashboard.Region
{
    public interface IRegionService
    {
        //Task<ListRegionViewModel> GetAllRegions();

        Task<bool> EditRegion(UpdateRegionViewModel regionVM); 
        Task<bool> CreateRegion(AddRegionViewModel region);
        Task<bool> DeleteRegion(int id);

        Task<bool> ChangeStatus(int id);
        Task<ListResultViewModel<RegionsList>> FilterRegions(int length, string search, string status);
    }
}