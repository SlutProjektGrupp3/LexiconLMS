using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace LMS.Shared.Request
{
    public class PagedList<T>
    {
        public IReadOnlyList<T> Items { get; }
        public MetaData MetaData { get; }

        private PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData
            (
                currentPage: pageNumber,
                totalPages: (int)Math.Ceiling(count / (double)pageSize),
                pageSize: pageSize,
                totalCount: count
            );

            Items = new List<T>(items).AsReadOnly();
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
