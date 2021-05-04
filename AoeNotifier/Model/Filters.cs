using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AoeNotifier.Model
{
    class Filters
    {
        public List<Filter> FilterList { get; set; }

        public Filters()
        {
            FilterList = new List<Filter>();
        }

        public void AddFilter(Filter filter)
        {
            if (FilterList == null)
            {
                FilterList = new List<Filter>();
            }

            filter.id = GetNextId();
            filter.GroupId = GetNextGroupId();
            FilterList.Add(filter);
            RefreshColorsAndGroups();

        }

        public void DeleteByIds(List<int> idsToDelete)
        {
            for (int i = FilterList.Count - 1; i >= 0; i--)
            {
                Filter filter = FilterList[i];
                if (idsToDelete.Contains(filter.id))
                {
                    FilterList.RemoveAt(i);
                }
            }

            RefreshColorsAndGroups();
        }

        public void JoinByIds(List<int> idsToJoin)
        {
            var filtersToJoin = from f in FilterList where idsToJoin.Contains(f.id) select f;
            int topGroup = GetNextGroupId();

            foreach(Filter filter in filtersToJoin)
            {
                if (filter.filterMode == FilterMode.Ignore)
                {
                    MessageBox.Show("Ignore filters (red) can not be joined!");
                    return;
                }
            }

            foreach (Filter filter in filtersToJoin)
            {
                filter.GroupId = topGroup;
            }

            RefreshColorsAndGroups();
        }

        public int GetNextGroupId()
        {
            if (FilterList?.Any() != true)
            {
                return 0;
            }

            return FilterList.Max(f => f.GroupId) + 1;
        }

        public int GetNextId()
        {
            if (FilterList?.Any() != true)
            {
                return 0;
            }

            return FilterList.Max(f => f.id) + 1;
        }

        public void RefreshColorsAndGroups()
        {
            if (FilterList?.Any() != true)
            {
                return;
            }

            FilterList.Sort((f1, f2) => f1.GroupId.CompareTo(f2.GroupId));
            var groupedFilters = FilterList.GroupBy(f => f.GroupId);

            int i = -1;

            foreach (var filterGroup in groupedFilters)
            {
                i++;

                foreach (Filter filter in filterGroup)
                {
                    if (filter.filterMode == FilterMode.Ignore)
                    {
                        filter.Color = Colors.FiltersNegativeFilter;
                        continue;
                    }

                    if (i % 2 == 0)
                    {
                        filter.Color = Colors.FiltersLightZebra;
                    }
                    else
                    {
                        filter.Color = Colors.FiltersDarkZebra;
                    }
                }
            }
        }
    }

}
