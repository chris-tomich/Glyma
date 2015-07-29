using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses
{
    public class BreadcrumbTrail
    {
        #region variables
        private readonly BreadcrumbEllipsis _ellipsis;
        private ObservableCollection<BreadcrumbControl> _breadcrumbs;

        public event EventHandler PropertiesClicked;
        public event EventHandler<BreadcrumbChangedEventArgs> BreadcrumbChanged;
        public event EventHandler<BreadcrumbClickedEventArgs> BreadcrumbClicked;
        #endregion

        #region properties
        private BreadcrumbContainer EllipsisContainer
        {
            get
            {
                return _ellipsis.BreadcrumbContainer;
            }
        }

        private BreadcrumbContainer MainContainer
        {
            get;
            set;
        }

        private ObservableCollection<BreadcrumbControl> Breadcrumbs
        {
            get
            {
                if (_breadcrumbs == null)
                {
                    _breadcrumbs = new ObservableCollection<BreadcrumbControl>();
                }
                return _breadcrumbs;
            }
        }

        public int Count
        {
            get
            {
                return Breadcrumbs.Count;
            }
        }

        #endregion

        public BreadcrumbTrail(BreadcrumbContainer main, BreadcrumbEllipsis ellipsis)
        {
            MainContainer = main;
            _ellipsis = ellipsis;
            Breadcrumbs.CollectionChanged += BreadcrumbsOnCollectionChanged;
        }

        #region private methods
        private void BreadcrumbsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MainContainer.Clear();
            _ellipsis.Clear();
            if (Count > 3)
            {
                _ellipsis.Visibility = Visibility.Visible;

                for (var i = Count - 3; i < Count; i++)
                {
                    MainContainer.Add(Breadcrumbs[i]);
                }

                for (var i = 0; i < Count - 3; i++)
                {
                    EllipsisContainer.Add(Breadcrumbs[i]);
                }
            }
            else
            {
                _ellipsis.Visibility = Visibility.Collapsed;
                foreach (var breadcrumbItem in Breadcrumbs)
                {
                    MainContainer.Add(breadcrumbItem);
                }
            }

            var last = Breadcrumbs.LastOrDefault();
            if (last != null)
            {
                last.BreadcrumbData.HasVisibleProperties = App.UserStyle == UserStyle.Author;
            }

            // Mark the first breadcrumb
            var index = 0;
            foreach (var breadcrumbControl in Breadcrumbs)
            {
                breadcrumbControl.BreadcrumbData.IsFirst = index == 0;
                index ++;
            }
        }

        private void OnPropertiesClicked(object sender, EventArgs e)
        {
            if (PropertiesClicked != null)
            {
                PropertiesClicked(sender, e);
            }
        }

        private void OnBreadcrumbClicked(object sender, EventArgs e)
        {
            var breadcrumb = sender as BreadcrumbControl;
            if (breadcrumb != null)
            {
                RemoveTo(breadcrumb);
                var args = new BreadcrumbClickedEventArgs();
                if (!IsContainRootMap())
                {
                    args.IsParentRemoved = true;
                    RemoveParentOf(breadcrumb);
                    args.ParentIndex = GetInsertPosition(breadcrumb);
                }

                if (BreadcrumbClicked != null)
                {
                    BreadcrumbClicked(sender, args);
                }
            }
        }

        private void OnBreadcrumbChanged(object sender, BreadcrumbChangedEventArgs e)
        {
            var breadcrumb = sender as BreadcrumbControl;
            if (breadcrumb != null)
            {
                RemoveTo(breadcrumb);
                RemoveParentOf(breadcrumb);

                e.ParentIndex = GetInsertPosition(breadcrumb);

                if (BreadcrumbChanged != null)
                {
                    BreadcrumbChanged(sender, e);
                }
            }
        }

        private void RemoveParentOf(BreadcrumbControl breadcrumb)
        {
            var index = Breadcrumbs.IndexOf(breadcrumb);
            if (index > 0)
            {
                Breadcrumbs.RemoveAt(index -1);
            }
        }

        private BreadcrumbControl CreateBreadcrumb(IBreadcrumbItem item)
        {
            var control = new BreadcrumbControl {DataContext = item};
            control.BreadcrumbChanged += OnBreadcrumbChanged;
            control.BreadcrumbClicked += OnBreadcrumbClicked;
            control.PropertiesClicked += OnPropertiesClicked;
            return control;
        }

        private int GetInsertPosition(BreadcrumbControl breadcrumb)
        {
            var index = Breadcrumbs.IndexOf(breadcrumb);
            if (index < 0)
            {
                index = 0;
            }
            return index;
        }

        private bool IsContainRootMap()
        {
            var nodes = Breadcrumbs.Select(q => q.BreadcrumbData.Node);
            return nodes.Any(q => q.IsRootMap);
        }

        #endregion

        #region public methods
        public void Add(IBreadcrumbItem item)
        {
            var exist = Breadcrumbs.FirstOrDefault(q => q.BreadcrumbData.Node.Id == item.Node.Id);
            if (exist != null)
            {
                RemoveTo(exist);
            }
            else
            {
                Breadcrumbs.Add(CreateBreadcrumb(item));
            }
        }

        public void Insert(IBreadcrumbItem item, int index = 0)
        {
            Breadcrumbs.Insert(index, CreateBreadcrumb(item));
        }

        public void RemoveTo(BreadcrumbControl item)
        {
            var index = Breadcrumbs.IndexOf(item);

            while (Breadcrumbs.Count > index + 1)
            {
                Breadcrumbs.RemoveAt(index + 1);
            }
        }

        public void Clear()
        {
            Breadcrumbs.Clear();
            MainContainer.Clear();
            EllipsisContainer.Clear();
        }

        public void RootMapFound()
        {
            var first = Breadcrumbs.FirstOrDefault();
            if (first != null)
            {
                first.BreadcrumbData.Node.IsRootMap = true;
            }
        }

        #endregion
    }
}
