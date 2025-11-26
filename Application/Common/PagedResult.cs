using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    internal class PagedResult<T> : Result<IEnumerable<T>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static PagedResult<T> Create(IEnumerable<T> data,int totalCount, int pageIndex, int pageSize)
        {
            return new PagedResult<T>
            {
                Success = true,
                Data = data,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
